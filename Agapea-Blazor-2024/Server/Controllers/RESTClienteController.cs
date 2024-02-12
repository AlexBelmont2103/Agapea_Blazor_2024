using Agapea_Blazor_2024.Server.Models;
using Agapea_Blazor_2024.Server.Models.Services;
using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
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
        private IConfiguration _accesoAppSettings;
        private AplicacionDBContext _dbcontext;
        #endregion
        public RESTClienteController(UserManager<MiClienteIdentity> userManagerDI,
            SignInManager<MiClienteIdentity> signInManagerDI,
            IClienteCorreo clienteEmailServiceDI,
            IConfiguration accesoAppSettingsDI,
            AplicacionDBContext dbcontextDI)
        {
            this._userManagerService = userManagerDI;
            this._signInManagerService = signInManagerDI;
            this._servicioCorreo = clienteEmailServiceDI;
            this._accesoAppSettings = accesoAppSettingsDI;
            this._dbcontext = dbcontextDI;
        }
        #region metodos de la clase RESTClienteController
        #region ///Registro y Login///
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
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw new Exception("Error al activar la cuenta de cliente: " + ex.Message);
            }

        }
        [HttpGet]
        [Route("Login")]
        public async Task<RestMessage> LoginCliente(string idcliente)
        {
            try
            {
                //Seleccionamos el cliente de la base de datos usado su id
                MiClienteIdentity _clienteALoguear = await this._userManagerService.FindByIdAsync(idcliente);
                //Creamos un cliente con los datos del cliente seleccionado
                Cliente _cliente = new Cliente()
                {
                    IdCliente = _clienteALoguear.Id,
                    Nombre = _clienteALoguear.Nombre,
                    Apellidos = _clienteALoguear.Apellidos,
                    Genero = _clienteALoguear.Genero,
                    Descripcion = _clienteALoguear.Descripcion,
                    Telefono = _clienteALoguear.PhoneNumber,
                    Credenciales = new Cuenta()
                    {
                        Login = _clienteALoguear.UserName,
                        Password = "",
                        Email = _clienteALoguear.Email,
                        ImagenCuentaBASE64 = _clienteALoguear.ImagenAvatarBASE64,
                        CuentaActiva = _clienteALoguear.EmailConfirmed
                    }
                };
                //Recuperamos también direcciones, pedidos, y opiniones del cliente
                _cliente.DireccionesCliente = this._dbcontext.Direcciones.Where(d => d.IdCliente == idcliente).ToList();
                _cliente.PedidosCliente = this._dbcontext.Pedidos.Where(p => p.IdCliente == idcliente).ToList();
                _cliente.OpinionesCliente = this._dbcontext.Opiniones.Where(o => o.IdCliente == idcliente).ToList();
                _cliente.ListasCliente = this._dbcontext.ListasLibros.Where(l => l.IdCliente == idcliente).ToList();
                //Generamos un token de sesion para el cliente
                String _tokenSesion = this.__GeneraJWT(_cliente.Nombre, _cliente.Apellidos, _cliente.Credenciales.Email, _cliente.IdCliente);
                if (_clienteALoguear == null)
                {
                    throw new Exception("Error al hacer login: El cliente no existe");
                }
                else
                {
                    return new RestMessage()
                    {
                        Codigo = 0,
                        Mensaje = "Login correcto",
                        Error = "",
                        Tokensesion = _tokenSesion,
                        DatosCliente = _cliente,
                        OtrosDatos = null
                    };
                }
            }
            catch (Exception ex)
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
                Cliente _clienteADevolver = new Cliente()
                {
                    IdCliente = _clienteALoguear.Id,
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
                };
                //Seleccionamos direcciones pedidos y opiniones del cliente
                _clienteADevolver.DireccionesCliente = this._dbcontext.Direcciones.Where(d => d.IdCliente == _clienteALoguear.Id).ToList();
                _clienteADevolver.OpinionesCliente = this._dbcontext.Opiniones.Where(o => o.IdCliente == _clienteALoguear.Id).ToList();
                _clienteADevolver.PedidosCliente = this._dbcontext.Pedidos.Where(p => p.IdCliente == _clienteALoguear.Id).ToList();
                _clienteADevolver.ListasCliente = this._dbcontext.ListasLibros.Where(l => l.IdCliente == _clienteALoguear.Id).ToList();
                //2º: Si las credenciales son correctas, generar un token de sesion y devolverlo al cliente
                String _tokenSesion = this.__GeneraJWT(_clienteADevolver.Nombre, _clienteADevolver.Apellidos, _clienteADevolver.Credenciales.Email, _clienteADevolver.IdCliente);
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Login correcto",
                    Error = "",
                    Tokensesion = _tokenSesion,
                    DatosCliente = _clienteADevolver,
                    OtrosDatos = null
                };
            }
            catch (Exception ex)
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
        #endregion
        #region ///Gestion de datos de cliente///
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UploadImagen")]
        public async Task<RestMessage> UploadImagen([FromBody] Dictionary<string, string> datos)
        {
            try
            {
                //Recupero los datos del dictionary
                String _imagenBASE64 = datos["imagenbase64"];
                String _idcliente = datos["idcliente"];
                MiClienteIdentity _cliente = await this._userManagerService.FindByIdAsync(_idcliente);
                _cliente.ImagenAvatarBASE64 = _imagenBASE64;
                IdentityResult _resultModif = await this._userManagerService.UpdateAsync(_cliente);
                if (_resultModif.Succeeded)
                {
                    return new RestMessage()
                    {
                        Codigo = 0,
                        Mensaje = "Imagen de cliente subida OK",
                        Error = "",
                        Tokensesion = this.__GeneraJWT(_cliente.Nombre, _cliente.Apellidos, _cliente.Email, _cliente.Id),
                        DatosCliente = await this.__GenerarClienteActualizado(_idcliente),
                        OtrosDatos = null
                    };
                }
                else
                {
                    throw new Exception("Error al subir imagen de cliente: " + _resultModif.Errors.FirstOrDefault().Description);
                }
            }
            catch (Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al subir imagen de cliente",
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateCliente")]
        public async Task<RestMessage> UpdateCliente(Cliente datoscliente)
        {
            MiClienteIdentity miClienteIdentity = await this._userManagerService.FindByIdAsync(datoscliente.IdCliente);
            miClienteIdentity.Nombre = datoscliente.Nombre;
            miClienteIdentity.Apellidos = datoscliente.Apellidos;
            miClienteIdentity.PhoneNumber = datoscliente.Telefono;
            miClienteIdentity.Genero = datoscliente.Genero;
            miClienteIdentity.Descripcion = datoscliente.Descripcion;
            miClienteIdentity.FechaNacimiento = datoscliente.FechaNacimiento;
            miClienteIdentity.UserName = datoscliente.Credenciales.Login;
            //Si la password que recibo no está vacía
            if (!String.IsNullOrEmpty(datoscliente.Credenciales.Password))
            {
                //Cambio la password del cliente
                miClienteIdentity.PasswordHash = this._userManagerService.PasswordHasher.HashPassword(miClienteIdentity, datoscliente.Credenciales.Password);
            }
            IdentityResult _resultModif = await this._userManagerService.UpdateAsync(miClienteIdentity);
            //Generamos cliente actualizado y nuevo token de sesion
            if (_resultModif.Succeeded)
            {
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Datos de cliente actualizados OK",
                    Error = "",
                    Tokensesion = this.__GeneraJWT(miClienteIdentity.Nombre, miClienteIdentity.Apellidos, miClienteIdentity.Email, miClienteIdentity.Id),
                    DatosCliente = await this.__GenerarClienteActualizado(datoscliente.IdCliente),
                    OtrosDatos = null
                };
            }
            else
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al actualizar datos de cliente",
                    Error = _resultModif.Errors.FirstOrDefault().Description,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("OperarDireccion")]
        public async Task<RestMessage> OperarDireccion([FromBody] Dictionary<string, string> datosdirec)
        {
            //en datosdirec <--- diccionario: Operacion: "crear|modificar|borrar", Direccion: "direccion_serializada" 
            try
            {
                //puedes extraer manualmente de las cabeceras de la peticion la q sea Authorization y extraer el JWT
                //y del mismo extraer los claims q te interesen (nombre y apellidos cliente, email cliente, ...)
                string _jwt = this.Request.Headers["Authorization"][0].Split(" ")[1].ToString();

                Direccion _direc = JsonSerializer.Deserialize<Direccion>(datosdirec["Direccion"]);

                //cursor de objetos con direccion q quiero modificar/borrar...
                IQueryable<Direccion> _queryDireccExiste = this._dbcontext
                                                                .Direcciones
                                                                .Where(
                                                                        (Direccion d) => d.IdDireccion == _direc.IdDireccion
                                                                  );

                switch (datosdirec["Operacion"])
                {
                    case "crear":
                        this._dbcontext.Direcciones.Add(_direc);
                        break;

                    case "modificar":
                        //dos formas de hacerlo:
                        // - recuperas el objeto direccion del dbset q quieres modificar y vas prop. por prop cambiando
                        //   sus valores con la nueva direccion: _direc
                        // - puedes cambiar todo el objeto entero mediante metodo .SetValues
                        if (_queryDireccExiste.Any())
                        {
                            Direccion _direcModif = _queryDireccExiste.Single<Direccion>();
                            this._dbcontext.Entry(_direcModif).CurrentValues.SetValues(_direc);
                        }
                        else
                        {
                            throw new Exception($"no existe ninguna direccion a MODIFICAR con ese id: {_direc.IdDireccion}");

                        }
                        break;

                    case "borrar":
                        if (_queryDireccExiste.Any())
                        {
                            this._dbcontext.Direcciones.Remove(_queryDireccExiste.Single<Direccion>());
                        }
                        else
                        {
                            throw new Exception($"no existe ninguna direccion a BORRAR con ese id: {_direc.IdDireccion}");
                        }
                        break;
                    case "principal":
                        //recuperar todas las direcciones del cliente
                        IQueryable<Direccion> _todasDirecciones = this._dbcontext.Direcciones.Where((Direccion d) => d.IdCliente == _direc.IdCliente);
                        //recorrer todas las direcciones y poner a false la propiedad principal
                        foreach (Direccion _dir in _todasDirecciones)
                        {
                            _dir.EsPrincipal = false;
                        }
                        //recuperar la direccion q quiero poner como principal y poner a true su propiedad principal
                        Direccion _dirPrincipal = _todasDirecciones.Where((Direccion d) => d.IdDireccion == _direc.IdDireccion).Single();
                        _dirPrincipal.EsPrincipal = true;
                        break;
                }
                this._dbcontext.SaveChanges();

                //retornar objeto cliente actualizado con direcciones actualizadas...generar JWT y devolver respuesta
                Cliente _datosCliente = await this.__GenerarClienteActualizado(_direc.IdCliente);

                return new RestMessage
                {
                    Codigo = 0,
                    Mensaje = $"operacion: {datosdirec["Operacion"]} sobre direccion con id: {_direc.IdDireccion} realizada OK!!!",
                    Error = null,
                    DatosCliente = _datosCliente,
                    Tokensesion = this.__GeneraJWT(_datosCliente.Nombre,
                                                  _datosCliente.Apellidos,
                                                  _datosCliente.Credenciales.Email,
                                                  _datosCliente.IdCliente),
                    OtrosDatos = null
                };
            }
            catch (Exception ex)
            {

                return new RestMessage
                {
                    Codigo = 1,
                    Mensaje = $"operacion: {datosdirec["Operacion"]} sobre direccion  ERRONEA!!",
                    Error = ex.Message,
                    DatosCliente = null,
                    Tokensesion = null,
                    OtrosDatos = null
                };
            }

        }
        [HttpGet]
        [Route("RecuperarNombreLogin")]
        public async Task<String> RecuperarNombreLogin([FromQuery] String idcliente)
        {
            try
            {
                MiClienteIdentity _cliente = await this._userManagerService.FindByIdAsync(idcliente);
                return _cliente.UserName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UploadOpinion")]
        public async Task<RestMessage> UploadOpinion(Opinion opinion)
        {
            try
            {
                this._dbcontext.Opiniones.Add(opinion);
                this._dbcontext.SaveChanges();
                //Generamos cliente con el id que se encuentra en la opinion
                Cliente _cliente = await this.__GenerarClienteActualizado(opinion.IdCliente);
                //Generamos token de sesion y devolvemos respuesta
                String _jwt = this.__GeneraJWT(_cliente.Nombre, _cliente.Apellidos, _cliente.Credenciales.Email, _cliente.IdCliente);
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Opinion subida OK",
                    Error = "",
                    Tokensesion = _jwt,
                    DatosCliente = _cliente,
                    OtrosDatos = null
                };
            }
            catch (Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al subir opinion",
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("OperarOpinion")]
        public async Task<RestMessage> OperarOpinion([FromBody] Opinion opi, [FromQuery] String operacion)
        {
            try
            {
                //Recuperar la opinion de la base de datos
                Opinion _opinion = this._dbcontext.Opiniones.Where((Opinion o) => o.IdOpinion == opi.IdOpinion).Single();
                switch (operacion)
                {
                    case ("modificar"):
                        _opinion.Comentario = opi.Comentario;
                        break;
                    case ("borrar"):
                        this._dbcontext.Opiniones.Remove(_opinion);
                        break;
                }
                this._dbcontext.SaveChanges();
                //Recuperamos cliente actualizado y generamos token de sesion
                Cliente _cliente = await this.__GenerarClienteActualizado(_opinion.IdCliente);
                String _jwt = this.__GeneraJWT(_cliente.Nombre, _cliente.Apellidos, _cliente.Credenciales.Email, _cliente.IdCliente);
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Opinion modificada OK",
                    Error = "",
                    Tokensesion = _jwt,
                    DatosCliente = _cliente,
                    OtrosDatos = null
                };
            }
            catch (Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al modificar opinion",
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("CrearLista")]
        public async Task<RestMessage> CrearLista(ListaLibros lista)
        {
            try
            {
                this._dbcontext.ListasLibros.Add(lista);
                this._dbcontext.SaveChanges();
                //Recuperamos cliente actualizado y generamos token de sesion
                Cliente _cliente = await this.__GenerarClienteActualizado(lista.IdCliente);
                String _jwt = this.__GeneraJWT(_cliente.Nombre, _cliente.Apellidos, _cliente.Credenciales.Email, _cliente.IdCliente);
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Lista de libros creada OK",
                    Error = "",
                    Tokensesion = _jwt,
                    DatosCliente = _cliente,
                    OtrosDatos = null
                };
            }
            catch (Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al crear lista de libros",
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateListasCliente")]
        public async Task<RestMessage> UpdateListasCliente(List<ListaLibros> listas)
        {
            try
            {
                foreach (ListaLibros _lista in listas)
                {
                    ListaLibros _listaModif = this._dbcontext.ListasLibros.Where((ListaLibros l) => l.IdLista == _lista.IdLista).Single();
                    _listaModif.NombreLista = _lista.NombreLista;
                    _listaModif.Libros = _lista.Libros;
                }
                this._dbcontext.SaveChanges();
                //Recuperamos cliente actualizado y generamos token de sesion
                Cliente _cliente = await this.__GenerarClienteActualizado(listas[0].IdCliente);
                String _jwt = this.__GeneraJWT(_cliente.Nombre, _cliente.Apellidos, _cliente.Credenciales.Email, _cliente.IdCliente);
                return new RestMessage()
                {
                    Codigo = 0,
                    Mensaje = "Listas de libros actualizadas OK",
                    Error = "",
                    Tokensesion = _jwt,
                    DatosCliente = _cliente,
                    OtrosDatos = null
                };
            }
            catch (Exception ex)
            {
                return new RestMessage()
                {
                    Codigo = 1,
                    Mensaje = "Error al actualizar listas de libros",
                    Error = ex.Message,
                    Tokensesion = null,
                    DatosCliente = null,
                    OtrosDatos = null
                };
            }
        }
        #endregion
        //Si quiero restringir el acceso a un metodo de un controlador a usuarios autenticados, debo decorar el metodo con el atributo [Authorize]

        #region ...metodos privados ...
        private String __GeneraJWT(String nombre, String apellidos, String email, String idcliente)
        {
            SecurityKey _clavefirma = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this._accesoAppSettings["JWT:firmaJWT"]));

            JwtSecurityToken _jwt = new JwtSecurityToken(
                    issuer: this._accesoAppSettings["JWT:issuer"],
                    audience: null,
                    claims: new List<Claim> {
                                new Claim("nombre",nombre),
                                new Claim("apellidos", apellidos),
                                new Claim("email", email),
                                new Claim("clienteId", idcliente)
                    },
                    notBefore: null,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(_clavefirma, SecurityAlgorithms.HmacSha256)
                );

            string _tokenjwt = new JwtSecurityTokenHandler().WriteToken(_jwt);
            return _tokenjwt;
        }
        private async Task<Cliente> __GenerarClienteActualizado(string idcliente)
        {
            MiClienteIdentity _cliente = await this._userManagerService.FindByIdAsync(idcliente);
            Cliente _datoscliente = new Cliente()
            {
                IdCliente = _cliente.Id,
                Nombre = _cliente.Nombre,
                Apellidos = _cliente.Apellidos,
                Telefono = _cliente.PhoneNumber,
                Genero = _cliente.Genero,
                Descripcion = _cliente.Descripcion,
                FechaNacimiento = _cliente.FechaNacimiento,
                Credenciales = new Cuenta
                {
                    Email = _cliente.Email,
                    Login = _cliente.UserName,
                    Password = "",
                    ImagenCuentaBASE64 = _cliente.ImagenAvatarBASE64
                }
            };
            _datoscliente.PedidosCliente = this._dbcontext.Pedidos.Where((Pedido p) => p.IdCliente == _cliente.Id).ToList<Pedido>();
            _datoscliente.DireccionesCliente = this._dbcontext.Direcciones.Where((Direccion d) => d.IdCliente == _cliente.Id).ToList<Direccion>();
            _datoscliente.OpinionesCliente = this._dbcontext.Opiniones.Where((Opinion o) => o.IdCliente == _cliente.Id).ToList<Opinion>();
            _datoscliente.ListasCliente = this._dbcontext.ListasLibros.Where((ListaLibros l) => l.IdCliente == _cliente.Id).ToList<ListaLibros>();

            return _datoscliente;
        }
        #endregion
        #endregion
    }
}
