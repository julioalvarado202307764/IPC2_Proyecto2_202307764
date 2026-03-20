namespace Proyecto2.Models
{
    public class NodoDron
    {
        public Dron DronInfo { get; set; }
        public NodoDron Siguiente { get; set; } // Puntero al siguiente elemento

        public NodoDron(Dron dronInfo)
        {
            DronInfo = dronInfo;
            Siguiente = null; // Siempre empieza apuntando a nulo
        }
    }
}