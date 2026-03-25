namespace Proyecto2.Models
{//(Lo que hace un dron en un segundo específico)
    public class AccionDron
    {
        public string NombreDron { get; set; }
        public string Accion { get; set; } // "Subir", "Bajar", "Esperar", "Emitir luz"

        public AccionDron(string nombre, string accion)
        {
            NombreDron = nombre;
            Accion = accion;
        }
    }

    public class NodoAccion
    {
        public AccionDron Datos { get; set; }
        public NodoAccion Siguiente { get; set; }
        public NodoAccion(AccionDron datos) { Datos = datos; Siguiente = null; }
    }

    public class ListaAcciones
    {
        public NodoAccion Cabeza { get; set; }
        public void Insertar(AccionDron datos)
        {
            NodoAccion nuevo = new NodoAccion(datos);
            if (Cabeza == null) Cabeza = nuevo;
            else { NodoAccion actual = Cabeza; while (actual.Siguiente != null) actual = actual.Siguiente; actual.Siguiente = nuevo; }
        }
    }
}