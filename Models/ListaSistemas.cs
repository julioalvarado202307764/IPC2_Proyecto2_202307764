namespace Proyecto2.Models
{
    public class ListaSistemas
    {
        public NodoSistema Cabeza { get; set; }

        public ListaSistemas()
        {
            Cabeza = null;
        }

        public void Insertar(SistemaDrones nuevosDatos)
        {
            NodoSistema nuevoNodo = new NodoSistema(nuevosDatos);

            if (Cabeza == null)
            {
                Cabeza = nuevoNodo;
            }
            else
            {
                NodoSistema actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
        }
    }
}