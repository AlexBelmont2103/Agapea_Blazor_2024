namespace Agapea_Blazor_2024.Shared
{
    public class DatosPago
    {
        #region ... propiedades de clase DatosPago ...
        public String TipoDireccionEnvio { get; set; } = "";
        public Direccion DireccionPrincipal { get; set; } = new Direccion();
        #region Datos envío
        public Direccion DireccionEnvio { get; set; } = new Direccion();
        public String NombreEnvio { get; set; } = "";
        public String ApellidosEnvio { get; set; } = "";
        public String TelefonoEnvio { get; set; } = "";
        public String? OtroTelefono { get; set; } = "";
        public String EmailEnvio { get; set; } = "";
        public String? OtrosDatos { get; set; } = "";
        #endregion
        #region Datos facturación
        public String? TipoFactura { get; set; } = "";
        public String? nombreFactura { get; set; } = "";
        public String? DocFiscalFactura { get; set; } = "";
        public Direccion? DireccionFactura { get; set; }
        public String? TipoDireccionFactura { get; set; } = "";
        #endregion
        #region Datos pago
        public String MetodoPago { get; set; } = "";
        public String? NumeroTarjeta { get; set; } = "";
        public String? NombreBanco { get; set; } = "";
        public int? MesCaducidad { get; set; } = 0;
        public int? AnioCaducidad { get; set; } = 0;
        public int? CVV { get; set; } = 0;
        #endregion
        #endregion
        #region ... métodos de clase DatosPago ...
        public String ToString()
        {
            return $"Tipo de dirección de envío: {TipoDireccionEnvio}\n" +
                $"Dirección de envío: {DireccionEnvio.ToString()}\n" +
                $"Nombre de envío: {NombreEnvio}\n" +
                $"Apellidos de envío: {ApellidosEnvio}\n" +
                $"Teléfono de envío: {TelefonoEnvio}\n" +
                $"Otro teléfono: {OtroTelefono}\n" +
                $"Email de envío: {EmailEnvio}\n" +
                $"Otros datos: {OtrosDatos}\n" +
                $"Tipo de factura: {TipoFactura}\n" +
                $"Nombre de factura: {nombreFactura}\n" +
                $"Documento fiscal de factura: {DocFiscalFactura}\n" +
                $"Dirección de factura: {DireccionFactura?.ToString()}\n" +
                $"Tipo de dirección de factura: {TipoDireccionFactura}\n" +
                $"Método de pago: {MetodoPago}\n" +
                $"Número de tarjeta: {NumeroTarjeta}\n" +
                $"Nombre del banco: {NombreBanco}\n" +
                $"Mes de caducidad: {MesCaducidad}\n" +
                $"Año de caducidad: {AnioCaducidad}\n" +
                $"CVV: {CVV}";
        }
        #endregion

    }
}
