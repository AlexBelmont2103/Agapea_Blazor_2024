using Agapea_Blazor_2024.Server.Models;
using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Agapea_Blazor_2024.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RESTTiendaController : ControllerBase
    {
        #region propiedades de la clase RESTTiendaController
        private AplicacionDBContext _dbContext;
        private IConfiguration __iconfig;
        private static readonly HttpClient cliente = new HttpClient(); //Para hacer llamadas a servicios API REST como PAYPAL
        #endregion
        public RESTTiendaController(AplicacionDBContext _dbContext, IConfiguration iconfig)
        {
            this._dbContext = _dbContext;
            this.__iconfig = iconfig;
        }
        #region ...metodos de la clase RESTTiendaController

        [HttpGet]
        public List<Libro> RecuperarLibros([FromQuery] String idcat)
        {
            try
            {
                if (String.IsNullOrEmpty(idcat)) idcat = "2-10";
                return this._dbContext.Libros.Where((Libro unLibro) => unLibro.IdCategoria.StartsWith(idcat)).ToList<Libro>();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public Libro RecuperarLibro([FromQuery] String isbn13)
        {
            try
            {
                return this._dbContext
                            .Libros
                            .Where(
                                    (Libro unlibro) => unlibro.ISBN13 == isbn13
                                    )
                            .Single<Libro>();
            }
            catch (Exception ex)
            {

                return new Libro();
            }
        }

        [HttpGet]
        public List<Categoria> RecuperarCategorias([FromQuery] String idcat)
        {
            try
            {
                //si en idcat esta vacio, quiero recuperar categorias "raiz" IdCategoria="un digito"
                //si no, quiero recuperar subcategorias de una categoria q pasan:  IdCategoria=idcat-"digito"
                Regex _patronBusqueda;
                if (String.IsNullOrEmpty(idcat) || idcat == "raices")
                {
                    _patronBusqueda = new Regex("^[0-9]{1,}$"); //<<---- "^\d+$"
                }
                else
                {
                    _patronBusqueda = new Regex("^" + idcat + "-[0-9]{1,}$");
                }
                //si usas patrones directamente en la consulta LINQ al intentar traducir esta consulta a lenguaje SQL
                //en SqlServer, como no hay operadores de tipo expresion Regular no puede convertirlo y zzzzzzzzasssss
                //excepcion....¿solucion?

                //return this._dbContext
                //            .Categorias
                //            .Where(
                //                    (Categoria unacat) => _patronBusqueda.IsMatch(unacat.IdCategoria)
                //                )
                //            .ToList<Categoria>();



                //dos opciones:
                // - el operador LIKE si existe en sqlserver <---- se mapea contra metodo .Contains() de LINQ
                // - te descargas tooooooooooooda la tabla en memoria y luego lo filtras con op.linq
                //    para hacer esto usas el metodo .AsEnumerable() tras el nombre del DbSet
                return this._dbContext
                            .Categorias
                            .AsEnumerable<Categoria>() //<--- SELECT * FROM dbo.Categorias y por cada fila construye objeto Categoria
                            .Where(
                                    (Categoria unacat) => _patronBusqueda.IsMatch(unacat.IdCategoria)
                                )
                            .ToList<Categoria>();

            }
            catch (Exception ex)
            {
                return new List<Categoria>();
            }
        }
        [HttpGet]
        public List<Provincia> RecuperarProvincias()
        {
            try
            {
                return this._dbContext.Provincias.AsEnumerable<Provincia>().OrderBy((Provincia p) => p.PRO).ToList<Provincia>();
            }
            catch (Exception ex)
            {

                return new List<Provincia>();
            }
        }

        [HttpGet]
        public List<Municipio> RecuperarMunicipios([FromQuery] String cpro)
        {
            try
            {
                return this._dbContext.Municipios.Where((Municipio muni) => muni.CPRO == cpro).ToList<Municipio>();
            }
            catch (Exception ex)
            {

                return new List<Municipio>();
            }
        }
        [HttpPost]
        public async Task<String> FinalizarPedido([FromBody] Dictionary<string, string> dic)
        {
            try
            {
                DatosPago datosPago = JsonSerializer.Deserialize<DatosPago>(dic["datosPago"]);
                Pedido pedido = JsonSerializer.Deserialize<Pedido>(dic["pedido"]);
                if (datosPago.MetodoPago == "pagoTarjeta")
                {
                    bool bandera = await PagoStripeNoAPI(datosPago, pedido);
                    if (bandera)
                    {
                        return "Pedido finalizado con exito";
                    }
                    else
                    {
                        return "Pedido no finalizado con exito";
                    }
                }
                else if (datosPago.MetodoPago == "pagoPayPal")
                {
                    //1º acceder a las claves de desarrollador de la api de paypal
                    HttpRequestMessage _requestToken = new HttpRequestMessage(HttpMethod.Post,
                                                                              "https://api-m.sandbox.paypal.com/v1/oauth2/token");
                    //Cabecera Authorization: Basic con las credenciales en base64
                    //Cuerpo de la peticion en formato x-www-form-urlencoded variable: grant_type valor:client_credentials

                    string _clientId = this.__iconfig["PayPalAPIKEYS:ClientId"];
                    string _clientSecret = this.__iconfig["PayPalAPIKEYS:ClientSecret"];
                    string _credenciales = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                    _requestToken.Headers.Add("Authorization", $"Basic {_credenciales}");
                    _requestToken.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage _responseToken = await cliente.SendAsync(_requestToken);
                    if (_responseToken.IsSuccessStatusCode)
                    {
                        String _respJson = await _responseToken.Content.ReadAsStringAsync();
                        JsonNode _respJsonDeserializado = JsonNode.Parse(_respJson);
                        String _accessToken = _respJsonDeserializado["access_token"].ToString();
                        //Token a añadir a la cabecera Authorization: Bearer para crear el order, confirmarlo y capturarlo

                        //2º Crear un order via api-rest de paypal

                        HttpRequestMessage _requestOrder = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v2/checkout/orders");
                        _requestOrder.Headers.Add("Authorization", $"Bearer {_accessToken}");
                        var listaItems = pedido.ElementosPedido.Select((ItemPedido unElemento) => new
                        {
                            name = unElemento.LibroItem.Titulo,
                            unit_amount = new
                            {
                                currency_code = "EUR",
                                value = unElemento.LibroItem.Precio.ToString().Replace(",", ".")
                            },
                            quantity = unElemento.CantidadItem.ToString()
                        }).ToList();
                        var order = new
                        {
                            intent = "CAPTURE",
                            purchase_units = new[] {
                                        new {
                                            items=listaItems,
                                            amount= new {
                                                            currency_code="EUR",
                                                            value=pedido.Total.ToString().Replace(",","."),
                                                            breakdown=new {
                                                                shipping=new { currency_code="EUR", value=pedido.GastosEnvio.ToString().Replace(",",".") },
                                                                item_total=new {currency_code="EUR", value=pedido.SubTotal.ToString().Replace(",",".")}
                                                            }
                                                        }
                                        }
                            },
                            application_context = new
                            {
                                return_url = $"https://localhost:7286/api/RESTTienda/PaypalCallBack?idcliente={pedido.IdCliente}&idpedido={pedido.IdPedido}",
                                cancel_url = $"https://localhost:7286/api/RESTTienda/PaypalCallBack?idcliente={pedido.IdCliente}&idpedido={pedido.IdPedido}&cancel=true"
                            }
                        };
                        _requestOrder.Content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
                        HttpResponseMessage _responseOrder = await cliente.SendAsync(_requestOrder);
                        if (_responseOrder.IsSuccessStatusCode)
                        {
                            string _respOrderJson = await _responseOrder.Content.ReadAsStringAsync();
                            JsonNode _respOrderJsonDeserializado = JsonNode.Parse(_respOrderJson);
                            //Del json de respuesta me interesa: id, que es el id del pago para meterlo en la tabla junto con idcliente e idpedido
                            //del array de links, el que contiene approve para mandarselo al cliente blazor para que le redireccione a la pagina de paypal
                            String _idOrder = _respOrderJsonDeserializado["id"].ToString();
                            String _linkApprove = _respOrderJsonDeserializado["links"].AsArray().Where((JsonNode unLink) => unLink["rel"].ToString() == "approve").Select((JsonNode unLink) => unLink["href"].ToString()).Single<String>();
                            return _linkApprove;
                        }
                    }
                    else
                    {
                        throw new Exception("No se ha podido obtener el token de acceso a la api de paypal");
                    }

                    return "Pedido finalizado con exito";
                }
                else
                {
                    return "Pedido no finalizado con exito";
                }

            }
            catch (Exception ex)
            {
                return "Hubo algún problema al finalizar el pedido";
            }
        }
        [HttpGet]
        public async Task<String> PaypalCallBack([FromQuery] String idcliente, [FromQuery] String idpedido, [FromQuery] String cancel)
        {
            //Vamos simplemente a recuperar el cliente y ya esta




        }
        #endregion
        #region ///METODOS PRIVADOS ///
        private async Task<bool> PagoStripeNoAPI(DatosPago datos, Pedido pedido)
        {
            bool bandera = false;
            try
            {
                //1º Crear un objeto customer via api-rest de stripe
                string _claveStripe = this.__iconfig["StripeCredentials:ClaveAPI"];
                string _claveSecreta = this.__iconfig["StripeCredentials:ClaveSecreta"];

                //Datos a la peticion rest tienen que ir estilo formulario x-www-form-urlencoded
                //variable=valor variable=valor variable=valor
                Dictionary<string, string> customerStripeValues = new Dictionary<string, string>()
            {
                {"name",datos.NombreEnvio+" "+datos.ApellidosEnvio },
                {"email", datos.EmailEnvio },
                {"phone", datos.TelefonoEnvio },
                {"address[city]", datos.DireccionEnvio.MunicipioDirec.DMUN50 },
                {"address[country]", datos.DireccionEnvio.Pais },
                {"address[line1]", datos.DireccionEnvio.Calle },
                {"address[postal_code]", datos.DireccionEnvio.CP.ToString() },
                {"address[state]", datos.DireccionEnvio.ProvinciaDirec.PRO}
            };
                HttpRequestMessage _request = new HttpRequestMessage(HttpMethod.Post, "https://api.stripe.com/v1/customers");
                _request.Headers.Add("Authorization", $"Bearer {_claveSecreta}");
                _request.Content = new FormUrlEncodedContent(customerStripeValues);

                HttpResponseMessage _response = await cliente.SendAsync(_request);
                if (_response.IsSuccessStatusCode)
                {
                    //Crear card asociada al customer-id que me devuelve stripe
                    //Deserializar objeto _response sin usar clases
                    //Recuperar la propiedad id del objeto deserializado utiliando jsondom
                    string _contenidorespuesta = await _response.Content.ReadAsStringAsync();
                    JsonNode _contenidorespuestaDeserializado = JsonNode.Parse(_contenidorespuesta);
                    string _idCustomer = _contenidorespuestaDeserializado["id"].ToString();
                    //Crear card asociada al customer-id que me devuelve stripe
                    Dictionary<string, string> cardStripeValues = new Dictionary<string, string>()
                {
                    {"source","tok_visa_debit" }
                };
                    HttpRequestMessage _requestCard = new HttpRequestMessage(HttpMethod.Post, $"https://api.stripe.com/v1/customers/{_idCustomer}/sources");
                    _requestCard.Headers.Add("Authorization", $"Bearer {_claveSecreta}");
                    _requestCard.Content = new FormUrlEncodedContent(cardStripeValues);

                    HttpResponseMessage _responseCard = await cliente.SendAsync(_requestCard);
                    if (_responseCard.IsSuccessStatusCode)
                    {
                        //Crear un charge con el customer id y el card id
                        string _contenidorespuestaCard = await _responseCard.Content.ReadAsStringAsync();
                        JsonNode _contenidorespuestaCardDeserializado = JsonNode.Parse(_contenidorespuestaCard);
                        string _idCard = _contenidorespuestaCardDeserializado["id"].ToString();
                        Dictionary<string, string> chargeStripeValues = new Dictionary<string, string>()
                        {
                            {"amount",((int)(pedido.Total*100)).ToString() },
                            {"currency","eur" },
                            {"description",pedido.IdPedido },
                            {"customer",_idCustomer },
                            {"source",_idCard }
                        };
                        HttpRequestMessage _requestCharge = new HttpRequestMessage(HttpMethod.Post, "https://api.stripe.com/v1/charges");
                        _requestCharge.Headers.Add("Authorization", $"Bearer {_claveSecreta}");
                        _requestCharge.Content = new FormUrlEncodedContent(chargeStripeValues);

                        HttpResponseMessage _responseCharge = await cliente.SendAsync(_requestCharge);

                        if (_responseCharge.IsSuccessStatusCode)
                        {
                            //Leer propiedad status de la respuesta y comprobar que es "succeeded"
                            string _contenidorespuestaCharge = await _responseCharge.Content.ReadAsStringAsync();
                            JsonNode _contenidorespuestaChargeDeserializado = JsonNode.Parse(_contenidorespuestaCharge);
                            string _status = _contenidorespuestaChargeDeserializado["status"].ToString();
                            if (_status == "succeeded")
                            {
                                bandera = true;
                            }
                        }
                    }

                }
                return bandera;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
    }
}
