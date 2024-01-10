using Agapea_Blazor_2024.Server.Models;
using Agapea_Blazor_2024.Server.Models.Services;
using agapea_netcore_mvc_23_24.Models.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

/*  --- CONFIGURACION DE IDENTITY CON ENTITY FRAMEWORK ---
 
        El conjunto de tablas sql server que se crean a traves de EF y que usa identity se mapean contra objetos DbSet 
        Todos los DbSets forman un DbContext. Es el que define las propiedades de cada DbSet (mapea props de cada clase con columnas de un DbSet
        establece claces, índices, relaciones, etc)
 */
String _cadenaConexionBD = builder.Configuration.GetConnectionString("BlazorSqlServerConnectionString");
String _nombreEnsablado = Assembly.GetExecutingAssembly().GetName().Name;
//1º: Configurar cadena de conexion que va a usar el DbContext para volcar cambios  en migraciones y recuperar datos
builder.Services.AddDbContext<AplicacionDBContext>((DbContextOptionsBuilder opciones) => 
{
    opciones.UseSqlServer(_cadenaConexionBD, (SqlServerDbContextOptionsBuilder opc)=> opc.MigrationsAssembly(_nombreEnsablado));
} );

//2º: Configurar servicio de Identity: UserManager y SignInManager
builder.Services.AddIdentity<MiClienteIdentity, IdentityRole>(
    (IdentityOptions opciones) => 
    {
        //Opciones de configuracion UserManager
        opciones.Password = new PasswordOptions()
        {
            RequireDigit = true,
            RequiredLength = 6,
            RequireUppercase = true,
            RequireLowercase = false,
            RequireNonAlphanumeric = false,
        };
        opciones.User = new UserOptions()
        {
            RequireUniqueEmail = true,
            AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
        };
        //Opciones de configuracion SignInManager
        opciones.SignIn = new SignInOptions()
        {
            RequireConfirmedEmail = true
        };
        opciones.Lockout = new LockoutOptions()
        {
            AllowedForNewUsers = false
        };
    }
    )
    .AddEntityFrameworkStores<AplicacionDBContext>(); // <== Saltará excepcion a la hora de validar emails cuando Identity genera token de activacion
                                                      // porque no existe un proveedor de tokens por defecto: AddTokenProvider()

//Inyeccion  servicio de envio de emails
builder.Services.AddScoped<IClienteCorreo, MailjetService>();
    

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
