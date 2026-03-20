namespace Proyecto2.Models
{
    public class ListaDrones
    {
        public NodoDron Cabeza { get; set; }

        public ListaDrones()
        {
            Cabeza = null;
        }

        // Método para agregar un nuevo dron en orden alfabético y validar que sea único
        public bool InsertarOrdenado(Dron nuevoDron)
        {
            // 1. Validar que el nombre sea único
            if (ExisteDron(nuevoDron.Nombre))
            {
                // Retornamos false para indicarle al Controlador/Vista que hubo un error
                return false;
            }

            NodoDron nuevoNodo = new NodoDron(nuevoDron);

            // 2. Caso A: La lista está vacía o el nuevo dron va ANTES que la cabeza actual
            // string.Compare devuelve menor a 0 si la primera palabra va antes en el alfabeto
            if (Cabeza == null || string.Compare(nuevoDron.Nombre, Cabeza.DronInfo.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nuevoNodo.Siguiente = Cabeza; // El nuevo nodo apunta a la antigua cabeza
                Cabeza = nuevoNodo;           // La cabeza ahora es el nuevo nodo
                return true;
            }

            // 3. Caso B: Buscar la posición correcta en el medio o al final
            NodoDron actual = Cabeza;

            // Avanzamos mientras el siguiente nodo no sea nulo Y el nombre del siguiente
            // nodo siga siendo alfabéticamente "menor" que el dron que queremos insertar
            while (actual.Siguiente != null &&
                   string.Compare(nuevoDron.Nombre, actual.Siguiente.DronInfo.Nombre, StringComparison.OrdinalIgnoreCase) > 0)
            {
                actual = actual.Siguiente;
            }

            // Insertamos el nuevo nodo entre 'actual' y 'actual.Siguiente'
            nuevoNodo.Siguiente = actual.Siguiente;
            actual.Siguiente = nuevoNodo;

            return true; // Inserción exitosa
        }

        // Método utilitario para ver si la lista está vacía
        public bool EstaVacia()
        {
            return Cabeza == null;
        }

        //METODO PARA NO REPETIR DRONES

        public bool ExisteDron(string nombre)
        {
            NodoDron actual = Cabeza;

            // Recorremos nodo por nodo
            while (actual != null)
            {
                // Si encontramos un nombre igual, retornamos true (sí existe)
                if (actual.DronInfo.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                actual = actual.Siguiente;
            }

            // Si terminamos de recorrer y no lo encontramos
            return false;
        }
    }

}

