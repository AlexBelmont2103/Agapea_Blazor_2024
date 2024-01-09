using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Agapea_Blazor_2024.Server.Models
{
    public class AplicacionDBContext : IdentityDbContext
    {
        /*
            Esta clase va a servir para que Identity genere a traves de EF el DbContext para generar tablas a traves de las clases modelo
            usando DbSets... como estamos creando un DbContext personalizado porque vamos a añadir tablas propias ademas de las de Identity
            EF te obliga a sobrecargar el constructor (si no lo pones, salta un error indicándote lo que debes hacer)
         */

        public AplicacionDBContext(DbContextOptions<AplicacionDBContext> options):base(options)
        {
            
        }

        #region ... propiedades de la clase AplicacionDBContext ...
        //Definimos un DbSet por cada clase modelo a mapear en el DbContext como propiedad ...
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItemsPedido { get; set; }
        public DbSet<Categoria> Categorias { get; set; }


        #endregion

        #region ... metodos de la clase AplicacionDBContext ...
        //metodo que se ejecuta para crear la tabla a partir de las clases en el momento que se lanza la migracion
        //Para lanzar la migracion, en la consola de NuGet, en el proyecto de la solucion que contiene el DbContext, ejecutar:
        //      Add-Migration numero_NombreMigracion
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region /// MODIFICACION TABLA IDENTITYUSER PARA AÑADIR PROPIEDADES PERSONALIZADAS ///

            //IdentityUser es la clase que usa Identity para crear la tabla de usuarios en la BD
            //IdentityUser tiene una propiedad llamada Id que es la PK de la tabla
            //IdentityUser tiene una propiedad llamada UserName que es el nombre de usuario
            //IdentityUser tiene una propiedad llamada Email que es el email del usuario
            //IdentityUser tiene una propiedad llamada PasswordHash que es el hash de la contraseña del usuario
            //IdentityUser tiene una propiedad llamada PhoneNumber que es el numero de telefono del usuario
            builder.Entity<MiClienteIdentity>()
            #endregion

            #region /// CREACION DE TABLA DIRECCIONES A PARTIR DE LA CLASE MODELO DIRECCION ///

            builder.Entity<Direccion>().ToTable("Direcciones");
            builder.Entity<Direccion>().HasKey((Direccion dir) => dir.IdDireccion);
            builder.Entity<Direccion>().Property((Direccion dir) => dir.Calle).IsRequired().HasMaxLength(250);
            builder.Entity<Direccion>().Property((Direccion dir) => dir.CP).IsRequired().HasMaxLength(5);

            //Cuando la clase modelo tiene como propiedad una clase, EF no puede mapearlo contra un tipo de dato de SQLServer
            //Solucion: o no almacenas esa prop como columna en la tabla o la serializas a un tipo de dato que si pueda almacenarse en una columna
            // .HasConversion() es un metodo de EF que permite serializar una clase a un tipo de dato que si pueda almacenarse en una columna
            // .HasConversion(1º param lambda serializacion, 2º param lambda deserializacion)
            builder.Entity<Direccion>().Property((Direccion dir) => dir.Pais)
                .HasConversion(
                    prov => JsonSerializer.Serialize<Provincia>(prov, (JsonSerializerOptions)null),
                    prov => JsonSerializer.Deserialize<Provincia>(prov, (JsonSerializerOptions)null)
                );
                
            #endregion
        }
        #endregion
    }
}
