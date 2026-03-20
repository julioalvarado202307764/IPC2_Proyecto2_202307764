namespace Proyecto2.Models
{
    public class AlturaLetra
    {
        public int ValorAltura { get; set; }
        public string Letra { get; set; }

        public AlturaLetra(int valor, string letra)
        {
            ValorAltura = valor;
            Letra = letra;
        }
    }

    public class NodoAltura
    {
        public AlturaLetra Datos { get; set; }
        public NodoAltura Siguiente { get; set; }
        // Constructor omitido para ahorrar espacio, es igual que los anteriores
    }
    
    // Deberás crear una clase 'ListaAlturas' para manejar estos nodos.
}