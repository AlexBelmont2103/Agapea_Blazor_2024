﻿using Agapea_Blazor_2024.Shared;

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
        #endregion

        #region metodos/props para zonaTienda
        Task<List<Libro>> RecuperarLibros(String idcat);
        Task<Libro> RecuperarLibro(String isbn13);
        Task<List<Categoria>> RecuperarCategorias(String idcat);
        #endregion
        #region metodos para zonaPedido
        Task<List<Provincia>> RecuperarProvincias();
        Task<List<Municipio>> RecuperarMunicipios(String cpro);
        Task<String> FinalizarPedido(DatosPago datos, Pedido pedido);
        #endregion
    }
}
