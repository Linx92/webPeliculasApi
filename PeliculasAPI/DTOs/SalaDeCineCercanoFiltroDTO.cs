using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-180, 180)]
        public double Longitud { get; set; }
        [Range(-90, 90)]
        public double Latitud { get; set; }

        private int distanciaEnKms { get; set; } = 10;
        private int distanciaMaximaEnKms { get; set; } = 50;
        public int DistanciaEnKms 
        {
            get { return distanciaEnKms; }
            set { distanciaEnKms = value > distanciaMaximaEnKms ? distanciaMaximaEnKms : value; }
        }
    }
}
