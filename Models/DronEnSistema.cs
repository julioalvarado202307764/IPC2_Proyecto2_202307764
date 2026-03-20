namespace Proyecto2.Models
{
    public class DronEnSistema
    {
        public string NombreDron { get; set; }
        public ListaAlturas Alturas { get; set; } // ¡Una lista enlazada dentro de nuestro objeto!

        public DronEnSistema(string nombreDron)
        {
            NombreDron = nombreDron;
            Alturas = new ListaAlturas(); // Inicializamos la sub-lista vacía
        }
    }

    public class NodoDronSistema
    {
        public DronEnSistema Datos { get; set; }
        public NodoDronSistema Siguiente { get; set; }
    }

    // Deberás crear una clase 'ListaDronesSistema' para manejar estos nodos.
}