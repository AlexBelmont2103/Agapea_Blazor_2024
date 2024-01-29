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
        private List<ItemPedido> _datosItemsPedido = new();
        #endregion
        public SubjectStorage()
        {
            IDisposable _subscripItemsPedidoSubject = this._itemsPedidoSubject
                                                            .Subscribe<List<ItemPedido>>(
                                                            (List<ItemPedido> items) => this._datosItemsPedido = items
                                                        );

            IDisposable _subscripClienteSubject = this._clienteSubject
                                                    .Subscribe<Cliente>(
                                                        (Cliente datosObs) => this._datoscliente = datosObs
                                                        );

            IDisposable _jwtSubject = this._jwtSubject
                                        .Subscribe<String>(
                                            (String datosJWT) => this._datosJWT = datosJWT
                                        );
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
        public void OperarElementosPedido(Libro libro, string operacion)
        {
            try
            {
                //en funcion del valor del parametro "operacion" actualizo datos del observable del subject....
                //agregar <--- añadir nuevo ItemPedido a la lista de items, comprobando antes si no existe (si existe inc.la cantidad)
                //borrar <----borrar ItemPedido de la lista de items
                //restar  <---- modificar cantidad de ItemPedido
                switch (operacion)
                {
                    case "agregar":
                        AgregarElementoPedido(libro);
                        break;
                    case "borrar":
                        BorrarElementoPedido(libro);
                        break;

                    case "restar":
                        RestarElementoPedido(libro);
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en OperarElementos");
                Console.WriteLine(ex.Message);
            }

        }
        private void AgregarElementoPedido(Libro libro)
        {
            if (_datosItemsPedido == null)
            {
                Console.WriteLine("Lista de items del pedido vacia");
                _datosItemsPedido = new List<ItemPedido>();
            }
            try
            {
                //añadir <--- añadir nuevo ItemPedido a la lista de items, comprobando antes si no existe (si existe inc.la cantidad)
                int _posItem = this._datosItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == libro.ISBN13);
                if (_posItem != -1)
                {
                    //el libro existe, incremento cantidad...
                    this._datosItemsPedido[_posItem].CantidadItem += 1;
                }
                else
                {
                    //libro no existe en lista de items del pedido, añado nuevo itempedido 
                    this._datosItemsPedido.Add(new ItemPedido { LibroItem = libro, CantidadItem = 1 });
                }
                Console.WriteLine("Operacion realizada con exito");

                //..actualizamos valor del observable del subject...
                this._itemsPedidoSubject.OnNext(this._datosItemsPedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en AgregarElementoPedido");
                Console.WriteLine(ex.Message);
            }
        }
        private void BorrarElementoPedido(Libro libro)
        {
            try
            {
                //borrar <----borrar ItemPedido de la lista de items
                int _posItem = this._datosItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == libro.ISBN13);
                if (_posItem != -1) this._datosItemsPedido.RemoveAt(_posItem);
                //..actualizamos valor del observable del subject...
                this._itemsPedidoSubject.OnNext(this._datosItemsPedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en BorrarElementoPedido");
                Console.WriteLine(ex.Message);
            }
        }
        private void RestarElementoPedido(Libro libro)
        {
            try
            {
                int _posItem = this._datosItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == libro.ISBN13);
                //Si el libro existe en la lista de items del pedido, decremento cantidad...
                //Si la cantidad resultante es 0, elimino el item de la lista de items del pedido
                if (_posItem != -1)
                {
                    this._datosItemsPedido[_posItem].CantidadItem -= 1;
                    if (this._datosItemsPedido[_posItem].CantidadItem == 0) this._datosItemsPedido.RemoveAt(_posItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RestarElementoPedido");
                Console.WriteLine(ex.Message);
            }


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
