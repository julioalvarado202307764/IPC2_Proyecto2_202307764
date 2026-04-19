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
        // Este método simplemente muestra la nueva vista web
        [HttpGet]
        public IActionResult CargarArchivo()
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
        //CAMBIOS EXAMEN FINAL PROYECTO 2
        // --- 1. MÉTODO AYUDANTE PARA INVERTIR EL TEXTO ---
        // Lo ponemos "private" porque solo lo usaremos aquí adentro
        private string InvertirCadenaManual(string original)
        {
            string invertida = "";
            for (int i = original.Length - 1; i >= 0; i--)
            {
                invertida += original[i];
            }
            return invertida;
        }

        // --- 2. MÉTODO AYUDANTE PARA INVERTIR LA LISTA DE INSTRUCCIONES ---
        // Lo ponemos "private" también
        // --- 2. MÉTODO AYUDANTE PARA INVERTIR LA LISTA DE INSTRUCCIONES ---
        private ListaInstrucciones InvertirInstruccionesManual(ListaInstrucciones original)
        {
            ListaInstrucciones listaInvertida = new ListaInstrucciones();
            var actual = original.Cabeza; // 'actual' es un NodoInstruccion

            while (actual != null)
            {
                // 1. Extraemos los datos usando actual.Datos
                string nombre = actual.Datos.NombreDron;
                int altura = actual.Datos.AlturaObjetivo;

                // 2. Creamos la nueva instrucción usando tu constructor
                Instruccion nuevaInstruccion = new Instruccion(nombre, altura);

                // 3. Empaquetamos la instrucción en un nuevo Nodo
                NodoInstruccion nuevoNodo = new NodoInstruccion();
                nuevoNodo.Datos = nuevaInstruccion;
                nuevoNodo.Siguiente = null;

                // 4. LÓGICA DE INSERCIÓN AL INICIO (Esto invierte la lista)
                if (listaInvertida.Cabeza == null)
                {
                    listaInvertida.Cabeza = nuevoNodo;
                }
                else
                {
                    nuevoNodo.Siguiente = listaInvertida.Cabeza;
                    listaInvertida.Cabeza = nuevoNodo;
                }

                // 5. Avanzamos al siguiente nodo de la lista original
                actual = actual.Siguiente;
            }

            return listaInvertida;
        }

        // --- MÉTODO AYUDANTE PARA BUSCAR EL MENSAJE EN TU LISTA GLOBAL ---
        // --- MÉTODO AYUDANTE PARA BUSCAR EL MENSAJE ---
        // --- MÉTODO AYUDANTE PARA BUSCAR EL MENSAJE ---
        private Mensaje BuscarMensajePorNombre(string nombreMensaje)
        {
            // Ahora usamos MensajesGlobales que es el nombre correcto
            var actual = DatosGlobales.SistemaPrincipal.MensajesGlobales.Cabeza;

            while (actual != null)
            {
                if (actual.Datos.Nombre == nombreMensaje)
                {
                    return actual.Datos;
                }
                actual = actual.Siguiente;
            }
            return null;
        }

        // buscar el sistema
        private SistemaDrones BuscarSistemaPorNombre(string nombreSistema)
        {
            // Ahora usamos SistemasGlobales que es el nombre correcto
            var actual = DatosGlobales.SistemaPrincipal.SistemasGlobales.Cabeza;

            while (actual != null)
            {
                if (actual.Datos.Nombre == nombreSistema)
                {
                    return actual.Datos;
                }
                actual = actual.Siguiente;
            }
            return null;
        }
        // boton inverso
        public IActionResult SimularModoInverso(string nombreMensaje)
        {
            // Busca el mensaje original
            Mensaje mensajeOriginal = BuscarMensajePorNombre(nombreMensaje);

            if (mensajeOriginal == null)
            {
                TempData["MensajeError"] = "Mensaje no encontrado.";
                return RedirectToAction("ListadoMensajes"); // Asegúrate de que esta sea la vista correcta
            }

            //  el nombre invertido primero
            string nombreInvertido = InvertirCadenaManual(mensajeOriginal.Nombre);
            string sistemaRequerido = mensajeOriginal.SistemaDronesRequerido; // Si te da error de nombre aquí, revisa cómo le llamaste a esta propiedad en tu clase Mensaje (ej. SistemaAsociado)

            // Usamos tu constructor exacto (el que vimos en ProcesadorXML)
            Mensaje mensajeInvertido = new Mensaje(nombreInvertido, sistemaRequerido);

            // Invertimos las instrucciones
            mensajeInvertido.Instrucciones = InvertirInstruccionesManual(mensajeOriginal.Instrucciones);

            // Busca el sistema y simula
            SistemaDrones sistema = BuscarSistemaPorNombre(sistemaRequerido);
            SimuladorVuelo simulador = new SimuladorVuelo();

            int tiempoOptimo = simulador.CalcularTiempoOptimo(mensajeInvertido, sistema);
            var lineaDeTiempo = simulador.GenerarSimulacion(mensajeInvertido, sistema);

            ViewBag.TiempoOptimo = tiempoOptimo;
            ViewBag.NombreMensaje = mensajeInvertido.Nombre;
            DatosGlobales.SistemaPrincipal.MensajesGlobales.InsertarOrdenado(mensajeInvertido);
            return View("VerInstrucciones", lineaDeTiempo);
        }

        public IActionResult Ayuda()
        {
            return View();
        }
    }
}