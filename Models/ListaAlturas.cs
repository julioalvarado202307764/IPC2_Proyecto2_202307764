namespace Proyecto2.Models
{
    public class ListaAlturas
    {
        public NodoAltura Cabeza { get; set; }

        public ListaAlturas()
        {
            Cabeza = null;
        }

        public void Insertar(AlturaLetra nuevosDatos)
        {
            // Ojo: Asumiendo que tu NodoAltura no tiene un constructor explícito
            NodoAltura nuevoNodo = new NodoAltura { Datos = nuevosDatos, Siguiente = null };

            if (Cabeza == null)
            {
                Cabeza = nuevoNodo;
            }
            else
            {
                NodoAltura actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
        }
    }
}