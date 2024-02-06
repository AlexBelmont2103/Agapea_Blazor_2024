namespace Agapea_Blazor_2024.Shared
{
    public class Provincia
    {
        #region ....propiedades de clase Provincia (DEBEN COINCIDIR CON PROPS. DE JSON NOS DEVUELVE SERVICIO EXTERNO!!!)
        public String CCOM { get; set; } = "";
        public String CPRO { get; set; } = "";
        public String PRO { get; set; } = "";

        #endregion
        #region ....métodos de clase Provincia....
        public String ToString()
        {
            return $"CCOM: {CCOM}\n" +
                $"CPRO: {CPRO}\n" +
                $"PRO: {PRO}";
        }
        #endregion
    }
}
