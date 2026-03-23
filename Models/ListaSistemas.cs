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
        public string GenerarGrafoDOT()
        {
            // Si la lista está vacía, retornamos un grafo básic
            if (Cabeza == null) return "digraph G { nodo [label=\"Sin sistemas cargados\"]; }";

            string dot = "digraph G {\n";
            // Configuramos el diseño: de izquierda a derecha y con nodos bonitos
            dot += "  rankdir=LR;\n";
            dot += "  node [fontname=\"Arial\", shape=box, style=filled, color=lightblue];\n";

            NodoSistema actualSis = Cabeza;
            int idSis = 0;

            // 1. Recorremos los Sistemas
            while (actualSis != null)
            {
                string nombreNodoSis = $"Sis_{idSis}";
                dot += $"  {nombreNodoSis} [label=\"Sistema: {actualSis.Datos.Nombre}\\nAltura Max: {actualSis.Datos.AlturaMaxima}m\\nDrones: {actualSis.Datos.CantidadDrones}\", shape=folder, style=filled, fillcolor=lightcoral, fontcolor=black];\n";
                // 2. Recorremos los Drones dentro del Sistema
                NodoDronSistema actualDron = actualSis.Datos.Contenido.Cabeza;
                int idDron = 0;
                while (actualDron != null)
                {
                    string nombreNodoDron = $"Dron_{idSis}_{idDron}";
                    dot += $"  {nombreNodoDron} [label=\"{actualDron.Datos.NombreDron}\", shape=component, style=filled, fillcolor=lightgreen, fontcolor=white];\n"; //letra blanca
                    dot += $"  {nombreNodoSis} -> {nombreNodoDron};\n"; // Conectamos Sistema con Dron

                    // 3. Recorremos las Alturas y Letras dentro del Dron
                    NodoAltura actualAltura = actualDron.Datos.Alturas.Cabeza;
                    int idAlt = 0;
                    while (actualAltura != null)
                    {
                        string nombreNodoAlt = $"Alt_{idSis}_{idDron}_{idAlt}";
                        dot += $"  {nombreNodoAlt} [label=\"Altura: {actualAltura.Datos.ValorAltura}m\\nLetra: '{actualAltura.Datos.Letra}'\", shape=ellipse, style=filled, fillcolor=lightyellow, fontcolor=black];\n";
                        dot += $"  {nombreNodoDron} -> {nombreNodoAlt};\n"; // Conectamos Dron con Altura

                        actualAltura = actualAltura.Siguiente;
                        idAlt++;
                    }
                    actualDron = actualDron.Siguiente;
                    idDron++;
                }
                actualSis = actualSis.Siguiente;
                idSis++;
            }

            dot += "}";
            return dot;
        }
    }
}