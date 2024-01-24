﻿using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;
using System.Net.Http.Json;

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
        public async Task<RestMessage> LoginCliente(Cuenta credenciales)
        {
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cuenta>("api/RESTCliente/Login", credenciales);
            RestMessage _bodyResp = await _resp.Content.ReadFromJsonAsync<RestMessage>();
            return _bodyResp;
        }

        public async Task<List<Categoria>> RecuperarCategorias(string idcat)
        {
            return await this._httpClient.GetFromJsonAsync<List<Categoria>>($"api/RESTTienda/RecuperarCategorias?idcat={idcat}") ?? new List<Categoria>();
        }

        public Task<Libro> RecuperarLibro(string isbn13)
        {
            throw new NotImplementedException();
        }

        public Task<List<Libro>> RecuperarLibros(string idcat)
        {
            throw new NotImplementedException();
        }

        public async Task<RestMessage> RegistrarCliente(Cliente NuevoCliente)
        {
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cliente>("api/RESTCliente/Registro", NuevoCliente);
            RestMessage _bodyResp = await _resp.Content.ReadFromJsonAsync<RestMessage>();
            return _bodyResp;
        }
        #endregion
    }
}
