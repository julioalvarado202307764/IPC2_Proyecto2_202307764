using System;

namespace Proyecto2.Models
{
    // Clase auxiliar para llevar el rastro matemático de cada dron
    public class EstadoDron
    {
        public string Nombre { get; set; }
        public int AlturaActual { get; set; }
        public int TiempoDisponible { get; set; } 
        public EstadoDron Siguiente { get; set; }

        public EstadoDron(string nombre)
        {
            Nombre = nombre;
            AlturaActual = 0; 
            TiempoDisponible = 0; 
        }
    }

    public class SimuladorVuelo
    {
        // 1. MÉTODO PRINCIPAL: Genera la línea de tiempo completa
        public ListaSegundos GenerarSimulacion(Mensaje mensaje, SistemaDrones sistema)
        {
            // Primero, descubrimos cuántos segundos durará en total la simulación
            int tiempoOptimo = CalcularTiempoOptimo(mensaje, sistema);
            
            ListaSegundos lineaDeTiempo = new ListaSegundos();

            // PASO A: Crear la línea de tiempo vacía y llenarla de "Esperar"
            for (int i = 1; i <= tiempoOptimo; i++)
            {
                SegundoSimulado nuevoSegundo = new SegundoSimulado(i);
                NodoDronSistema actualDron = sistema.Contenido.Cabeza;
                
                while (actualDron != null)
                {
                    // Por defecto, todos los drones esperan en este segundo
                    nuevoSegundo.AccionesDrones.Insertar(new AccionDron(actualDron.Datos.NombreDron, "Esperar"));
                    actualDron = actualDron.Siguiente;
                }
                lineaDeTiempo.Insertar(nuevoSegundo);
            }

            // PASO B: Reescribir la historia con los movimientos reales
            int tiempoUltimaEmision = 0;
            EstadoDron cabezaEstados = InicializarEstados(sistema);
            NodoInstruccion actualInst = mensaje.Instrucciones.Cabeza;

            while (actualInst != null)
            {
                string dronObj = actualInst.Datos.NombreDron;
                int altObj = actualInst.Datos.AlturaObjetivo;
                EstadoDron estado = BuscarEstado(cabezaEstados, dronObj);

                if (estado != null)
                {
                    int tiempoInicio = estado.TiempoDisponible;
                    int tiempoLlegada = tiempoInicio + Math.Abs(altObj - estado.AlturaActual);
                    int tiempoEmision = Math.Max(tiempoLlegada, tiempoUltimaEmision) + 1;

                    // Reescribir Fase 1: Movimiento físico (Subir o Bajar)
                    int tiempoActual = tiempoInicio + 1;
                    while (tiempoActual <= tiempoLlegada)
                    {
                        string accion = (altObj > estado.AlturaActual) ? "Subir" : "Bajar";
                        ModificarAccion(lineaDeTiempo, tiempoActual, dronObj, accion);
                        tiempoActual++;
                    }

                    // Reescribir Fase 2: Emitir Luz
                    ModificarAccion(lineaDeTiempo, tiempoEmision, dronObj, "Emitir luz");

                    // Actualizar el estado interno del dron
                    estado.AlturaActual = altObj;
                    estado.TiempoDisponible = tiempoEmision;
                    tiempoUltimaEmision = tiempoEmision;
                }
                actualInst = actualInst.Siguiente;
            }

            return lineaDeTiempo;
        }

        // 2. MÉTODO MATEMÁTICO: Calcula solo el número del tiempo óptimo
        public int CalcularTiempoOptimo(Mensaje mensaje, SistemaDrones sistema)
        {
            int tiempoUltimaEmision = 0;
            EstadoDron cabezaEstados = InicializarEstados(sistema);
            NodoInstruccion actualInstruccion = mensaje.Instrucciones.Cabeza;

            while (actualInstruccion != null)
            {
                EstadoDron estado = BuscarEstado(cabezaEstados, actualInstruccion.Datos.NombreDron);
                if (estado != null)
                {
                    int tiempoLlegada = estado.TiempoDisponible + Math.Abs(actualInstruccion.Datos.AlturaObjetivo - estado.AlturaActual);
                    int tiempoEmisionLuz = Math.Max(tiempoLlegada, tiempoUltimaEmision) + 1;
                    
                    estado.AlturaActual = actualInstruccion.Datos.AlturaObjetivo;
                    estado.TiempoDisponible = tiempoEmisionLuz; 
                    tiempoUltimaEmision = tiempoEmisionLuz;
                }
                actualInstruccion = actualInstruccion.Siguiente;
            }
            return tiempoUltimaEmision;
        }

        // --- MÉTODOS AUXILIARES PARA MANEJAR PUNTEROS ---
        
        // Busca un segundo específico en la línea de tiempo y cambia la acción de un dron
        private void ModificarAccion(ListaSegundos linea, int segundoDestino, string nombreDron, string nuevaAccion)
        {
            NodoSegundo actualSeg = linea.Cabeza;
            while (actualSeg != null)
            {
                if (actualSeg.Datos.TiempoSegundo == segundoDestino)
                {
                    NodoAccion actualAccion = actualSeg.Datos.AccionesDrones.Cabeza;
                    while (actualAccion != null)
                    {
                        if (actualAccion.Datos.NombreDron == nombreDron)
                        {
                            actualAccion.Datos.Accion = nuevaAccion;
                            return; // Encontramos la celda y la cambiamos, salimos rápido
                        }
                        actualAccion = actualAccion.Siguiente;
                    }
                }
                actualSeg = actualSeg.Siguiente;
            }
        }

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