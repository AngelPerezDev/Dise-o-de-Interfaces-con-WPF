using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ut7_actividad
{
    class Ventas
    {
        public int idVenta;
        public int sesion;
        public int cantidad;
        public string pago;

        public Ventas()
        {
        }

        public Ventas(int idVenta, int sesion, int cantidad, string pago)
        {
            this.idVenta = idVenta;
            this.sesion = sesion;
            this.cantidad = cantidad;
            this.pago = pago;
        }

        public int IdVenta { get => idVenta; }

        public int Sesion { get => sesion; }

        public int Cantidad { get => cantidad; }

        public string Pago { get => pago; }
    }
}
