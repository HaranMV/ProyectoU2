using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace U2_Proyecto.Models
{
    public class Episodio
    {
        public string NombreEpisodio { get; set; } = "";
        public byte NumeroEpisodio { get; set; }
        public string Descripcion { get; set; } = "";
        public int Duracion { get; set; }
        public string URLImagenEpisodio { get; set; } = "";
        public byte numerotemporada { get; set; }   


    }
}
