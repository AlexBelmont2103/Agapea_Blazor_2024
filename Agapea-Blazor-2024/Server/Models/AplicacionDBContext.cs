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

        public AplicacionDBContext(DbContextOptions<AplicacionDBContext> options) : base(options)
        {

        }

        #region ... propiedades de la clase AplicacionDBContext ...
        //Definimos un DbSet por cada clase modelo a mapear en el DbContext como propiedad ...
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItemsPedido { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<PedidoPayPal> pedidoPayPal { get; set; }


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

            builder.Entity<MiClienteIdentity>();
            #endregion
            #region /// CREACION DE TABLA PEDIDOSPAYPAL A PARTIR DE LA CLASE MODELO PEDIDOPAYPAL
            builder.Entity<PedidoPayPal>().ToTable("PedidosPayPal");
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
            /*
             //Podemos intentar almacenar solamente la propiedad CPRO + la propiedad PRO de la clase Provincia en la tabla Direcciones
             builder.Entity<Direccion>().Property((Direccion dir) => dir.ProvinciaDirecString).IsRequired();
             */
            builder.Entity<Direccion>().Property((Direccion dir) => dir.ProvinciaDirec)
                .HasConversion(
                prov => JsonSerializer.Serialize<Provincia>(prov, (JsonSerializerOptions)null),
                prov => JsonSerializer.Deserialize<Provincia>(prov, (JsonSerializerOptions)null)
                               ).HasColumnName("Provincia");
            builder.Entity<Direccion>().Property((Direccion dir) => dir.MunicipioDirec)
                .HasConversion(
                muni => JsonSerializer.Serialize<Municipio>(muni, (JsonSerializerOptions)null),
                muni => JsonSerializer.Deserialize<Municipio>(muni, (JsonSerializerOptions)null)
                               ).HasColumnName("Municipio");
            builder.Entity<Direccion>().Property((Direccion dir) => dir.Pais).IsRequired().HasMaxLength(50);
            builder.Entity<Direccion>().Property((Direccion dir) => dir.EsPrincipal).IsRequired().HasDefaultValue(false);
            builder.Entity<Direccion>().Property((Direccion dir) => dir.EsFacturacion).IsRequired().HasDefaultValue(false);

            #endregion
            #region /// CREACION DE TABLA LIBROS A PARTIR DE LA CLASE MODELO LIBRO ///
            builder.Entity<Libro>().ToTable("Libros");
            builder.Entity<Libro>().HasKey("ISBN13");
            builder.Entity<Libro>().Property((Libro lib) => lib.Precio).HasColumnType("DECIMAL(5,2)");
            #endregion
            #region /// CREACION DE TABLA ITEMMSPEDIDO A PARTIR DE LA CLASE MODELO ITEMPEDIDO 
            builder.Entity<ItemPedido>().ToTable("ItemsPedido");
            //Serializamos la propiedad LibroItem de la clase ItemPedido para poder almacenarla en una columna de la tabla ItemsPedido
            builder.Entity<ItemPedido>().HasNoKey();
            builder.Entity<ItemPedido>().Property((ItemPedido item) => item.LibroItem)
                .HasConversion(
                               libro => JsonSerializer.Serialize<Libro>(libro, (JsonSerializerOptions)null),
                                              libro => JsonSerializer.Deserialize<Libro>(libro, (JsonSerializerOptions)null)
                                                                            ).HasColumnName("LibroItem");
            builder.Entity<ItemPedido>().Property((ItemPedido item) => item.CantidadItem).IsRequired();

            #endregion
            #region /// CREACION DE TABLA PEDIDOS A PARTIR DE LA CLASE MODELO PEDIDO
            builder.Entity<Pedido>().ToTable("Pedidos");
            builder.Entity<Pedido>().HasKey((Pedido ped) => ped.IdPedido);
            builder.Entity<Pedido>().Property((Pedido ped) => ped.FechaPedido).IsRequired();
            //Serializamos la propiedad ElementosPedido de la clase Pedido para poder almacenarla en una columna de la tabla Pedidos
            builder.Entity<Pedido>().Property((Pedido ped) => ped.ElementosPedido)
                .HasConversion(
                 items => JsonSerializer.Serialize<List<ItemPedido>>(items, (JsonSerializerOptions)null),
                 items => JsonSerializer.Deserialize<List<ItemPedido>>(items, (JsonSerializerOptions)null)
                          ).HasColumnName("ElementosPedido");
            //Serializamos la propiedad DireccionEnvio de la clase Pedido para poder almacenarla en una columna de la tabla Pedidos
            builder.Entity<Pedido>().Property((Pedido ped) => ped.DireccionEnvio)
                .HasConversion(
                 dir => JsonSerializer.Serialize<Direccion>(dir, (JsonSerializerOptions)null),
                 dir => JsonSerializer.Deserialize<Direccion>(dir, (JsonSerializerOptions)null)
                          ).HasColumnName("DireccionEnvio");
            //Serializamos la propiedad DireccionFacturacion de la clase Pedido para poder almacenarla en una columna de la tabla Pedidos
            builder.Entity<Pedido>().Property((Pedido ped) => ped.DireccionFacturacion)
                .HasConversion(
                 dir => JsonSerializer.Serialize<Direccion>(dir, (JsonSerializerOptions)null),
                 dir => JsonSerializer.Deserialize<Direccion>(dir, (JsonSerializerOptions)null)
                          ).HasColumnName("DireccionFacturacion");
            builder.Entity<Pedido>().Property((Pedido ped) => ped.SubTotal).IsRequired();
            builder.Entity<Pedido>().Property((Pedido ped) => ped.GastosEnvio).IsRequired();
            builder.Entity<Pedido>().Property((Pedido ped) => ped.Total).IsRequired();
            builder.Entity<Pedido>().Property((Pedido ped) => ped.EstadoPedido).IsRequired().HasMaxLength(50);
            #endregion
            #region /// CREACION DE TABLA CATEGORIAS A PARTIR DE LA CLASE MODELO CATEGORIA
            //builder.Entity<Categoria>().ToTable("Categorias");
            builder.Entity<Categoria>().HasNoKey();
            #endregion
            #region /// CREACION DE TABLA PROVINCIAS A PARTIR DE LA CLASE MODELO PROVINCIA
            builder.Entity<Provincia>().HasNoKey();
            #endregion
            #region /// CREACION DE TABLA MUNICIPIOS A PARTIR DE LA CLASE MODELO MUNICIPIO
            builder.Entity<Municipio>().HasNoKey();
            #endregion
        }
        #endregion
    }
}