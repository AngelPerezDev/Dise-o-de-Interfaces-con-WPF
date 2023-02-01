using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ut7_actividad
{
    class Sesiones
    {
        public int idSesion;
        public string pelicula;
        public int sala;
        public string hora;

        public Sesiones()
        {
        }

        public Sesiones(int idSesion, string pelicula, int sala, string hora)
        {
            this.idSesion = idSesion;
            this.pelicula = pelicula;
            this.sala = sala;
            this.hora = hora;
        }

        public int IdSesion { get => idSesion; }
        public string Pelicula { get => pelicula; }
        public int Sala { get => sala; }
        public string Hora { get => hora; }



    }
}
