﻿using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace Agapea_Blazor_2024.Client.Models.Services
{
    public class MiRestService : IRestService
    {
        #region propiedades del servicio
        private HttpClient _httpClient;
        #endregion
        public MiRestService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        #region metodos del servicio
        #region ///// llamada endpoints zona Cliente /////
        public async Task<RestMessage> LoginCliente(Cuenta credenciales)
        {
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cuenta>("api/RESTCliente/Login", credenciales);
            RestMessage _bodyResp = await _resp.Content.ReadFromJsonAsync<RestMessage>();
            return _bodyResp;
        }
        public async Task<RestMessage> RegistrarCliente(Cliente NuevoCliente)
        {
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cliente>("api/RESTCliente/Registro", NuevoCliente);
            RestMessage _bodyResp = await _resp.Content.ReadFromJsonAsync<RestMessage>();
            return _bodyResp;
        }
        public async Task<RestMessage> LoginCliente(string idcliente)
        {
            HttpResponseMessage _resp = await this._httpClient.GetAsync($"api/RESTCliente/Login?idcliente={idcliente}");
            return await _resp.Content.ReadFromJsonAsync<RestMessage>();
        }
        #endregion

        #region ///// llamada endpoints zona Tienda /////
        public async Task<List<Categoria>> RecuperarCategorias(string idcat)
        {
            if (String.IsNullOrEmpty(idcat)) idcat = "raices";
            return await this._httpClient
                            .GetFromJsonAsync<List<Categoria>>($"/api/RESTTienda/RecuperarCategorias?idcat={idcat}") ?? new List<Categoria>();
        }
        public async Task<List<Libro>> RecuperarLibros(string idcat)
        {
            return await this._httpClient
                             .GetFromJsonAsync<List<Libro>>($"/api/RESTTienda/RecuperarLibros?idcat={idcat}") ?? new List<Libro>();
        }
        public async Task<Libro> RecuperarLibro(string isbn13)
        {
            return await this._httpClient
                            .GetFromJsonAsync<Libro>($"/api/RESTTienda/RecuperarLibro?isbn13={isbn13}") ?? new Libro();
        }


        #endregion
        #region ///// llamada endpoints zona Pedido /////

        public async Task<List<Provincia>> RecuperarProvincias()
        {
            try
            {
                return await this._httpClient
                            .GetFromJsonAsync<List<Provincia>>($"/api/RESTTienda/RecuperarProvincias") ?? new List<Provincia>();
            }
            catch (Exception ex)
            {
                return new List<Provincia>();
            }

        }

        public async Task<List<Municipio>> RecuperarMunicipios(string cpro)
        {
            try
            {
                return await this._httpClient
                            .GetFromJsonAsync<List<Municipio>>($"/api/RESTTienda/RecuperarMunicipios?cpro={cpro}") ?? new List<Municipio>();
            }
            catch (Exception ex)
            {
                return new List<Municipio>();
            }

        }

        public async Task<String> FinalizarPedido(DatosPago datos, Pedido pedido)
        {
            Dictionary<string, string> _dic = new Dictionary<string, string>()
            {
                { "datosPago", JsonSerializer.Serialize(datos) },
                { "pedido", JsonSerializer.Serialize(pedido) }
            };
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Dictionary<string, string>>("/api/RESTTienda/FinalizarPedido", _dic);
            return await _resp.Content.ReadAsStringAsync();
        }

        #endregion
        #endregion
    }
}
