namespace Proyecto2.Models
{
    public class Instruccion
    {
        public string NombreDron { get; set; }
        public int AlturaObjetivo { get; set; }

        public Instruccion(string nombreDron, int alturaObjetivo)
        {
            NombreDron = nombreDron;
            AlturaObjetivo = alturaObjetivo;
        }
    }

    public class NodoInstruccion
    {
        public Instruccion Datos { get; set; }
        public NodoInstruccion Siguiente { get; set; }
    }
    
    // Se requiere crear una 'ListaInstrucciones'
}