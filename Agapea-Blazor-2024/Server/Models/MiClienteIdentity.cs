using Microsoft.AspNetCore.Identity;

namespace Agapea_Blazor_2024.Server.Models
{
    public class MiClienteIdentity : IdentityUser
    {
        //Clase personalizada para añadir sobre las props de identityuser datos propios que me interesan y que Identity no refleja
        #region ... propiedades nuevas que añado a la clase MiClienteIdentity ...

        public String Nombre { get; set; }
        public String Apellidos { get; set; }
        public String NIF { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public String genero { get; set; }
        public String Descripcion { get; set; }
        public String ImagenAvatarBASE64 { get; set; }
        #endregion
    }
}
