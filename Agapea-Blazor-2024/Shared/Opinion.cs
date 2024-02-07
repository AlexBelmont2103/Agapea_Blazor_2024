namespace Agapea_Blazor_2024.Shared
{
    public class Opinion
    {
        #region ....propiedades clase opinion...
        public String IdCliente { get; set; } = "";
        public String IdOpinion { get; set; } = Guid.NewGuid().ToString();
        public String isbn13 { get; set; } = "";
        public String Comentario { get; set; } = "";
        public int Valoracion { get; set; } = 0;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool EsModerada { get; set; } = false;
        #endregion
        public Opinion()
        {
        }
        public Opinion(String idCliente, String isbn13, String comentario, int valoracion)
        {
            this.IdCliente = idCliente;
            this.isbn13 = isbn13;
            this.Comentario = comentario;
            this.Valoracion = valoracion;
        }

        #region ...metodos clase opinon.....

        #endregion
    }
}
