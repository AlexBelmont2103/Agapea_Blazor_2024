using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;
using System.Reactive.Subjects;

namespace Agapea_Blazor_2024.Client.Models.Services
{
    public class SubjectStorage : IStorageService
    {

        /*
            servicio:
                    BehaviorSubject<Cliente> 
                    ----------------------------
                     datos_cliente
                    ----------------------------
                    |                          |
        RecuperarDatosCliente()     AlmacenamientoDatosCliente(Cliente datoscliente)
                    BehaviorSubject<string>
                    ----------------------------
                     tokenJWT
                    ----------------------------
         
         */
        #region propiedades del servicio
        public event EventHandler<Cliente> ClienteRecupIndexedDBEvent;
        private BehaviorSubject<Cliente> _clienteSubject= new BehaviorSubject<Cliente>(null);
        private BehaviorSubject<String> _jwtSubject= new BehaviorSubject<string>("");
        private Cliente _datoscliente = new Cliente(); //Variable privada para almacenar los datos del subject Cliente
        private String _datosJWT = ""; //Variable privada para almacenar datos del subject String
        #endregion
        public SubjectStorage()
        {
            IDisposable _subcrptionClienteSubject = this._clienteSubject.Subscribe<Cliente>((Cliente datosObs) => this._datoscliente = datosObs);
            IDisposable _subcrptionJWTSubject = this._jwtSubject.Subscribe<String>((String datosJWT) => this._datosJWT = datosJWT);
        }
        #region metodos servicio SubjectStorage
        #region metodos SINCRONOS
        public void AlmacenamientoDatosCliente(Cliente datoscliente)
        {
            this._clienteSubject.OnNext(datoscliente); //Actualizo los datos del subject
            this.ClienteRecupIndexedDBEvent.Invoke(this,datoscliente); //Disparo el evento por si alguien lo esta escuchando
        }
        public void AlmacenamientoJWT(string tokenJWT)
        {
            this._jwtSubject.OnNext(tokenJWT);
        }
        public Cliente RecuperarDatosCliente()
        {
            return this._datoscliente;
        }
        public string RecuperarJWT()
        {
            return this._datosJWT;
        }
        #endregion

        #region metodos ASINCRONOS (no usados en este caso)

        public Task AlmacenamientoDatosClienteAsync(Cliente datoscliente)
        {
            throw new NotImplementedException();
        }



        public Task AlmacenamientoJWTAsync(string tokenJWT)
        {
            throw new NotImplementedException();
        }



        public Task<Cliente> RecuperarDatosClienteAsync()
        {
            throw new NotImplementedException();
        }



        public Task<string> RecuperarJWTAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
