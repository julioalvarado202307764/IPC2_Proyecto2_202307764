namespace Proyecto2.Models
{
    public class ListaDrones
    {
        public NodoDron Cabeza { get; set; }

        public ListaDrones()
        {
            Cabeza = null;
        }

        // Método para agregar un nuevo dron al final de la lista
        public void Insertar(Dron nuevoDron)
        {
            NodoDron nuevoNodo = new NodoDron(nuevoDron);

            if (Cabeza == null)
            {
                // Si la lista está vacía, el nuevo nodo es la cabeza
                Cabeza = nuevoNodo;
            }
            else
            {
                // Si ya hay elementos, recorremos hasta el final
                NodoDron actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                // Enlazamos el último nodo con el nuevo
                actual.Siguiente = nuevoNodo;
            }
        }
        
        // Método utilitario para ver si la lista está vacía
        public bool EstaVacia()
        {
            return Cabeza == null;
        }
    }
}