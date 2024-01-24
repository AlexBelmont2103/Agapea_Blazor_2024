
using Agapea_Blazor_2024.Server.Models;
using Agapea_Blazor_2024.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Agapea_Blazor_2024.Server.Controllers
{
    [Route("api/[controller]/[action]")]
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
                    
            }catch(Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public Libro RecuperarLibro([FromQuery] String isbn13)
        {
            try
            {
                return this._dbContext.Libros.Where((Libro unLibro) => unLibro.ISBN13 == isbn13).Single<Libro>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<Categoria> RecuperarCategorias([FromQuery]String idcat)
        {

            try
            {
                //Si idcat está vacío, devolver las categorias padre con un regex
                //Si idcat no está vacío, devolver las categorias hijas de la categoria indicada en idcat
                //Convertir el regex según el formato de idcat
                Regex _regex = new Regex(@"^" + idcat + @"-\d+$");
                if (String.IsNullOrEmpty(idcat)) _regex= new Regex(@"^[0-9]{1,}$");
                //Si usas patrones directamente en la consulta LINQ, EF no puede convertirlo a SQL
                //Dos soluciones:
                //Usar el operador LIKE de SQLServer se mapea contra el operador Contains de LINQ
                //Coger todos los datos de la tabla y filtrarlos en memoria con LINQ
                // Para hacer esto usas el metodo .AsEnumerable() tras el nombre del DbSet
                //return this._dbContext.Categorias.Where((Categoria unaCategoria) => _regex.IsMatch(unaCategoria.IdCategoria)).ToList<Categoria>();
                return this._dbContext.Categorias.AsEnumerable<Categoria>().Where((Categoria unaCategoria) => _regex.IsMatch(unaCategoria.IdCategoria)).ToList<Categoria>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
