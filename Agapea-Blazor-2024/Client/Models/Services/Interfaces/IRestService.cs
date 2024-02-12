using Agapea_Blazor_2024.Shared;

namespace Agapea_Blazor_2024.Client.Models.Services.Interfaces
{
    public interface IRestService
    {
        //Interfaz que sirve como patron para definir los servicios a inyectar en Program.cs
        //para hacer pet ajax a servicios externos
        #region metodos/props para zonaCliente
        Task<RestMessage> RegistrarCliente(Cliente NuevoCliente);
        Task<RestMessage> LoginCliente(Cuenta credenciales);
        Task<RestMessage> LoginCliente(String idcliente);
        Task<RestMessage> OperarDireccion(Direccion direccion, String operacion);
        Task<RestMessage> UploadImagen(String imagenbase64, String idcliente);
        Task<RestMessage> UpdateCliente(Cliente datoscliente);
        Task<String> RecuperarNombreLogin(String idcliente);
        Task<RestMessage> UploadOpinion(Opinion opinion);
        Task<RestMessage> OperarOpinion(KeyValuePair<String, Opinion> datos);
        Task<RestMessage> CrearLista(ListaLibros lista);
        Task<RestMessage> UpdateListasCliente(List<ListaLibros> listas);
        #endregion

        #region metodos/props para zonaTienda
        Task<List<Libro>> RecuperarLibros(String idcat);
        Task<Libro> RecuperarLibro(String isbn13);
        Task<List<Categoria>> RecuperarCategorias(String idcat);
        Task<List<Opinion>> RecuperarOpiniones(String isbn13);
        Task<List<Libro>> RecuperarLibrosBusqueda(String busqueda);
        #endregion
        #region metodos para zonaPedido
        Task<List<Provincia>> RecuperarProvincias();
        Task<List<Municipio>> RecuperarMunicipios(String cpro);
        Task<String> FinalizarPedido(DatosPago datos, Pedido pedido);
        #endregion
    }
}
