using System.ComponentModel.DataAnnotations;

namespace Agapea_Blazor_2024.Shared
{

    /*
  OJO!!!! la validacion con DataAnnotations solo vale para props. de primer nivel (nombre, apellidos, ...) NO
        PARA PROPIEDADES Q SON OBJETOS ANIDADOS, como Credenciales q es de tipo Cuenta.cs <--- p.e en formulario
        de InicioPanel, NO SE PRODUCIRIAN LAS VALIDACIONES SOBRE LOGIN ni PASSWORD!!! con el componente <DataAnnotationsValidator>...
        ¿¿solucion?? 2 posibles:
            - implementar en clase padre Cliente.cs la validacion de prop. objetos anidadas haciendo q implemente interface
            IValidateObject <---- exige implementar metodo Validate(..) q se lanza cada vez q se intenta validar una prop.
                                del objeto cliente 
            - si no te permiten tocar clase modelo, tienes q crearte un componente validador personalizado al q le pasas
            el EditContext con el modelo Cliente q va en su interior (como parametro o parametro en cascada)<==no tiene vista
            este componente solo codigo, y debe tener como prop un objeto ValidationMessageStore <=== lista de errores de validacion de todo el modelo
            y tienes q añadir manejadores de evento al editcontext:
                EditContext.OnValidationRequest += (sender, eventargs)=>{ ... }
                EditContext.OnFieldChanged += (sender, eventargs) => { ..... }
 */
    public class Cliente : IValidatableObject
    {
        #region Propiedades de clase
        public String IdCliente { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "* el Nombre es obligatorio")]
        [MaxLength(50, ErrorMessage = "* no se admiten mas de 50 caracteres en el nombre")]
        public String Nombre { get; set; } = "";

        [Required(ErrorMessage = "* los Apellidos son obligatorios")]
        [MaxLength(200, ErrorMessage = "* no se admiten mas de 200 caracteres en los apellidos")]
        public String Apellidos { get; set; } = "";

        [Required(ErrorMessage = "* el Telefono de contacto es obligatorio")]
        [RegularExpression(@"^\d{3}(\s?\d{2}){3}$", ErrorMessage = "* formato de Telefono invalido: 666 11 22 33")]
        public String Telefono { get; set; } = "";
        public String Genero { get; set; } = "";
        public String Descripcion { get; set; } = ""; //intereses, etc...
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        public Cuenta Credenciales { get; set; } = new Cuenta();
        public List<Direccion> DireccionesCliente { get; set; } = new List<Direccion>();
        public List<Pedido> PedidosCliente { get; set; } = new List<Pedido>();
        public List<Opinion> OpinionesCliente { get; set; } = new List<Opinion>();



        #endregion

        #region Métodos de clase
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //-------- este metodo se lanza cuando provocas VALIDACION DESDE CODIGO con metodo .validate() del modelo ----
            //el resultado de la validacion del objeto Cliente por medio de DataAnnotations es una lista de objetos de tipo
            //ValidationResult...pues creo lista para objetos propiedad anidados, como Credenciales:
            //para provocar la validacion desde codigo: hay una clase estatica Validator con metodos para validar props, objetos,...

            List<ValidationResult> _listaErroresCreds = new List<ValidationResult>();
            //Esto si tiene dataannotations, si no hay que hacerlo a pelo
            //Validator.TryValidateObject(
            //                               this.Credenciales,
            //                               new ValidationContext(this.Credenciales),
            //                               _listaErroresCreds,
            //                               true);
            if (!String.IsNullOrEmpty(this.Credenciales.Password))
            {
                if (this.Credenciales.Password.Length < 6)
                {
                    _listaErroresCreds.Add(new ValidationResult("* la contraseña debe tener al menos 6 caracteres", new String[] { "Credenciales.Password" }));
                }
                if (this.Credenciales.Password.Length > 200)
                {
                    _listaErroresCreds.Add(new ValidationResult("* la contraseña no puede tener mas de 200 caracteres", new String[] { "Credenciales.Password" }));
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(this.Credenciales.Password, @"^(?=.*\d)(?=.*[\u0021-\u002b\u003c-\u0040])(?=.*[A-Z])(?=.*[a-z])\S{8,}$"))
                {
                    _listaErroresCreds.Add(new ValidationResult("*la Contraseña debe contener al menos MAYS, MINS, digito y caracter raro."));
                }
            }
            return _listaErroresCreds;
        }
        #endregion
    }
}
