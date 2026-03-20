namespace Proyecto2.Models
{
    public class ListaDronesSistema
    {
        public NodoDronSistema Cabeza { get; set; }

        public ListaDronesSistema()
        {
            Cabeza = null;
        }

        public void Insertar(DronEnSistema nuevosDatos)
        {
            NodoDronSistema nuevoNodo = new NodoDronSistema { Datos = nuevosDatos, Siguiente = null };

            if (Cabeza == null)
            {
                Cabeza = nuevoNodo;
            }
            else
            {
                NodoDronSistema actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
        }
    }
}