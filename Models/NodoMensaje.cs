namespace Proyecto2.Models
{
    public class NodoMensaje
    {
        public Mensaje Datos { get; set; }
        public NodoMensaje Siguiente { get; set; }

        public NodoMensaje(Mensaje datos)
        {
            Datos = datos;
            Siguiente = null;
        }
    }
}