var builder = WebApplication.CreateBuilder(args);

/*  --- CONFIGURACION DE IDENTITY CON ENTITY FRAMEWORK ---
 
        El conjunto de tablas sql server que se crean a traves de EF y que usa identity se mapean contra objetos DbSet 
        Todos los DbSets forman un DbContext. Es el que define las propiedades de cada DbSet (mapea props de cada clase con columnas de un DbSet
        establece claces, índices, relaciones, etc)
 */
String _cadenaConexionBD = builder.Configuration.GetConnectionString("BlazorSqlServerConnectionString");

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
