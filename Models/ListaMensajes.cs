using System;

namespace Proyecto2.Models
{
    public class ListaMensajes
    {
        public NodoMensaje Cabeza { get; set; }

        public ListaMensajes()
        {
            Cabeza = null;
        }

        public void InsertarOrdenado(Mensaje nuevoMensaje)
        {
            NodoMensaje nuevoNodo = new NodoMensaje(nuevoMensaje);

            // Si está vacía o va antes que la cabeza
            if (Cabeza == null || string.Compare(nuevoMensaje.Nombre, Cabeza.Datos.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nuevoNodo.Siguiente = Cabeza;
                Cabeza = nuevoNodo;
                return;
            }

            // Buscar su lugar
            NodoMensaje actual = Cabeza;
            while (actual.Siguiente != null &&
                   string.Compare(nuevoMensaje.Nombre, actual.Siguiente.Datos.Nombre, StringComparison.OrdinalIgnoreCase) > 0)
            {
                actual = actual.Siguiente;
            }

            nuevoNodo.Siguiente = actual.Siguiente;
            actual.Siguiente = nuevoNodo;
        }
        
        
    }
}