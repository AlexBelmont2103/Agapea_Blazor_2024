namespace Agapea_Blazor_2024.Shared
{
    public class DatosPago
    {
        #region ... propiedades de clase DatosPago ...
        public String TipoDireccion { get; set; } = "";
        public Direccion DireccionPrincipal { get; set; } = new Direccion();
        #region Datos envío
        public Direccion DireccionEnvio { get; set; } = new Direccion();
        public String NombreEnvio { get; set; } = "";
        public String ApellidosEnvio { get; set; } = "";
        public String TelefonoEnvio { get; set; } = "";
        public String EmailEnvio { get; set; } = "";
        #endregion
        #region Datos facturación
        public String TipoFactura { get; set; } = "";
        public String nombreFactura { get; set; } = "";
        public String DocFiscalFactura { get; set; } = "";
        public Direccion? DireccionFactura { get; set; }
        #endregion
        #region Datos pago
        public String MetodoPago { get; set; } = "";
        public String NumeroTarjeta { get; set; } = "";
        public String NombreBanco { get; set; } = "";
        public int MesCaducidad { get; set; } = 0;
        public int AnioCaducidad { get; set; } = 0;
        public int CVV { get; set; } = 0;
        #endregion
        #endregion
    }
}
