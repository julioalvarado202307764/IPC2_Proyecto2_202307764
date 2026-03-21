using System;
using System.Xml;

namespace Proyecto2.Models
{
    public class ProcesadorXML
    {
        // Estas son tus listas globales donde guardaremos todo
        public ListaDrones DronesGlobales { get; set; }
        // Nota: Asumo que ya creaste ListaSistemas y ListaMensajes con su método Insertar()
         public ListaSistemas SistemasGlobales { get; set; } 
         public ListaMensajes MensajesGlobales { get; set; }

        public ProcesadorXML()
        {
            DronesGlobales = new ListaDrones();
             SistemasGlobales = new ListaSistemas();
             MensajesGlobales = new ListaMensajes();
        }

        public void CargarDatosDesdeXML(string rutaArchivo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(rutaArchivo); // Carga el archivo en memoria

            // --- 1. LEER DRONES --- 
            XmlNodeList nodosDrones = doc.SelectNodes("//config/listaDrones/dron");
            if (nodosDrones != null)
            {
                foreach (XmlNode nodo in nodosDrones)
                {
                    string nombreDron = nodo.InnerText.Trim();
                    DronesGlobales.InsertarOrdenado(new Dron(nombreDron));
                }
            }

            // --- 2. LEER SISTEMAS DE DRONES --- 
            XmlNodeList nodosSistemas = doc.SelectNodes("//config/listaSistemasDrones/sistemaDrones");
            if (nodosSistemas != null)
            {
                foreach (XmlNode nodoSistema in nodosSistemas)
                {
                    string nombreSistema = nodoSistema.Attributes["nombre"].Value;
                    int alturaMax = int.Parse(nodoSistema["alturaMaxima"].InnerText);
                    int cantDrones = int.Parse(nodoSistema["cantidadDrones"].InnerText);

                    SistemaDrones nuevoSistema = new SistemaDrones(nombreSistema, alturaMax, cantDrones);

                    // Leer el contenido de este sistema 
                    XmlNodeList nodosContenido = nodoSistema.SelectNodes("contenido/dron");
                    if (nodosContenido != null)
                    {
                        foreach (XmlNode nodoDronContenido in nodosContenido)
                        {
                            string nombreDronContenido = nodoDronContenido.InnerText.Trim();
                            DronEnSistema nuevoDronEnSistema = new DronEnSistema(nombreDronContenido);

                            // Leer las alturas de este dron 
                            XmlNodeList nodosAlturas = nodoDronContenido.NextSibling.SelectNodes("altura");
                            foreach (XmlNode nodoAltura in nodosAlturas)
                            {
                                int valorAltura = int.Parse(nodoAltura.Attributes["valor"].Value);
                                string letra = nodoAltura.InnerText.Trim();

                                nuevoDronEnSistema.Alturas.Insertar(new AlturaLetra(valorAltura, letra));
                            }

                            nuevoSistema.Contenido.Insertar(nuevoDronEnSistema);
                        }
                    }
                     SistemasGlobales.Insertar(nuevoSistema); // Descomentar cuando tengas ListaSistemas
                }
            }

            // --- 3. LEER MENSAJES --- 
            XmlNodeList nodosMensajes = doc.SelectNodes("//config/listaMensajes/Mensaje");
            if (nodosMensajes != null)
            {
                foreach (XmlNode nodoMensaje in nodosMensajes)
                {
                    string nombreMensaje = nodoMensaje.Attributes["nombre"].Value;
                    string sistemaAsociado = nodoMensaje["sistemaDrones"].InnerText.Trim();

                    Mensaje nuevoMensaje = new Mensaje(nombreMensaje, sistemaAsociado);

                    // Leer instrucciones 
                    XmlNodeList nodosInstrucciones = nodoMensaje.SelectNodes("instrucciones/instruccion");
                    if (nodosInstrucciones != null)
                    {
                        foreach (XmlNode inst in nodosInstrucciones)
                        {
                            string dronInstruccion = inst.Attributes["dron"].Value;
                            int alturaInstruccion = int.Parse(inst.InnerText.Trim());

                            nuevoMensaje.Instrucciones.Insertar(new Instruccion(dronInstruccion, alturaInstruccion));
                        }
                    }
                    MensajesGlobales.InsertarOrdenado(nuevoMensaje); // Descomentar cuando tengas ListaMensajes
                }
            }
        }
    }
}