using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ut7_actividad
{
    public class Peliculas
    {
        public int idPelicula;
        public string titulo;
        public string cartel;
        public int año;
        public string genero;
        public string calificacion;

        public Peliculas()
        {
        }

        public Peliculas(int idPelicula, string titulo, string cartel, int año, string genero, string calificacion)
        {
            this.idPelicula = idPelicula;
            this.titulo = titulo;
            this.cartel = cartel;
            this.año = año;
            this.genero = genero;
            this.calificacion = calificacion;
        }

        public int IdPelicula { get => idPelicula; }

        public string Titulo { get => titulo; }

        public string Cartel { get => cartel; }

        public int Año { get => año; }

        public string Genero { get => genero; }

        public string Calificacion { get => calificacion; }

    }
}
