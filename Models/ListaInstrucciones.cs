namespace Proyecto2.Models
{
    public class ListaInstrucciones
    {
        public NodoInstruccion Cabeza { get; set; }

        public ListaInstrucciones()
        {
            Cabeza = null;
        }

        public void Insertar(Instruccion nuevosDatos)
        {
            NodoInstruccion nuevoNodo = new NodoInstruccion { Datos = nuevosDatos, Siguiente = null };

            if (Cabeza == null)
            {
                Cabeza = nuevoNodo;
            }
            else
            {
                NodoInstruccion actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
        }
    }
}