namespace Proyecto2.Models
{
    public static class DatosGlobales
    {
        // Esta instancia estática vivirá mientras el servidor esté corriendo
        public static ProcesadorXML SistemaPrincipal = new ProcesadorXML();
        
        // Función rápida para el requerimiento de Inicialización
        public static void ReiniciarSistema()
        {
            SistemaPrincipal = new ProcesadorXML();
        }
    }
}