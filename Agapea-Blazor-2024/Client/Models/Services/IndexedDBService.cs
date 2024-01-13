using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;
using Microsoft.JSInterop;

namespace Agapea_Blazor_2024.Client.Models.Services
{
    public class IndexedDBService : IStorageService
    {
        #region propiedades de la clase IndexedDBService
        private IJSRuntime _jsRuntime;
        #endregion
        public IndexedDBService(IJSRuntime jsServiceDI)
        {
            this._jsRuntime = jsServiceDI;
        }
        #region metodos de la clase IndexedDBService

        #region metodos SINCRONOS (Sin uso en este caso)
        public void AlmacenamientoDatosCliente(Cliente datoscliente)
        {
            throw new NotImplementedException();
        }
        public void AlmacenamientoJWT(string tokenJWT)
        {
            throw new NotImplementedException();
        }
        public Cliente RecuperarDatosCliente()
        {
            throw new NotImplementedException();
        }
        public string RecuperarJWT()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region metodos ASINCRONOS
        public async Task AlmacenamientoDatosClienteAsync(Cliente datoscliente)
        {
            await this._jsRuntime.InvokeVoidAsync("adminIndexedDB.almacenarValor", "datoscliente", datoscliente);
        }
        public async Task AlmacenamientoJWTAsync(string tokenJWT)
        {
            await this._jsRuntime.InvokeVoidAsync("adminIndexedDB.almacenarValor", "tokensesion", tokenJWT);
        }
        public async Task<Cliente> RecuperarDatosClienteAsync()
        {
            return await this._jsRuntime.InvokeAsync<Cliente>("adminIndexedDB.recuperarValor", "datoscliente");
        }
        public async Task<string> RecuperarJWTAsync()
        {
            return await this._jsRuntime.InvokeAsync<string>("adminIndexedDB.recuperarValor", "tokensesion");
        }
        #endregion

        #endregion



    }
}
