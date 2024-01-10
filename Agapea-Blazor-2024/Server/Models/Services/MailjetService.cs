using System.Net;
using System.Net.Mail;
using Agapea_Blazor_2024.Server.Models.Services;

namespace agapea_netcore_mvc_23_24.Models.Servicios
{
    public class MailjetService : IClienteCorreo
    {

        #region ... Propiedades de la clase MailjetService
        private IConfiguration __accesoAppSettings;
        public string UserId { get; set; }
        public string Password { get; set; }
        #endregion
        public MailjetService(IConfiguration servicioAccesoAppSettingsDI)
        {
            this.__accesoAppSettings = servicioAccesoAppSettingsDI;

            this.UserId = servicioAccesoAppSettingsDI.GetSection("MailJetCredentials:ClaveAPI").Value;
            this.Password = servicioAccesoAppSettingsDI.GetSection("MailJetCredentials:ClaveSecreta").Value;
        }


        #region ... Métodos de la clase MailjetService
        public bool EnviarEmail(string emailCliente, string subject, string cuerpoMensaje, string? ficheroAdjunto)
        {
            try
            {
                //Abrir socket al servidor SMTP de Mailjet con las credenciales de la API
                //Usando la clase SmtpClient
                SmtpClient _clienteSMTP = new SmtpClient("in-v3.mailjet.com");
                _clienteSMTP.Credentials = new NetworkCredential(this.UserId, this.Password);

                //Crear el cuerpo del mensaje de correo a mandar al cliente
                //Usando la clase MailMessage
                MailMessage _mensajeAEnviar = new MailMessage("alejandromgarcia90@gmail.com", emailCliente);
                _mensajeAEnviar.Subject = subject;
                _mensajeAEnviar.IsBodyHtml = true; //Para incrustar contenido html en el email (botones, imágenes, etc)
                _mensajeAEnviar.Body = cuerpoMensaje;
                if (!String.IsNullOrEmpty(ficheroAdjunto))
                {
                    _mensajeAEnviar.Attachments.Add(new Attachment(ficheroAdjunto));
                }
                //Mandar el email usando el socket abierto con el cliente SMTP creado (método send)
                _clienteSMTP.Send(_mensajeAEnviar);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion


    }
}
