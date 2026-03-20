namespace Proyecto2.Models
{
    public class SistemaDrones
    {
        public string Nombre { get; set; }
        public int AlturaMaxima { get; set; }
        public int CantidadDrones { get; set; }
        public ListaDronesSistema Contenido { get; set; } // Lista de listas

        public SistemaDrones(string nombre, int alturaMaxima, int cantidadDrones)
        {
            Nombre = nombre;
            AlturaMaxima = alturaMaxima;
            CantidadDrones = cantidadDrones;
            Contenido = new ListaDronesSistema(); 
        }
    }
    
    // Y finalmente, crearás NodoSistema y ListaSistemas para guardar todos los sistemas del XML.
}