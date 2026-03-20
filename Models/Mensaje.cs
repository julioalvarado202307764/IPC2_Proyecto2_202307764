namespace Proyecto2.Models
{
    public class Mensaje
    {
        public string Nombre { get; set; }
        public string SistemaDronesRequerido { get; set; }
        public ListaInstrucciones Instrucciones { get; set; } // La lista de pasos a seguir

        public Mensaje(string nombre, string sistemaDronesRequerido)
        {
            Nombre = nombre;
            SistemaDronesRequerido = sistemaDronesRequerido;
            Instrucciones = new ListaInstrucciones();
        }
    }

    // Terminarías con un NodoMensaje y una ListaMensajes global.
}