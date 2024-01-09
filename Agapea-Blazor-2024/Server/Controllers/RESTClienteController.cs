using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agapea_Blazor_2024.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RESTClienteController : ControllerBase
    {
        #region propiedades de la clase RESTClienteController

        #endregion

        #region metodos de la clase RESTClienteController
        [HttpPost]
        public async Task<RestMessage> RegistrarCliente(Cliente nuevoCliente)
        {

            //Delegamos a identity la creacion del usuario
            throw new NotImplementedException();
        }
        #endregion
    }
}
