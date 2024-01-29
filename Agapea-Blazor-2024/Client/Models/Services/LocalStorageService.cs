using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;
using Microsoft.JSInterop;

namespace Agapea_Blazor_2024.Client.Models.Services
{
    public class LocalStorageService : IStorageService
    {
        #region propiedades del servicio
        private IJSRuntime _jsRuntime;
        public event EventHandler<Cliente> ClienteRecupIndexedDBEvent;
        #endregion
        public LocalStorageService(IJSRuntime jsServiceDI)
        {
            this._jsRuntime = jsServiceDI;
        }
        #region metodos servicio LocalStorage
        #region metodos SINCRONOS (Sin uso en este caso)
        public void AlmacenamientoDatosCliente(Cliente datoscliente)
        {
            throw new NotImplementedException();
        }
        public void AlmacenamientoJWT(string tokenJWT)
        {
            throw new NotImplementedException();
        }
        public Task OperarElementosPedidoAsync(Libro libro, string operacion)
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
        public List<ItemPedido> RecuperarElementosPedido()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region metodos ASINCRONOS
        public async Task AlmacenamientoDatosClienteAsync(Cliente datoscliente)
        {
            //Tendria que ejecutar esto desde Javascript: localStorage.setItem("Cliente", JSON.stringuify(datoscliente));
            await this._jsRuntime.InvokeVoidAsync("adminLocalStorage.almacenarValor", "datoscliente", datoscliente);
        }
        public async Task AlmacenamientoJWTAsync(string tokenJWT)
        {
            //Tendria que ejecutar esto desde Javascript: localStorage.setItem("JWT", tokenJWT);
            await this._jsRuntime.InvokeVoidAsync("adminLocalStorage.almacenarValor", "tokensesion", tokenJWT);
        }
        public async Task<Cliente> RecuperarDatosClienteAsync()
        {
            return await this._jsRuntime.InvokeAsync<Cliente>("adminLocalStorage.recuperarValor", "datoscliente");
        }
        public async Task<string> RecuperarJWTAsync()
        {
            return await this._jsRuntime.InvokeAsync<string>("adminLocalStorage.recuperarValor", "tokensesion");
        }
        public void OperarElementosPedido(Libro libro, int cantidad, string operacion)
        {
            throw new NotImplementedException();
        }
        public Task<List<ItemPedido>> RecuperarElementosPedidoAsync()
        {
            throw new NotImplementedException();
        }

        public void OperarElementosPedido(Libro libro, string operacion)
        {
            throw new NotImplementedException();
        }

        public Task OperarElementosPedidoAsync(Libro libro, int cantidad, string operacion)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
