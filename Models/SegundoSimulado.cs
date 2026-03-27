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
        //Grafica para instrucciones mensajes utilizando graphviz
        public string GenerarGrafoDOT()
        {
            // Si la simulación está vacía, retornamos un grafo básico
            if (Cabeza == null) return "digraph G { nodo [label=\"Sin instrucciones\"]; }";

            string dot = "digraph G {\n";
            // rankdir=TB significa "Top to Bottom" (De arriba hacia abajo)
            dot += "  rankdir=TB;\n";
            // Configuramos nodos con fuente legible y fondo pastel
            dot += "  node [fontname=\"Arial\", shape=box, style=filled, fillcolor=lightcyan, fontcolor=black];\n";

            NodoSegundo actual = Cabeza;

            while (actual != null)
            {
                string nombreNodo = $"T_{actual.Datos.TiempoSegundo}";

                // El label mostrará el Segundo y luego la lista de acciones de los drones
                // Usamos \\n para que Graphviz haga un salto de línea dentro de la cajita
                string label = $"TIEMPO: {actual.Datos.TiempoSegundo}s";

                NodoAccion actualAccion = actual.Datos.AccionesDrones.Cabeza;
                while (actualAccion != null)
                {
                    // Agregamos el dron y su acción (Ej: "DronDelAux: Subir")
                    label += $"\\n{actualAccion.Datos.NombreDron}: {actualAccion.Datos.Accion}";
                    actualAccion = actualAccion.Siguiente;
                }

                // Dibujamos la cajita de este segundo
                dot += $"  {nombreNodo} [label=\"{label}\"];\n";

                // Si hay un segundo siguiente, dibujamos la flecha que los conecta
                if (actual.Siguiente != null)
                {
                    string siguienteNodo = $"T_{actual.Siguiente.Datos.TiempoSegundo}";
                    dot += $"  {nombreNodo} -> {siguienteNodo} [color=blue, penwidth=2];\n";
                }

                actual = actual.Siguiente;
            }

            dot += "}";
            return dot;
        }
    }

    
    
}