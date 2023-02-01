using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ut7_actividad
{
    class Salas
    {
        public int idSala;
        public int capacidad;
        public bool disponible;

        public Salas()
        {
        }

        public Salas(int idSala, int capacidad, bool disponible)
        {
            this.idSala = idSala;
            this.capacidad = capacidad;
            this.disponible = disponible;
        }

        public int IdSala { get => idSala; }

        public int Capacidad { get => capacidad; }

        public bool Disponible { get => disponible; }
    }
}
