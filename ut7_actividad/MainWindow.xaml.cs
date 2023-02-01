using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ut7_actividad
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Peliculas> listaPeliculas = new ObservableCollection<Peliculas>();
        private ObservableCollection<Salas> listaSalas = new ObservableCollection<Salas>();
        private ObservableCollection<Salas> listaSalasDisponibles = new ObservableCollection<Salas>();
        private ObservableCollection<Sesiones> listaSesiones = new ObservableCollection<Sesiones>();

        public MainWindow()
        {
            InitializeComponent();

        }

        public void ConexionBD()
        {
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();

            string sql = File.ReadAllText("../../sql/bbdd_inicio.sql");
            comando.CommandText = sql;
            comando.ExecuteNonQuery();
            sql = File.ReadAllText("../../sql/bbdd_peliculas.sql");
            comando.CommandText = sql;
            comando.ExecuteNonQuery();
            comando.CommandText = "SELECT * FROM peliculas";
            SQLiteDataReader lector = comando.ExecuteReader();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idPelicula = lector.GetInt32(0);
                    string titulo = (string)lector["titulo"];
                    string cartel = (string)lector["cartel"];
                    int año = lector.GetInt32(3);
                    string genero = (string)lector["genero"];
                    string calificacion = (string)lector["calificacion"];

                    listaPeliculas.Add(new Peliculas() { idPelicula = idPelicula, titulo = titulo, cartel = cartel, año = año, genero = genero, calificacion = calificacion });
                }
            }
            lector.Close();
            peliculasDataGrid2.ItemsSource = listaPeliculas;
        }

        private void inicializadorButton_Click(object sender, RoutedEventArgs e)
        {
            ConexionBD();
            inicializadorButton.Visibility = Visibility.Hidden;
            peliculasDataGrid2.Visibility = Visibility.Visible;
            taquillaTab.IsEnabled = true;
            salasTab.IsEnabled = true;
            sesionesTab.IsEnabled = true;
            cargarSalas();
            cargarSesiones();
            ventaPeliculaComboBox.ItemsSource = listaPeliculas;
            ventaPeliculaComboBox.DisplayMemberPath = "Titulo";
            ventaPeliculaComboBox.SelectedItem = 1;
            consultaPeliculaComboBox.ItemsSource = listaPeliculas;
            consultaPeliculaComboBox.DisplayMemberPath = "Titulo";
            consultaPeliculaComboBox.SelectedItem = 1;
            ObservableCollection<int> listaNumeros = new ObservableCollection<int>();
            for (int i = 1; i < 100; i++)
            {
                listaNumeros.Add(i);
            }
            ventaCantidadComboBox.ItemsSource = listaNumeros;

        }

        private void nuevaSalaButton_Click(object sender, RoutedEventArgs e)
        {
            if (capacidadSalaTextBox.Text != "")
            {
                int capacidadIntroducida = int.Parse(capacidadSalaTextBox.Text);
                listaSalas.Clear();
                var conexion = new SQLiteConnection("Data Source=prueba.db");
                conexion.Open();
                var comando = conexion.CreateCommand();
                string s = "INSERT INTO salas (capacidad) VALUES (" + capacidadIntroducida + ")";
                comando.CommandText = s;
                comando.ExecuteNonQuery();
                comando.CommandText = "SELECT * FROM salas";
                SQLiteDataReader lector = comando.ExecuteReader();
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        int idSala = lector.GetInt32(0);
                        int capacidad = lector.GetInt32(1);
                        bool disponible = (bool)lector["disponible"];


                        listaSalas.Add(new Salas() { idSala = idSala, capacidad = capacidad, disponible = disponible });
                    }
                }
                lector.Close();
                salasDataGrid.DataContext = listaSalas;
                salasComboBox.ItemsSource = listaSalas;
                salasComboBox.DisplayMemberPath = "IdSala";
            }
        }

        private void cargarSesiones()
        {
            listaSesiones.Clear();
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT * FROM sesiones";
            SQLiteDataReader lector = comando.ExecuteReader();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idSesion = lector.GetInt32(0);
                    string pelicula = (string)lector["pelicula"];
                    int sala = lector.GetInt32(2);
                    string hora = (string)lector["hora"];


                    listaSesiones.Add(new Sesiones() { idSesion = idSesion, pelicula = pelicula, sala = sala, hora = hora });
                }
            }
            lector.Close();
            sesionesDataGrid.ItemsSource = listaSesiones;

            peliculaComboBox.ItemsSource = listaPeliculas;
            peliculaComboBox.DisplayMemberPath = "Titulo";

            listaDeSalasDisponibles();

            nuevaSesionButton.IsEnabled = false;
        }

        private void listaDeSalasDisponibles()
        {
            listaSalasDisponibles.Clear();
            var conexion2 = new SQLiteConnection("Data Source=prueba.db");
            conexion2.Open();
            var comando2 = conexion2.CreateCommand();
            comando2.CommandText = "SELECT * FROM salas";
            SQLiteDataReader lector2 = comando2.ExecuteReader();
            if (lector2.HasRows)
            {
                while (lector2.Read())
                {
                    int idSala = lector2.GetInt32(0);
                    int capacidad = lector2.GetInt32(1);
                    bool disponible = (bool)lector2["disponible"];

                    if (disponible)
                    {
                        listaSalasDisponibles.Add(new Salas() { idSala = idSala, capacidad = capacidad, disponible = disponible });
                    }
                }
            }
            lector2.Close();

            salaComboBox.ItemsSource = listaSalasDisponibles;
            salaComboBox.DisplayMemberPath = "IdSala";
        }

        private void nuevaSesionButton_Click(object sender, RoutedEventArgs e)
        {
            string peliculaCB = peliculaComboBox.Text;
            int salaCB = int.Parse(salaComboBox.Text);
            string horaCB = horaSesionTimePicker.Text;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            string s = "SELECT COUNT(*) FROM sesiones WHERE sala = " + salaCB + ";";
            comando.CommandText = s;

            int resultado = Convert.ToInt32(comando.ExecuteScalar());
            if (resultado < 3)
            {

                s = "INSERT INTO sesiones (pelicula, sala, hora) VALUES ('" + peliculaCB + "', " + salaCB + ", '" + horaCB + "');";
                comando.CommandText = s;
                comando.ExecuteNonQuery();
                cargarSesiones();
            }
            else
            {
                MessageBox.Show("No pueden haber más de 3 sesiones en una misma sala");
            }
        }

        private void borrarSesionButton_Click(object sender, RoutedEventArgs e)
        {
            string peliculaCB = peliculaComboBox.Text;
            int salaCB = int.Parse(salaComboBox.Text);
            string horaCB = horaSesionTimePicker.Text;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            string s = "DELETE FROM sesiones WHERE(pelicula = '" + peliculaCB + "') AND (sala = " + salaCB + ") AND (hora = '" + horaCB + "');";
            comando.CommandText = s;
            comando.ExecuteNonQuery();
            cargarSesiones();
        }

        private void cargarSalas()
        {
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT * FROM salas";
            SQLiteDataReader lector = comando.ExecuteReader();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idSala = lector.GetInt32(0);
                    int capacidad = lector.GetInt32(1);
                    bool disponible = (bool)lector["disponible"];


                    listaSalas.Add(new Salas() { idSala = idSala, capacidad = capacidad, disponible = disponible });
                }
            }
            lector.Close();
            salasDataGrid.DataContext = listaSalas;
            salasComboBox.ItemsSource = listaSalas;
            salasComboBox.DisplayMemberPath = "IdSala";
            salasHabilitarComboBox.ItemsSource = listaSalas;
            salasHabilitarComboBox.DisplayMemberPath = "IdSala";
            crearButton.IsEnabled = false;
        }

        private void actualizarSalaButton_Click(object sender, RoutedEventArgs e)
        {
            if (nuevaCapacidadSalaTextBox.Text != "" && salasComboBox.Text != "")
            {
                int capacidadIntroducida = int.Parse(nuevaCapacidadSalaTextBox.Text);
                int sala = int.Parse(salasComboBox.Text);
                listaSalas.Clear();
                var conexion = new SQLiteConnection("Data Source=prueba.db");
                conexion.Open();
                var comando = conexion.CreateCommand();
                string s = "UPDATE salas SET capacidad = " + capacidadIntroducida + " WHERE idSala = " + sala + ";";
                comando.CommandText = s;
                comando.ExecuteNonQuery();
                comando.CommandText = "SELECT * FROM salas";
                SQLiteDataReader lector = comando.ExecuteReader();
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        int idSala = lector.GetInt32(0);
                        int capacidad = lector.GetInt32(1);
                        bool disponible = (bool)lector["disponible"];


                        listaSalas.Add(new Salas() { idSala = idSala, capacidad = capacidad, disponible = disponible });
                    }
                }
                lector.Close();
                salasDataGrid.DataContext = listaSalas;
                salasComboBox.ItemsSource = listaSalas;
                salasComboBox.DisplayMemberPath = "IdSala";
                actualizarButton.IsEnabled = false;
            }
        }

        private void habilitarSalaButton_Click(object sender, RoutedEventArgs e)
        {
            if (salasHabilitarComboBox.Text != null)
            {
                int sala = int.Parse(salasHabilitarComboBox.Text);
                bool disponible = true;
                listaSalas.Clear();
                var conexion = new SQLiteConnection("Data Source=prueba.db");
                conexion.Open();
                var comando = conexion.CreateCommand();

                comando.CommandText = "SELECT disponible FROM salas WHERE idSala = " + sala;
                SQLiteDataReader lector = comando.ExecuteReader();
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        disponible = (bool)lector["disponible"];
                    }
                }
                lector.Close();

                if (disponible) disponible = false;
                else disponible = true;
                string s = "UPDATE salas SET disponible = " + disponible + " WHERE idSala = " + sala + ";";
                comando.CommandText = s;
                comando.ExecuteNonQuery();
                comando.CommandText = "SELECT * FROM salas";
                SQLiteDataReader lector2 = comando.ExecuteReader();
                if (lector2.HasRows)
                {
                    while (lector2.Read())
                    {
                        int idSala = lector2.GetInt32(0);
                        int capacidad = lector2.GetInt32(1);
                        bool disponible2 = (bool)lector2["disponible"];


                        listaSalas.Add(new Salas() { idSala = idSala, capacidad = capacidad, disponible = disponible2 });
                    }
                }
                lector2.Close();
                salasDataGrid.DataContext = listaSalas;
                salasHabilitarComboBox.ItemsSource = listaSalas;
                salasHabilitarComboBox.DisplayMemberPath = "IdSala";
                listaDeSalasDisponibles();

                habilitarButton.IsEnabled = false;
            }
        }

        private void activarCrearButton(object sender, TextChangedEventArgs e)
        {
            if (capacidadSalaTextBox.Text != "")
                crearButton.IsEnabled = true;
            else crearButton.IsEnabled = false;
        }
        private void activarActualizarButton(object sender, TextChangedEventArgs e)
        {
            if (nuevaCapacidadSalaTextBox.Text != "" && salasHabilitarComboBox.Text != null)
                actualizarButton.IsEnabled = true;
            else actualizarButton.IsEnabled = false;
        }

        private void salasHabilitarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (salasHabilitarComboBox.Text != null)
                habilitarButton.IsEnabled = true;
            else habilitarButton.IsEnabled = false;
        }

        private void sesionHabilitarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (peliculaComboBox.SelectedItem != null && salaComboBox.SelectedItem != null)
            {
                nuevaSesionButton.IsEnabled = true;
                borrarBDButton.IsEnabled = true;
            }
            else nuevaSesionButton.IsEnabled = false;
        }

        private void horaSesionTimePicker_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }


        private void borrarBDButton_Click(object sender, RoutedEventArgs e)
        {
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "DROP TABLE sesiones;";
            comando.ExecuteNonQuery();
        }


        private void ventaHabilitarBoton(object sender, SelectionChangedEventArgs e)
        {
            if (ventaPeliculaComboBox.SelectedItem != null && ventaHoraComboBox.SelectedItem != null && ventaSalaComboBox.SelectedItem != null && ventaPagoComboBox.SelectedItem != null && ventaCantidadComboBox.SelectedItem != null)
            {
                ventaButton.IsEnabled = true;
            }
            else ventaButton.IsEnabled = false;
        }

        private void ventaPeliculaComboBox_DropDownClosed(object sender, EventArgs e)
        {
            string ventaIdPelicula = ventaPeliculaComboBox.Text;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT * FROM peliculas WHERE titulo = '" + ventaIdPelicula + "';";
            SQLiteDataReader lector = comando.ExecuteReader();
            ObservableCollection<Peliculas> peliculaSeleccionada = new ObservableCollection<Peliculas>();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idPelicula = lector.GetInt32(0);
                    string titulo = (string)lector["titulo"];
                    string cartel = (string)lector["cartel"];
                    int año = lector.GetInt32(3);
                    string genero = (string)lector["genero"];
                    string calificacion = (string)lector["calificacion"];

                    peliculaSeleccionada.Add(new Peliculas() { idPelicula = idPelicula, titulo = titulo, cartel = cartel, año = año, genero = genero, calificacion = calificacion });
                }
            }
            lector.Close();

            comando.CommandText = "SELECT * FROM sesiones WHERE pelicula = '" + ventaIdPelicula + "';";
            lector = comando.ExecuteReader();
            ObservableCollection<Sesiones> sesionesSeleccionadas = new ObservableCollection<Sesiones>();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idSesion = lector.GetInt32(0);
                    string pelicula = (string)lector["pelicula"];
                    int sala = lector.GetInt32(2);
                    string hora = (string)lector["hora"];


                    sesionesSeleccionadas.Add(new Sesiones() { idSesion = idSesion, pelicula = pelicula, sala = sala, hora = hora });
                }
            }

            lector.Close();



            ventaCartelPeliculaSeleccionadaDataGrid.ItemsSource = peliculaSeleccionada;

            ventaHoraComboBox.ItemsSource = sesionesSeleccionadas;
            ventaHoraComboBox.DisplayMemberPath = "Hora";




        }
        private void ventaPeliculaComboBox_DropDownClosed2(object sender, EventArgs e)
        {
            string ventaIdPelicula = ventaPeliculaComboBox.Text;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT * FROM sesiones WHERE pelicula = '" + ventaIdPelicula + "' AND hora = '" + ventaHoraComboBox.Text + "';";
            SQLiteDataReader lector = comando.ExecuteReader();
            ObservableCollection<Sesiones> sesionesSeleccionadas2 = new ObservableCollection<Sesiones>();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idSesion = lector.GetInt32(0);
                    string pelicula = (string)lector["pelicula"];
                    int sala = lector.GetInt32(2);
                    string hora = (string)lector["hora"];


                    sesionesSeleccionadas2.Add(new Sesiones() { idSesion = idSesion, pelicula = pelicula, sala = sala, hora = hora });
                }
            }

            lector.Close();

            ventaSalaComboBox.ItemsSource = sesionesSeleccionadas2;
            ventaSalaComboBox.DisplayMemberPath = "Sala";
        }

        private void ventaButton_Click(object sender, RoutedEventArgs e)
        {
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            string s = "SELECT idSesion FROM sesiones WHERE pelicula = '" + ventaPeliculaComboBox.Text + "' AND sala = " + int.Parse(ventaSalaComboBox.Text) + " AND hora = '" + ventaHoraComboBox.Text + "';";
            comando.CommandText = s;
            int sesionComprada = Convert.ToInt32(comando.ExecuteScalar());

            s = "INSERT INTO ventas (sesion, cantidad, pago) VALUES (" + sesionComprada + ", " + ventaCantidadComboBox.SelectedItem + ", '" + ventaPagoComboBox.Text + "');";
            comando.CommandText = s;
            comando.ExecuteNonQuery();

            ventaPeliculaComboBox.SelectedValue = null;
            ventaHoraComboBox.SelectedValue = null;
            ventaSalaComboBox.SelectedValue = null;
            ventaPagoComboBox.SelectedValue = null;
            ventaCantidadComboBox.SelectedValue = null;
            ventaCartelPeliculaSeleccionadaDataGrid.ItemsSource = null;



        }
        private void consultaButton_Click(object sender, RoutedEventArgs e)
        {
            consultaPeliculaComboBox.SelectedValue = null;
            consultaHoraComboBox.SelectedValue = null;
            consultaSalaComboBox.SelectedValue = null;
            consultaButacasRestantesTextBlock.Text = "-";



        }
        private void consultaPeliculaComboBox_DropDownClosed(object sender, EventArgs e)
        {
            string ventaIdPelicula = consultaPeliculaComboBox.Text;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT * FROM peliculas WHERE titulo = '" + ventaIdPelicula + "';";
            SQLiteDataReader lector = comando.ExecuteReader();
            ObservableCollection<Peliculas> peliculaSeleccionada = new ObservableCollection<Peliculas>();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idPelicula = lector.GetInt32(0);
                    string titulo = (string)lector["titulo"];
                    string cartel = (string)lector["cartel"];
                    int año = lector.GetInt32(3);
                    string genero = (string)lector["genero"];
                    string calificacion = (string)lector["calificacion"];

                    peliculaSeleccionada.Add(new Peliculas() { idPelicula = idPelicula, titulo = titulo, cartel = cartel, año = año, genero = genero, calificacion = calificacion });
                }
            }
            lector.Close();

            comando.CommandText = "SELECT * FROM sesiones WHERE pelicula = '" + ventaIdPelicula + "';";
            lector = comando.ExecuteReader();
            ObservableCollection<Sesiones> sesionesSeleccionadas = new ObservableCollection<Sesiones>();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idSesion = lector.GetInt32(0);
                    string pelicula = (string)lector["pelicula"];
                    int sala = lector.GetInt32(2);
                    string hora = (string)lector["hora"];


                    sesionesSeleccionadas.Add(new Sesiones() { idSesion = idSesion, pelicula = pelicula, sala = sala, hora = hora });
                }
            }

            lector.Close();



            consultaCartelPeliculaSeleccionadaDataGrid.ItemsSource = peliculaSeleccionada;

            consultaHoraComboBox.ItemsSource = sesionesSeleccionadas;
            consultaHoraComboBox.DisplayMemberPath = "Hora";


        }
        private void consultaPeliculaComboBox_DropDownClosed2(object sender, EventArgs e)
        {
            string ventaIdPelicula = consultaPeliculaComboBox.Text;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT * FROM sesiones WHERE pelicula = '" + ventaIdPelicula + "' AND hora = '" + consultaHoraComboBox.Text + "';";
            SQLiteDataReader lector = comando.ExecuteReader();
            ObservableCollection<Sesiones> sesionesSeleccionadas2 = new ObservableCollection<Sesiones>();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    int idSesion = lector.GetInt32(0);
                    string pelicula = (string)lector["pelicula"];
                    int sala = lector.GetInt32(2);
                    string hora = (string)lector["hora"];


                    sesionesSeleccionadas2.Add(new Sesiones() { idSesion = idSesion, pelicula = pelicula, sala = sala, hora = hora });
                }
            }

            lector.Close();

            consultaSalaComboBox.ItemsSource = sesionesSeleccionadas2;
            consultaSalaComboBox.DisplayMemberPath = "Sala";
        }
        private void consultaCalculoButacasComboBox_DropDownClosed2(object sender, EventArgs e)
        {
            int idSesion=0;
            int capacidadSala=0;
            int sumaVentas = 0;
            var conexion = new SQLiteConnection("Data Source=prueba.db");
            conexion.Open();
            var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT idSesion FROM sesiones WHERE pelicula = '" + consultaPeliculaComboBox.Text + "' AND sala = " + int.Parse(consultaSalaComboBox.Text) + " AND hora = '" + consultaHoraComboBox.Text + "';";
            SQLiteDataReader lector = comando.ExecuteReader();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    idSesion = lector.GetInt32(0);
                }
            }
            lector.Close();
            comando.CommandText = "SELECT capacidad FROM salas WHERE idSala = " + int.Parse(consultaSalaComboBox.Text) + ";";
            lector = comando.ExecuteReader();
            if (lector.HasRows)
            {
                while (lector.Read())
                {
                    capacidadSala = lector.GetInt32(0);
                }
            }
            lector.Close();
            comando.CommandText = "SELECT SUM(cantidad) FROM ventas WHERE sesion = " + idSesion + ";";
            sumaVentas = Convert.ToInt32(comando.ExecuteScalar());

            consultaButacasRestantesTextBlock.Text = (capacidadSala - sumaVentas).ToString();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = System.IO.Path.GetFullPath(@"..\\..\\sql\\AyudaGestordeCine.chm");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = str;
            Process.Start(startInfo);
        }
    }
}
