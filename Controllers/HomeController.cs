using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Proyecto2.Models;
using System.Xml;

namespace Proyecto2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost] // Indica que este método recibe datos de un formulario
        public IActionResult CargarArchivo(IFormFile archivoXml)
        {
            if (archivoXml != null && archivoXml.Length > 0)
            {
                // 1. Guardamos el archivo temporalmente en el servidor
                string rutaTemp = Path.GetTempFileName();
                using (var stream = new FileStream(rutaTemp, FileMode.Create))
                {
                    archivoXml.CopyTo(stream);
                }

                // 2. Le pasamos la ruta a nuestro ProcesadorXML estático
                DatosGlobales.SistemaPrincipal.CargarDatosDesdeXML(rutaTemp);

                // 3. Borramos el archivo temporal por limpieza
                System.IO.File.Delete(rutaTemp);

                // 4. Mandamos un mensaje de éxito a la vista
                ViewBag.MensajeExito = "¡Archivo XML cargado y procesado correctamente!";
            }
            else
            {
                ViewBag.MensajeError = "Por favor, selecciona un archivo válido.";
            }

            // Volvemos a mostrar la misma página
            return View("Index");
        }

        // Método extra para cumplir con la Inicialización del sistema
        public IActionResult Inicializar()
        {
            DatosGlobales.ReiniciarSistema();
            ViewBag.MensajeExito = "Sistema inicializado en blanco exitosamente.";
            return View("Index");
        }
        public IActionResult ListadoDrones()
        {
            // Pasamos nuestra ListaDrones directamente a la vista
            return View(DatosGlobales.SistemaPrincipal.DronesGlobales);
        }

        [HttpPost]
        public IActionResult AgregarDron(string nombreDron)
        {
            // 1. Validar que no manden un texto vacío
            if (string.IsNullOrWhiteSpace(nombreDron))
            {
                TempData["MensajeError"] = "El nombre del dron no puede estar vacío.";
                return RedirectToAction("ListadoDrones"); // Recargamos la vista
            }

            // 2. Intentar insertar el dron (recuerda que nuestro método devuelve true/false)
            bool exito = DatosGlobales.SistemaPrincipal.DronesGlobales.InsertarOrdenado(new Dron(nombreDron.Trim()));

            // 3. Evaluar el resultado
            if (exito)
            {
                TempData["MensajeExito"] = $"¡Dron '{nombreDron}' agregado exitosamente!";
            }
            else
            {
                TempData["MensajeError"] = $"Error: El dron '{nombreDron}' ya existe en el sistema.";
            }

            // 4. Volver a cargar la página para ver la tabla actualizada
            return RedirectToAction("ListadoDrones");
        }
        public IActionResult GraficaSistemas()
        {
            // Obtenemos el string con todo el código de Graphviz
            string codigoDot = DatosGlobales.SistemaPrincipal.SistemasGlobales.GenerarGrafoDOT();

            // Lo guardamos en ViewBag para que la vista lo pueda leer
            ViewBag.CodigoDot = codigoDot;

            return View();
        }

        // 1. Mostrar el listado general de mensajes
        public IActionResult ListadoMensajes()
        {
            return View(DatosGlobales.SistemaPrincipal.MensajesGlobales);
        }

        // 2. Método auxiliar privado para decodificar el mensaje original a texto
        private string DecodificarMensaje(Mensaje mensaje, SistemaDrones sistema)
        {
            string textoDecodificado = "";
            NodoInstruccion actualInst = mensaje.Instrucciones.Cabeza;

            while (actualInst != null)
            {
                NodoDronSistema actualDronSis = sistema.Contenido.Cabeza;
                while (actualDronSis != null)
                {
                    if (actualDronSis.Datos.NombreDron == actualInst.Datos.NombreDron)
                    {
                        NodoAltura actualAlt = actualDronSis.Datos.Alturas.Cabeza;
                        while (actualAlt != null)
                        {
                            if (actualAlt.Datos.ValorAltura == actualInst.Datos.AlturaObjetivo)
                            {
                                textoDecodificado += actualAlt.Datos.Letra;
                                break;
                            }
                            actualAlt = actualAlt.Siguiente;
                        }
                        break;
                    }
                    actualDronSis = actualDronSis.Siguiente;
                }
                actualInst = actualInst.Siguiente;
            }
            return textoDecodificado;
        }

        // 3. Generar la simulación detallada de un mensaje específico
        public IActionResult VerInstrucciones(string nombreMensaje)
        {
            // A. Buscar el mensaje en la memoria dinámica
            Mensaje msjEncontrado = null;
            NodoMensaje actualM = DatosGlobales.SistemaPrincipal.MensajesGlobales.Cabeza;
            while (actualM != null)
            {
                if (actualM.Datos.Nombre == nombreMensaje) { msjEncontrado = actualM.Datos; break; }
                actualM = actualM.Siguiente;
            }

            if (msjEncontrado == null) return RedirectToAction("ListadoMensajes");

            // B. Buscar el sistema de drones asociado
            SistemaDrones sisAsociado = null;
            NodoSistema actualS = DatosGlobales.SistemaPrincipal.SistemasGlobales.Cabeza;
            while (actualS != null)
            {
                if (actualS.Datos.Nombre == msjEncontrado.SistemaDronesRequerido) { sisAsociado = actualS.Datos; break; }
                actualS = actualS.Siguiente;
            }

            // C. ¡Usar nuestro super cerebro simulador!
            SimuladorVuelo simulador = new SimuladorVuelo();
            int tiempoOptimo = simulador.CalcularTiempoOptimo(msjEncontrado, sisAsociado);
            ListaSegundos simulacion = simulador.GenerarSimulacion(msjEncontrado, sisAsociado);
            string textoMensaje = DecodificarMensaje(msjEncontrado, sisAsociado);

            // D. Mandar todo empacado a la vista usando ViewBag
            ViewBag.NombreMensaje = msjEncontrado.Nombre;
            ViewBag.NombreSistema = sisAsociado.Nombre;
            ViewBag.TiempoOptimo = tiempoOptimo;
            ViewBag.TextoMensaje = textoMensaje;
            ViewBag.SistemaDrones = sisAsociado; // Lo mandamos para saber las columnas de la tabla

            // Pasamos la línea de tiempo como el modelo principal
            return View(simulacion);
        }
        public IActionResult DescargarXML()
        {
            // 1. Crear el documento XML desde cero
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(xmlDeclaration);

            // Etiqueta Raíz <respuesta>
            XmlElement raiz = doc.CreateElement("respuesta");
            doc.AppendChild(raiz);

            // Etiqueta <listaMensajes>
            XmlElement listaMensajes = doc.CreateElement("listaMensajes");
            raiz.AppendChild(listaMensajes);

            // 2. Recorrer nuestra memoria dinámica de mensajes
            NodoMensaje actualM = DatosGlobales.SistemaPrincipal.MensajesGlobales.Cabeza;
            SimuladorVuelo simulador = new SimuladorVuelo();

            while (actualM != null)
            {
                Mensaje msj = actualM.Datos;

                // Buscar el sistema de drones asociado a este mensaje
                SistemaDrones sis = null;
                NodoSistema actualS = DatosGlobales.SistemaPrincipal.SistemasGlobales.Cabeza;
                while (actualS != null)
                {
                    if (actualS.Datos.Nombre == msj.SistemaDronesRequerido) { sis = actualS.Datos; break; }
                    actualS = actualS.Siguiente;
                }

                if (sis != null)
                {
                    // Ejecutar la simulación para obtener los datos
                    int tiempoOptimo = simulador.CalcularTiempoOptimo(msj, sis);
                    ListaSegundos lineaTiempo = simulador.GenerarSimulacion(msj, sis);
                    string msjDecodificado = DecodificarMensaje(msj, sis);

                    // <mensaje nombre="nombreMensaje">
                    XmlElement nodoMensaje = doc.CreateElement("mensaje");
                    nodoMensaje.SetAttribute("nombre", msj.Nombre);
                    listaMensajes.AppendChild(nodoMensaje);

                    // <sistemaDrones>
                    XmlElement nodoSis = doc.CreateElement("sistemaDrones");
                    nodoSis.InnerText = sis.Nombre;
                    nodoMensaje.AppendChild(nodoSis);

                    // <tiempoOptimo>
                    XmlElement nodoTiempoOp = doc.CreateElement("tiempoOptimo");
                    nodoTiempoOp.InnerText = tiempoOptimo.ToString();
                    nodoMensaje.AppendChild(nodoTiempoOp);

                    // <mensajeRecibido>
                    XmlElement nodoMsjRec = doc.CreateElement("mensajeRecibido");
                    nodoMsjRec.InnerText = msjDecodificado;
                    nodoMensaje.AppendChild(nodoMsjRec);

                    // <instrucciones>
                    XmlElement nodoInstrucciones = doc.CreateElement("instrucciones");
                    nodoMensaje.AppendChild(nodoInstrucciones);

                    // Recorrer la línea de tiempo segundo a segundo
                    NodoSegundo actualSeg = lineaTiempo.Cabeza;
                    while (actualSeg != null)
                    {
                        // <tiempo valor="X">
                        XmlElement nodoTiempo = doc.CreateElement("tiempo");
                        nodoTiempo.SetAttribute("valor", actualSeg.Datos.TiempoSegundo.ToString());
                        nodoInstrucciones.AppendChild(nodoTiempo);

                        // <acciones>
                        XmlElement nodoAcciones = doc.CreateElement("acciones");
                        nodoTiempo.AppendChild(nodoAcciones);

                        // Recorrer qué hizo cada dron en este segundo
                        NodoAccion actualAccion = actualSeg.Datos.AccionesDrones.Cabeza;
                        while (actualAccion != null)
                        {
                            // <dron nombre="DronX">Accion</dron>
                            XmlElement nodoDron = doc.CreateElement("dron");
                            nodoDron.SetAttribute("nombre", actualAccion.Datos.NombreDron);
                            nodoDron.InnerText = actualAccion.Datos.Accion;
                            nodoAcciones.AppendChild(nodoDron);

                            actualAccion = actualAccion.Siguiente;
                        }
                        actualSeg = actualSeg.Siguiente;
                    }
                }
                actualM = actualM.Siguiente;
            }

            // 3. Convertir el XML a un archivo descargable
            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                byte[] bytes = ms.ToArray();
                // Esto le dice al navegador "Oye, descarga este archivo, no lo abras"
                return File(bytes, "application/xml", "salida.xml");
            }


        }
        
        public IActionResult GraficaInstrucciones(string nombreMensaje)
        {
            // 1. Buscar el mensaje y el sistema en la memoria (igual que en VerInstrucciones)
            Mensaje msjEncontrado = null;
            NodoMensaje actualM = DatosGlobales.SistemaPrincipal.MensajesGlobales.Cabeza;
            while (actualM != null)
            {
                if (actualM.Datos.Nombre == nombreMensaje) { msjEncontrado = actualM.Datos; break; }
                actualM = actualM.Siguiente;
            }

            if (msjEncontrado == null) return RedirectToAction("ListadoMensajes");

            SistemaDrones sisAsociado = null;
            NodoSistema actualS = DatosGlobales.SistemaPrincipal.SistemasGlobales.Cabeza;
            while (actualS != null)
            {
                if (actualS.Datos.Nombre == msjEncontrado.SistemaDronesRequerido) { sisAsociado = actualS.Datos; break; }
                actualS = actualS.Siguiente;
            }

            // 2. Simular para tener la lista de segundos llena
            SimuladorVuelo simulador = new SimuladorVuelo();
            ListaSegundos simulacion = simulador.GenerarSimulacion(msjEncontrado, sisAsociado);

            // 3. Generar el código Graphviz y enviarlo a la vista
            ViewBag.CodigoDot = simulacion.GenerarGrafoDOT();
            ViewBag.NombreMensaje = nombreMensaje;

            return View();
        }
    }
}