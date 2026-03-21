using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Proyecto2.Models;

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
    }
}