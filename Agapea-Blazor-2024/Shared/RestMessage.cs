namespace Agapea_Blazor_2024.Shared
{
    public class RestMessage
    {
        //Clase que mapea la respuesta del servicio REST

        #region propiedades de la clase RestMessage

        public int Codigo { get; set; } = 0;
        public String Mensaje { get; set; } = "";
        public String Error { get; set; } = "";
        public String TokenSesion { get; set; } = "";
        public Cliente? DatosCliente { get; set; } = null;
        public Object? OtrosDatos { get; set; } = null;
        #endregion

        #region metodos de la clase RestMessage

        #endregion
    }
}
