using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using U2_Proyecto.Models;

namespace U2_Proyecto.ViewModels
{
    public enum Vistas { AgregarTemporada, EditarTemporada, EliminarTemporada, ListaTemporada, AgregarEpisodio,
        EditarEpisodio, ElimianarEpisodio, ListaEpisodio,
    }

    public class TemporadaViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Temporada> Temporadas { get; set; } = new();

        public ICommand AgregarTemporadaCommand { get; set; }
        public ICommand VerAgregarTemporadaCommand { get; set; }
        public ICommand EliminarTemporadaCommand { get; set; }
        public ICommand EditarTemporadaCommand { get; set; }
        public ICommand VerEditarTemporadaCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand VerEpisodiosCommand { get; set; }

        public ICommand AgregarEpisodioCommand { get; set; }
        public ICommand VerAgregarEpisodioCommand { get; set; }
        public ICommand EliminarEpisodioCommand { get; set; }
        public ICommand VerEliminarEpisodioCommand { get; set; }
        public ICommand CancelarEpisodioCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public ICommand EditarEpisodioCommand { get; set; }
       
        public Temporada Temporada { get; set; } = null!;

        public string Error { get; set; } = "";

        public Vistas Vista { get; set; } = Vistas.ListaTemporada;

        public ObservableCollection<Episodio> EpisodioObservable { get; set; } = new();
        public List<Episodio> Episodios { get; set; } = new();
        public Episodio Episodio { get; set; } = null!;


        public TemporadaViewModel()
        {
            AgregarTemporadaCommand = new RelayCommand(Agregar);
            EliminarTemporadaCommand = new RelayCommand(Eliminar);
            EditarTemporadaCommand = new RelayCommand(EditarTemporada);
            VerEditarTemporadaCommand = new RelayCommand(VerEditar);
            CancelarCommand = new RelayCommand(Cancelar);
            VerEpisodiosCommand = new RelayCommand(VerEpisodios);

            //Episodios

            AgregarEpisodioCommand = new RelayCommand(AgregarEpisodio);
            CancelarEpisodioCommand = new RelayCommand(CancelarEpisodio);
            CambiarVistaCommand = new RelayCommand<Vistas>(CambiarVistas);
            EditarEpisodioCommand = new RelayCommand(EditarEpisodio);
            EliminarEpisodioCommand = new RelayCommand(EliminarEpisodio);

            Deserializar();
            CambiarVistas(Vistas.ListaTemporada);
        }

        private void EliminarEpisodio()
        {
            if (Vista == Vistas.ElimianarEpisodio)
            {
                if (Episodio != null)
                {
                    Episodios.Remove(Episodio);
                    Serializar();
                    VerEpisodios();
                }
            }
        }

        private void EditarEpisodio()
        {
            Error = "";

            if (Episodio != null)
            {
                if (string.IsNullOrWhiteSpace(Episodio.NombreEpisodio))
                {
                    Error = "Ingrese el nombre del episodio";
                }

                if (Episodio.NumeroEpisodio <= 0)
                {
                    Error = "Ingrese el número de episodio";
                }

                if (Episodio.Duracion <= 0)
                {
                    Error = "Ingrese la duración del episodio";
                }

                if (string.IsNullOrWhiteSpace(Episodio.URLImagenEpisodio) || !Episodio.URLImagenEpisodio.StartsWith("http")
                    || !Episodio.URLImagenEpisodio.EndsWith(".jpg"))
                {
                    Error = "Escribe la dirección de una imgaen jpg";
                }

                if (string.IsNullOrWhiteSpace(Episodio.Descripcion))
                {
                    Error = "Ingrese la descripcion del episodio";
                }

                if (!string.IsNullOrEmpty(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }

                Episodios[posAnterior] = Episodio;
                Serializar();
                VerEpisodios();
                
            }
        }

        private void CambiarVistas(Vistas vistas)
        {
            if (vistas == Vistas.AgregarEpisodio)
            {
                Episodio = new();
                Episodio.numerotemporada = Temporada.NumeroTemporada;
                Error = "";

                Vista = Vistas.AgregarEpisodio;
                Actualizar(nameof(Vista));
                Actualizar(nameof(Error));
            }

            if(vistas == Vistas.EditarEpisodio)
            {
                Error = "";
                if (Episodio != null)
                {
                    posAnterior = Episodios.IndexOf(Episodio);
                    var clon = new Episodio
                    {
                      NombreEpisodio = Episodio.NombreEpisodio,
                      NumeroEpisodio = Episodio.NumeroEpisodio,
                      Descripcion = Episodio.Descripcion,
                      Duracion = Episodio.Duracion,
                      URLImagenEpisodio = Episodio.URLImagenEpisodio,
                      numerotemporada = Episodio.numerotemporada
                    };
                    Episodio = clon;
                    Vista = Vistas.EditarEpisodio;
                    Actualizar(nameof(Vista));
                    Actualizar(nameof(Error));
                }
            }

            if (vistas == Vistas.ElimianarEpisodio)
            {
                Vista = Vistas.ElimianarEpisodio;
                Actualizar(nameof(Vista));
            }
            if (vistas == Vistas.ListaEpisodio)
            {
                Vista = Vistas.ListaEpisodio;
                Actualizar(nameof(Vista)); 
            }


            if(vistas == Vistas.AgregarTemporada)
            {
                Temporada = new();
                Error = "";

                Vista = Vistas.AgregarTemporada;
                Actualizar(nameof(Vista));
                Actualizar(nameof(Error));
            }

            if(vistas == Vistas.ListaTemporada)
            {
                Vista = vistas;
                List<Temporada> lista = Temporadas.OrderBy(x => x.NumeroTemporada).ToList();
                Temporadas.Clear();
                foreach (var item in lista)
                {
                    Temporadas.Add(item);
                }
            }

            if (vistas == Vistas.EditarTemporada)
            {
                Error = "";
                if (Temporada != null)
                {
                    posAnterior = Temporadas.IndexOf(Temporada);
                    var clon = new Temporada
                    {
                        NumeroTemporada = Temporada.NumeroTemporada,
                        InfoTemporada = Temporada.InfoTemporada,
                        NumeroEpisodios = Temporada.NumeroEpisodios,
                        URLTemporada = Temporada.URLTemporada,
                    };
                    Temporada = clon;
                    Vista = Vistas.EditarTemporada;
                    Actualizar(nameof(Vista));
                    Actualizar(nameof(Error));
                }
            }

            if(vistas == Vistas.EliminarTemporada)
            {
                Vista = Vistas.EliminarTemporada;
                Actualizar(nameof(Vista));
            }
        }

        private void CancelarEpisodio()
        {
            Vista = Vistas.ListaEpisodio;
            Actualizar(nameof(Vista));
        }

        private void AgregarEpisodio()
        {
            Error = "";

            if(Episodio != null)
            {
                if (Episodio.Duracion <= 0)
                {
                    Error = "Ingrese la duracion del episodio";
                }
                if (Episodio.NumeroEpisodio <= 0)
                {
                    Error = "Ingrese el numero del episodio";
                }

                if (string.IsNullOrWhiteSpace(Episodio.NombreEpisodio))
                {
                    Error = "Ingrese el nombre del episodio";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Descripcion))
                {
                    Error = "Ingrese la descripción del episodio";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLImagenEpisodio) || !Episodio.URLImagenEpisodio.StartsWith("http")
                    || !Episodio.URLImagenEpisodio.EndsWith(".jpg"))
                {
                    Error = "Escribe la dirección de una imgaen jpg";
                }

                if (!string.IsNullOrEmpty(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }

                if (Error == "")
                {
                    EpisodioObservable.Add(Episodio);
                    Episodios.Add(Episodio);
                    Serializar();
                    VerEpisodios();
                    
                }

            }
        }






        //Temporadas





        int posAnterior;
        private void VerEditar()
        {
            Vista = Vistas.AgregarTemporada;
            Actualizar(nameof(Vista));
        }



        private void VerEpisodios()
        {
            Vista = Vistas.ListaEpisodio;
            List<Episodio> episodios = Episodios.Where(x => x.numerotemporada == Temporada.NumeroTemporada).
                OrderBy(x=> x.NumeroEpisodio).ToList();
            EpisodioObservable.Clear();
            foreach (var item in episodios)
            {
                EpisodioObservable.Add(item);
            }
            Actualizar(nameof(Vista));
        }

        private void Cancelar()
        {
            Temporada = null;
            Vista = Vistas.ListaTemporada;

            Actualizar(nameof(Vista));
        }

        private void EditarTemporada()
        {
            Error = "";

            if (Temporada != null)
            {

                if (Temporada.NumeroTemporada <= 0)
                {
                    Error = "Ingrese el número de temporada";
                }

                if (Temporada.NumeroEpisodios <= 0)
                {
                    Error = "Ingrese el número de episodios";
                }

                if (string.IsNullOrWhiteSpace(Temporada.URLTemporada) || !Temporada.URLTemporada.StartsWith("http")
                    || !Temporada.URLTemporada.EndsWith(".jpg"))
                {
                    Error = "Escribe la dirección de una imgaen jpg";
                }

                if (string.IsNullOrWhiteSpace(Temporada.InfoTemporada))
                {
                    Error = "Ingrese la información de la temporada";
                }

                if (!string.IsNullOrEmpty(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }

                int numero = Temporadas[posAnterior].NumeroTemporada;
                Temporadas[posAnterior] = Temporada;
                foreach (var item in Episodios)
                {
                    if(item.numerotemporada == numero)
                    {
                        item.numerotemporada = Temporada.NumeroTemporada;
                    }
                }
                Serializar();
                CambiarVistas(Vistas.ListaTemporada);
                Actualizar(nameof(Vista));
            }

        }

        private void Eliminar()
        {
            if (Vista == Vistas.EliminarTemporada)
            {
                if (Temporada != null)
                {
                    Temporadas.Remove(Temporada);
                    Serializar();
                    CambiarVistas(Vistas.ListaTemporada);
                    Actualizar(nameof(Vista));
                }
            }
        }

        

        private void Agregar()
        {
            if (Temporada != null)
            {
                Error = "";


                if (Temporada.NumeroTemporada <= 0)
                {
                    Error = "Ingrese el número de temporada";
                }

                if(Temporada.NumeroEpisodios <= 0)
                {
                    Error = "Ingrese el número de episodios";
                }

                if (string.IsNullOrWhiteSpace(Temporada.URLTemporada) || !Temporada.URLTemporada.StartsWith("http")
                    || !Temporada.URLTemporada.EndsWith(".jpg"))
                {
                    Error = "Escribe la dirección de una imgaen jpg";
                }

                if (string.IsNullOrWhiteSpace(Temporada.InfoTemporada)) 
                {
                    Error = "Ingrese la información de la temporada";
                }

                if (!string.IsNullOrEmpty(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Actualizar(nameof(Error));

              
                if(Error == "")
                {
                Temporadas.Add(Temporada);
                Serializar();
                CambiarVistas(Vistas.ListaTemporada);
                Actualizar(nameof(Vista));
               }
            }
        }

        private void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        void Serializar()
        {
            File.WriteAllText("Temporadas.json", JsonSerializer.Serialize(Temporadas));
            File.WriteAllText("Episodios.json", JsonSerializer.Serialize(Episodios));
        }
        void Deserializar()
        {
            if (File.Exists("Temporadas.json"))
            {
                var Temporadas = JsonSerializer.Deserialize<ObservableCollection<Temporada>>(File.ReadAllText("Temporadas.json"));

                if(Temporadas != null)
                {
                    foreach(var t in Temporadas)
                    {
                        this.Temporadas.Add(t);
                    }
                }
            }

            if (File.Exists("Episodios.json"))
            {
                var Episodios = JsonSerializer.Deserialize<List<Episodio>>(File.ReadAllText("Episodios.json"));

                if(Episodios != null)
                {
                    foreach (var e in Episodios)
                    {
                        this.Episodios.Add(e);
                    }
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
