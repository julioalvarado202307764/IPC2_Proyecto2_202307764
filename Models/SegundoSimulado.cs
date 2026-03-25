namespace Proyecto2.Models
{//(Un segundo completo con las acciones de todos los drones)
    public class SegundoSimulado
    {
        public int TiempoSegundo { get; set; }
        public ListaAcciones AccionesDrones { get; set; }

        public SegundoSimulado(int tiempoSegundo)
        {
            TiempoSegundo = tiempoSegundo;
            AccionesDrones = new ListaAcciones();
        }
    }

    public class NodoSegundo
    {
        public SegundoSimulado Datos { get; set; }
        public NodoSegundo Siguiente { get; set; }
        public NodoSegundo(SegundoSimulado datos) { Datos = datos; Siguiente = null; }
    }

    public class ListaSegundos
    {
        public NodoSegundo Cabeza { get; set; }
        public void Insertar(SegundoSimulado datos)
        {
            NodoSegundo nuevo = new NodoSegundo(datos);
            if (Cabeza == null) Cabeza = nuevo;
            else { NodoSegundo actual = Cabeza; while (actual.Siguiente != null) actual = actual.Siguiente; actual.Siguiente = nuevo; }
        }
    }
}