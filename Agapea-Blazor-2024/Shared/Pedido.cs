using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor_2024.Shared
{
    public class Pedido
{
    #region ....propiedades de clase....
    public String IdPedido { get; set; } = Guid.NewGuid().ToString();
    public DateTime FechaPedido { get; set; } = DateTime.Now;
    public List<ItemPedido> ElementosPedido { get; set; } = new List<ItemPedido>();

    public Direccion DireccionEnvio { get; set; }
    public Direccion DireccionFacturacion { get; set; }

    public Decimal SubTotal { get; set; } = 0;
    public Decimal GastosEnvio { get; set; } = 2;
    public Decimal Total { get; set; } = 0;
    public String EstadoPedido { get; set; } = "en preparacion";


    #endregion

    #region ... metodos de clase....
    public Decimal CalculoSubTotalPedido()
    {
        //Decimal _subtotal = 0;
        //foreach (ItemPedido item in this.ElementosPedido)
        //{
        //    _subtotal += item.LibroItem.Precio * item.CantidadItem;
        //}
        //return _subtotal;

        this.SubTotal = this.ElementosPedido.Sum((ItemPedido item) => item.LibroItem.Precio * item.CantidadItem);


        this.Total = this.SubTotal + this.GastosEnvio;

        return this.SubTotal;
    }

    #endregion
}
}
