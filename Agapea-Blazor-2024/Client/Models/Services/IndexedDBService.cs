using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;
using Microsoft.JSInterop;

namespace Agapea_Blazor_2024.Client.Models.Services
{
    public class IndexedDBService : IStorageService
    {
        #region propiedades de la clase IndexedDBService
        private IJSRuntime _jsRuntime;
        private DotNetObjectReference<IndexedDBService> _refIndexedService;
        //Propiedad de tipo evento que vamos a usar para notificar a aquellos componentes que usen este servicio
        //que se han recuperado datos de IndexedDB (en este caso, el cliente) y ya están disponibles
        public event EventHandler<Cliente> ClienteRecupIndexedDBEvent;
        #endregion
        public IndexedDBService(IJSRuntime jsServiceDI)
        {
            this._jsRuntime = jsServiceDI;
            this._refIndexedService = DotNetObjectReference.Create(this);
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
        public void OperarElementosPedido(Libro libro, int cantidad, string operacion)
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
        public Task OperarElementosPedidoAsync(Libro libro, int cantidad, string operacion)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region metodos ASINCRONOS
        public async Task AlmacenamientoDatosClienteAsync(Cliente datoscliente)
        {
            await this._jsRuntime.InvokeVoidAsync("adminIndexedDB.almacenarDatosCliente", datoscliente);
        }
        public async Task AlmacenamientoJWTAsync(string tokenJWT)
        {
            await this._jsRuntime.InvokeVoidAsync("adminIndexedDB.almacenarTokenCliente", tokenJWT);
        }
        public async Task<Cliente> RecuperarDatosClienteAsync()
        {
            //A la funcion de js le paso la ref del servicio para que
            //cuando acabe su ejecucion, llame a un metodo de este servicio
            return await this._jsRuntime.InvokeAsync<Cliente>("adminIndexedDB.recuperarDatosCliente", this._refIndexedService);
        }

        //Metodo invocable desde js (ManageIndexedDB.js)
        [JSInvokable("CallbackServIndexedDBblazor")]
        public void CallFromJS(Cliente clienteIndexedDB)
        {
            //Cuando recibo los datos del cliente, notifico a los componentes que usen este servicio
            //que ya están disponibles los datos del cliente
            this.ClienteRecupIndexedDBEvent.Invoke(this, clienteIndexedDB);
        }
        [JSInvokable("CallbackServIndexedDBblazorJWT")]
        public void CallFromJS2(String jwt)
        {

        }
        public async Task<string> RecuperarJWTAsync()
        {
            return await this._jsRuntime.InvokeAsync<string>("adminIndexedDB.recuperarTokenCliente", this._refIndexedService);
        }

        public List<ItemPedido> RecuperarElementosPedido()
        {
            throw new NotImplementedException();
        }
        public Task<List<ItemPedido>> RecuperarElementosPedidoAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion



    }
}
