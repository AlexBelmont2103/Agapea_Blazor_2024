namespace Agapea_Blazor_2024.Shared
{
    public class ListaLibros
    {
        public String IdLista { get; set; } = Guid.NewGuid().ToString();
        public String IdCliente { get; set; }
        public String NombreLista { get; set; }
        public List<Libro> Libros { get; set; } = new List<Libro>();
    }
}
