namespace Proyecto2.Models
{
    public class NodoSistema
    {
        public SistemaDrones Datos { get; set; }
        public NodoSistema Siguiente { get; set; }

        public NodoSistema(SistemaDrones datos)
        {
            Datos = datos;
            Siguiente = null;
        }
    }
}