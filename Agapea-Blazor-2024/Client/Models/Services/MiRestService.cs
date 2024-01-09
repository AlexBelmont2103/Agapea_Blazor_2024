using Agapea_Blazor_2024.Client.Models.Services.Interfaces;
using Agapea_Blazor_2024.Shared;

namespace Agapea_Blazor_2024.Client.Models.Services
{
    public class MiRestService : IRestService
    {
        #region propiedades del servicio

        #endregion

        #region metodos del servicio
        public Task<RestMessage> LoginCliente(Cuenta credenciales)
        {
            throw new NotImplementedException();
        }

        public Task<RestMessage> RegistrarCliente(Cliente NuevoCliente)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
