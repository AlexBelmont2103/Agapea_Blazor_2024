using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor_2024.Shared
{
    public class Opinion
{
    #region ....propiedades clase opinion...
    public String IdOpinion { get; set; } = Guid.NewGuid().ToString();
    public String IdCliente { get; set; } = "";
    public String IdLibro { get; set; } = "";
    public String Comentario { get; set; } = "";
    public int Valoracion { get; set; } = 0;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool EsModerada { get; set; } = false;
    #endregion
    public Opinion()
    {
    }
    public Opinion(String idCliente, String idLibro, String comentario, int valoracion)
    {
        this.IdCliente = idCliente;
        this.IdLibro = idLibro;
        this.Comentario = comentario;
        this.Valoracion = valoracion;
    }

    #region ...metodos clase opinon.....

    #endregion
}
}
