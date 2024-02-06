namespace Agapea_Blazor_2024.Shared
{
    public class Municipio
    {
        #region ...propiedades de clase Municipio (DEBEN COINCIDIR CON PROPS. JSON DEVUELVE SERV.REST EXTERNO)...
        public String CPRO { get; set; } = "";
        public String CMUM { get; set; } = "";
        public String CUN { get; set; } = "";
        public String DMUN50 { get; set; } = "";

        #endregion
        #region ...métodos de clase Municipio...
        public String ToString()
        {
            return $"CPRO: {CPRO}\n" +
                $"CMUM: {CMUM}\n" +
                $"CUN: {CUN}\n" +
                $"DMUN50: {DMUN50}";
        }
        #endregion
    }
}
