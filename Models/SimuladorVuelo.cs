using System;

namespace Proyecto2.Models
{
    // Clase auxiliar interna para llevar el rastro de cada dron
    public class EstadoDron
    {
        public string Nombre { get; set; }
        public int AlturaActual { get; set; }
        public int TiempoDisponible { get; set; } 
        public EstadoDron Siguiente { get; set; }

        public EstadoDron(string nombre)
        {
            Nombre = nombre;
            AlturaActual = 0; // Todos empiezan en el suelo (0 metros)
            TiempoDisponible = 0; // Todos están libres en el segundo 0
        }
    }

    public class SimuladorVuelo
    {
        // Este método devuelve el tiempo óptimo total en segundos
        public int CalcularTiempoOptimo(Mensaje mensaje, SistemaDrones sistema)
        {
            int tiempoUltimaEmision = 0;
            EstadoDron cabezaEstados = InicializarEstados(sistema);

            NodoInstruccion actualInstruccion = mensaje.Instrucciones.Cabeza;

            // Recorremos cada letra del mensaje que queremos enviar
            while (actualInstruccion != null)
            {
                string dronObjetivo = actualInstruccion.Datos.NombreDron;
                int alturaObjetivo = actualInstruccion.Datos.AlturaObjetivo;

                EstadoDron estado = BuscarEstado(cabezaEstados, dronObjetivo);
                if (estado != null)
                {
                    // 1. ¿Cuánto tiempo le toma moverse físicamente a la altura?
                    int tiempoMovimiento = Math.Abs(alturaObjetivo - estado.AlturaActual);
                    
                    // 2. ¿En qué segundo está posicionado y listo para brillar?
                    int tiempoListoParaEmitir = estado.TiempoDisponible + tiempoMovimiento;

                    // 3. Calculamos el segundo exacto en que emitirá luz
                    // Debe ser cuando esté posicionado Y después de la última luz emitida
                    int tiempoEmisionLuz = Math.Max(tiempoListoParaEmitir, tiempoUltimaEmision) + 1;

                    // 4. Actualizamos el rastro del dron y del sistema
                    estado.AlturaActual = alturaObjetivo;
                    estado.TiempoDisponible = tiempoEmisionLuz; 
                    tiempoUltimaEmision = tiempoEmisionLuz;
                }

                actualInstruccion = actualInstruccion.Siguiente;
            }

            return tiempoUltimaEmision; // Este es el tiempo óptimo final
        }

        // --- MÉTODOS AUXILIARES (Pura memoria dinámica) ---
        private EstadoDron InicializarEstados(SistemaDrones sistema)
        {
            EstadoDron cabeza = null;
            NodoDronSistema actual = sistema.Contenido.Cabeza;
            while (actual != null)
            {
                EstadoDron nuevo = new EstadoDron(actual.Datos.NombreDron);
                nuevo.Siguiente = cabeza;
                cabeza = nuevo;
                actual = actual.Siguiente;
            }
            return cabeza;
        }

        private EstadoDron BuscarEstado(EstadoDron cabeza, string nombre)
        {
            EstadoDron actual = cabeza;
            while (actual != null)
            {
                if (actual.Nombre == nombre) return actual;
                actual = actual.Siguiente;
            }
            return null;
        }
    }
}