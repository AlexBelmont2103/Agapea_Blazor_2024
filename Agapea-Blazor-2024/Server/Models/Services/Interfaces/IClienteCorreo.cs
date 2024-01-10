namespace Agapea_Blazor_2024.Server.Models.Services
{
    public interface IClienteCorreo
    {

        #region ... Propiedades de la interface que deben implementar servicios para mandar emails ...
        //Como propiedades: Usuario con el que te conectas al servicio externo proveedor de correo
        //                  Password con la que te identificas ante el servicio externo para poder mandar correos

        public String UserId { get; set; }
        public String Password { get; set; }
        #endregion



        #region ... Métodos de la interace que deben implementar servicios para mandar emails ...
        public bool EnviarEmail(string emailCliente, string subject, string cuerpoMensaje, string? ficheroAdjunto);
        #endregion

    }
}
