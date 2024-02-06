namespace Agapea_Blazor_2024.Shared
{
    public class Direccion
    {
        #region ...propiedades clase direccion....
        public String IdDireccion { get; set; } = Guid.NewGuid().ToString();
        public String Calle { get; set; } = "";
        public int CP { get; set; } = 0;
        public Provincia ProvinciaDirec { get; set; } = new Provincia();
        public Municipio MunicipioDirec { get; set; } = new Municipio();
        public String Pais { get; set; } = "España";
        public Boolean EsPrincipal { get; set; } = false;
        public Boolean EsFacturacion { get; set; } = false;
        #endregion
        #region ...métodos clase direccion...
        public String ToString()
        {
            return $"Calle: {Calle}\n" +
                $"Código postal: {CP}\n" +
                $"Provincia: {ProvinciaDirec.ToString()}\n" +
                $"Municipio: {MunicipioDirec.ToString()}\n" +
                $"País: {Pais}\n" +
                $"Es principal: {EsPrincipal}\n" +
                $"Es de facturación: {EsFacturacion}";
        }
        #endregion
    }
}
