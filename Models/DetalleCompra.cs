﻿namespace AppProyecto.Models
{
    public class DetalleCompra
    {
        public int IdDetalleCompra { get; set; }
        public int IdCompra { get; set; }
        public int IdProducto { get; set; }
        public Producto oProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
