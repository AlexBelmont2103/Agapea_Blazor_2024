using Agapea_Blazor_2024.Shared;

namespace Agapea_Blazor_2024.Client.Models.Services.Interfaces
{
    public interface IStorageService
    {
        #region propiedades de la clase IStorageService
        //Propiedad de tipo evento que vamos a usar para notificar a aquellos componentes que usen este servicio
        //que se han recuperado datos de IndexedDB (en este caso, el cliente) y ya están disponibles
        public event EventHandler<Cliente> ClienteRecupIndexedDBEvent;
        #endregion
        #region metodos SÍNCRONOS de almacenamiento de valores en servicios storage (OBSERVABLES)
        void AlmacenamientoDatosCliente(Cliente datoscliente);
        void AlmacenamientoJWT(String tokenJWT);
        Cliente RecuperarDatosCliente();
        String RecuperarJWT();
        #endregion

        #region metodos ASÍNCRONOS de almacenamiento de valores en servicios storage (LOCALSTORAGE, INDEXEDDB, SESSIONSTORAGE)
        Task AlmacenamientoDatosClienteAsync(Cliente datoscliente);
        Task AlmacenamientoJWTAsync (String tokenJWT);
        Task<Cliente> RecuperarDatosClienteAsync();
        Task<String> RecuperarJWTAsync();

        #endregion
    }
}
