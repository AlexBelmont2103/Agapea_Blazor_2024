using Agapea_Blazor_2024.Client;
using Agapea_Blazor_2024.Client.Models.Services;
using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("Agapea_Blazor_2024.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Agapea_Blazor_2024.ServerAPI"));

//Definicion de inyeccion de dependencias para los servicios de la aplicacion
builder.Services.AddScoped<IRestService, MiRestService>();
builder.Services.AddScoped<IStorageService, SubjectStorage>();

await builder.Build().RunAsync();
