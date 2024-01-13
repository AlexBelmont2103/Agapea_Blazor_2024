using Agapea_Blazor_2024.Server.Models;
using Agapea_Blazor_2024.Server.Models.Services;
using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Agapea_Blazor_2024.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RESTClienteController : ControllerBase
    {
        #region propiedades de la clase RESTClienteController
        private UserManager<MiClienteIdentity> _userManagerService;
        private SignInManager<MiClienteIdentity> _signInManagerService;
        private IClienteCorreo _servicioCorreo;
        #endregion
        public RESTClienteController(UserManager<MiClienteIdentity> userManagerDI, SignInManager<MiClienteIdentity> signInManagerDI, IClienteCorreo clienteEmailServiceDI)
        {
            this._userManagerService = userManagerDI;
            this._signInManagerService = signInManagerDI;
            this._servicioCorreo = clienteEmailServiceDI;
        }
        #region metodos de la clase RESTClienteController
        [HttpPost]
        [Route("Registro")]
        public async Task<RestMessage> RegistrarCliente(Cliente nuevoCliente)
        {
            try
            {
                //1º: Usando el servicio UserManager de Identity, crear un nuevo cliente en la tabla AspNetUsers
                MiClienteIdentity _clienteACrear = new MiClienteIdentity()
                {
                    UserName = nuevoCliente.Credenciales.Login,
                    Email = nuevoCliente.Credenciales.Email,
                    Nombre = nuevoCliente.Nombre,
                    Apellidos = nuevoCliente.Apellidos,
                    FechaNacimiento = DateTime.Now,
                    Genero = nuevoCliente.Genero ?? "",
                    Descripcion = nuevoCliente.Descripcion ?? "",
                    ImagenAvatarBASE64 = nuevoCliente.Credenciales.ImagenCuentaBASE64,
                    PhoneNumber = nuevoCliente.Telefono,

                };
                IdentityResult __resultRegistro = await this._userManagerService.CreateAsync(_clienteACrear, nuevoCliente.Credenciales.Password);

                if (__resultRegistro.Succeeded)
                {
                    //2º: Usando el servicio UserManager de Identity, generar un token de activacion de cuenta y enviarlo por email al cliente
                    //La url de activacion de cuente debe ser una url que invoque a un metodo de este controlador que confirme el token de activacion
                    String _tokenActivacionEmail = await this._userManagerService.GenerateEmailConfirmationTokenAsync(_clienteACrear);
                    String _urlMail = $"https://localhost:7286/api/RESTCliente/ActivarCuenta?token={HttpUtility.UrlEncode(_tokenActivacionEmail)}&idcliente={HttpUtility.UrlEncode(_clienteACrear.Id)}";
                    String mensaje = $@"<h1>Activacion de cuenta de cliente</h1>
                                    <p>Para activar su cuenta de cliente en la tienda online Agapea, haga click en el siguiente enlace:</p>
                                    <a href='{_urlMail}'>Activar cuenta</a>";
                    _servicioCorreo.EnviarEmail(_clienteACrear.Email, "Activacion de cuenta de cliente", mensaje, null);

                    return new RestMessage()
                    {
                        Codigo = 0,
                        Mensaje = "Se ha enviado un email al cliente",
                        Error = "",
                        Tokensesion = null,
                        DatosCliente = null,
                        OtrosDatos = null

                    };
                }
                else
                {
                    throw new Exception("Error al registrar el cliente: " + __resultRegistro.Errors.FirstOrDefault().Description);
                }
            }catch(Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al registrar el cliente: " + ex.Message,
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null

                };
            }
            
        }

        [HttpGet]
        [Route("ActivarCuenta")]
        public async Task ActivarCuenta([FromQuery] String token, [FromQuery] String idcliente)
        {
            try
            {
                //3º: Usando el servicio UserManager de Identity, confirmar el token de activacion y activar la cuenta si el token es correcto
                //Tengo que recuperar los datos del cliente identity asociados al idcliente
                
                MiClienteIdentity _clienteAActivar = await this._userManagerService.FindByIdAsync(idcliente);

                IdentityResult _resultadoCompToken = await this._userManagerService.ConfirmEmailAsync(_clienteAActivar, token);
                if (!_resultadoCompToken.Succeeded)
                {
                    throw new Exception("Error al activar la cuenta de cliente: " + _resultadoCompToken.Errors.FirstOrDefault().Description);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error al activar la cuenta de cliente: " + ex.Message);
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<RestMessage> LoginCliente(Cuenta credenciales)
        {
            try
            {
                //1º: Usando el servicio SignInManager de Identity, comprobar si las credenciales de login son correctas
                MiClienteIdentity _clienteALoguear = await this._userManagerService.FindByEmailAsync(credenciales.Email);
                if (_clienteALoguear == null)
                {
                    throw new Exception("Error al hacer login: El cliente no existe");
                }
                SignInResult _resultadoLogin = await this._signInManagerService.PasswordSignInAsync(_clienteALoguear, credenciales.Password, false, false);
                if (!_resultadoLogin.Succeeded)
                {
                    throw new Exception("Error al hacer login: " + _resultadoLogin.ToString());
                }
                //2º: Si las credenciales son correctas, generar un token de sesion y devolverlo al cliente
                String _tokenSesion = await this._userManagerService.GenerateUserTokenAsync(_clienteALoguear, "Default", "tokenSesion");
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Login correcto",
                    Error = "",
                    Tokensesion = _tokenSesion,
                    DatosCliente = new Cliente()
                    {
                        Nombre = _clienteALoguear.Nombre,
                        Apellidos = _clienteALoguear.Apellidos,
                        Genero = _clienteALoguear.Genero,
                        Descripcion = _clienteALoguear.Descripcion,
                        Telefono = _clienteALoguear.PhoneNumber,
                        Credenciales = new Cuenta()
                        {
                            Login = _clienteALoguear.UserName,
                            Password = credenciales.Password,
                            Email = _clienteALoguear.Email,
                            ImagenCuentaBASE64 = _clienteALoguear.ImagenAvatarBASE64,
                            CuentaActiva = _clienteALoguear.EmailConfirmed
                        }
                    },
                    OtrosDatos = null
                };
            }
            catch(Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al hacer login: " + ex.Message,
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }

        //Si quiero restringir el acceso a un metodo de un controlador a usuarios autenticados, debo decorar el metodo con el atributo [Authorize]

        #endregion
    }
}
