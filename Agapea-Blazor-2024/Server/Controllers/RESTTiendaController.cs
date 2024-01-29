using Agapea_Blazor_2024.Server.Models;
using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Agapea_Blazor_2024.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RESTTiendaController : ControllerBase
    {
        #region propiedades de la clase RESTTiendaController
        private AplicacionDBContext _dbContext;
        #endregion
        public RESTTiendaController(AplicacionDBContext _dbContext)
        {
            this._dbContext = _dbContext;
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
        #endregion
    }
}
