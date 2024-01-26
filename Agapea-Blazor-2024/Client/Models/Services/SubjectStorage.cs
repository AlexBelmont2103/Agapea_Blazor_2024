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
                    BehaviorSubject<List<ItemPedido>>
                    ----------------------------
                     elementos del pedido
                    ----------------------------
                    |                          |
        RecuperarElementosPedido()     OperarElementosPedido(elemento, cantidad, operacion)
         
         */
        #region propiedades del servicio
        public event EventHandler<Cliente> ClienteRecupIndexedDBEvent;
        private BehaviorSubject<Cliente> _clienteSubject = new BehaviorSubject<Cliente>(null);
        private BehaviorSubject<String> _jwtSubject = new BehaviorSubject<string>("");
        private BehaviorSubject<List<ItemPedido>> _itemsPedidoSubject = new BehaviorSubject<List<ItemPedido>>(null);
        private Cliente _datoscliente = new Cliente(); //Variable privada para almacenar los datos del subject Cliente
        private String _datosJWT = ""; //Variable privada para almacenar datos del subject String
        private List<ItemPedido> _datosItemsPedido = new List<ItemPedido>();
        #endregion
        public SubjectStorage()
        {
            IDisposable _subscriptionItemsPedidoSubject = this._itemsPedidoSubject.Subscribe<List<ItemPedido>>((List<ItemPedido> datosItemsPedido) => this._datosItemsPedido = datosItemsPedido);
            IDisposable _subscriptionClienteSubject = this._clienteSubject.Subscribe<Cliente>((Cliente datosObs) => this._datoscliente = datosObs);
            IDisposable _subscriptionJWTSubject = this._jwtSubject.Subscribe<String>((String datosJWT) => this._datosJWT = datosJWT);
        }
        #region metodos servicio SubjectStorage
        #region metodos SINCRONOS
        public void AlmacenamientoDatosCliente(Cliente datoscliente)
        {
            this._clienteSubject.OnNext(datoscliente); //Actualizo los datos del subject
            this.ClienteRecupIndexedDBEvent.Invoke(this, datoscliente); //Disparo el evento por si alguien lo esta escuchando
        }
        public void AlmacenamientoJWT(string tokenJWT)
        {
            this._jwtSubject.OnNext(tokenJWT); //Actualizo los datos del subject
        }
        public void OperarElementosPedido(Libro libro, int cantidad, string operacion)
        {
            //En funcion del valor de operacion, añado, quito, o elimino el elemento del pedido
            //Si vale agregar, añado el elemento al pedido (si ya existe, aumento la cantidad)
            //Si vale quitar, resto la cantidad del elemento del pedido (si la cantidad es 0, elimino el elemento del pedido)
            //Si vale eliminar, elimino el elemento del pedido
            //Una vez realizada la operacion, actualizo los datos del subject

            if (operacion == "agregar")
            {
                this.AgregarItemPedido(libro, cantidad);
            }
            else if (operacion == "quitar")
            {
                this.RestarItemPedido(libro, cantidad);
            }
            else if (operacion == "eliminar")
            {
                this.EliminarItemPedido(libro);
            }
        }

        private void AgregarItemPedido(Libro libro, int cantidad)
        {
            //Busco si el libro ya existe en el pedido
            ItemPedido itemPedido = this._datosItemsPedido.Find(item => item.LibroItem.ISBN13 == libro.ISBN13);
            //Si el libro existe, aumento la cantidad
            if (itemPedido != null)
            {
                itemPedido.CantidadItem += cantidad;
            }
            else
            {
                //Si el libro no existe, lo añado al pedido
                itemPedido = new ItemPedido();
                itemPedido.LibroItem = libro;
                itemPedido.CantidadItem = cantidad;
                this._datosItemsPedido.Add(itemPedido);
            }
            //Actualizo los datos del subject
            this._itemsPedidoSubject.OnNext(this._datosItemsPedido);
        }
        private void RestarItemPedido(Libro libro, int cantidad)
        {
            //Busco si el libro ya existe en el pedido
            ItemPedido itemPedido = this._datosItemsPedido.Find(item => item.LibroItem.ISBN13 == libro.ISBN13);
            //Si el libro existe, compruebo si la cantidad a restar es mayor que la cantidad del pedido
            if (itemPedido != null)
            {
                if (itemPedido.CantidadItem > cantidad)
                {
                    itemPedido.CantidadItem -= cantidad;
                }
                else
                {
                    //Si la cantidad a restar es mayor que la cantidad del pedido, elimino el elemento del pedido
                    this._datosItemsPedido.Remove(itemPedido);
                }
            }
            //Actualizo los datos del subject
            this._itemsPedidoSubject.OnNext(this._datosItemsPedido);
        }
        private void EliminarItemPedido(Libro libro)
        {
            //Busco si el libro ya existe en el pedido y lo elimino
            ItemPedido itemPedido = this._datosItemsPedido.Find(item => item.LibroItem.ISBN13 == libro.ISBN13);
            if (itemPedido != null)
            {
                this._datosItemsPedido.Remove(itemPedido);
            }
            //Actualizo los datos del subject
            this._itemsPedidoSubject.OnNext(this._datosItemsPedido);
        }
        public Cliente RecuperarDatosCliente()
        {
            return this._datoscliente;
        }
        public string RecuperarJWT()
        {
            return this._datosJWT;
        }
        public List<ItemPedido> RecuperarElementosPedido()
        {
            return this._datosItemsPedido;
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
        public Task OperarElementosPedidoAsync(Libro libro, int cantidad, string operacion)
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
