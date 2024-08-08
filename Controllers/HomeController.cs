using AppProyecto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;

namespace AppProyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult Index()
        {
            var session = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (session == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult Categoria()
        {
            var session = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (session == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult Marca()
        {
            var session = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (session == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult Producto()
        {
            var session = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (session == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult Tienda()
        {
            var session = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (session == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<JsonResult> ListarCategoria()
        {
            List<Categoria> oLista = new List<Categoria>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7177/api/CategoriaAPI/");
                HttpResponseMessage response = await client.GetAsync("getCategorias");
                string apiReponse = await response.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Categoria>>(apiReponse);
            }
            return Json(new { data = oLista });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarCategoria(Categoria objeto)
        {
            bool respuesta = false;
            if(objeto.IdCategoria == 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7177/api/CategoriaAPI/");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(objeto),
                       Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("insertCategoria", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    respuesta = Convert.ToBoolean(apiResponse.ToLower());
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7177/api/CategoriaAPI/");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(objeto),
                       Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("updateCategoria", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    respuesta = Convert.ToBoolean(apiResponse.ToLower());
                }
            }
            return Json(new {data = respuesta});
        }

        [HttpPost]
        public async Task<JsonResult> EliminarCategoria(int id)
        {
            bool respuesta = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7177/api/CategoriaAPI/");
                HttpResponseMessage response = await client.DeleteAsync($"deleteCategoria/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                respuesta = Convert.ToBoolean(apiResponse.ToLower());
            }
            return Json(new { data = respuesta });
        }

        [HttpGet]
        public async Task<JsonResult> ListarMarca()
        {
            List<Marca> oLista = new List<Marca>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7177/api/MarcaAPI/");
                HttpResponseMessage response = await client.GetAsync("getMarcas");
                string apiReponse = await response.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Marca>>(apiReponse);
            }
            return Json(new { data = oLista });
        }

        [HttpGet]
        public async Task<JsonResult> ListarProducto()
        {
            List<Producto> oLista = new List<Producto>();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                HttpResponseMessage response = await client.GetAsync("getProductos");
                string apiReponse = await response.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Producto>>(apiReponse);
                oLista = (from o in oLista
                          select new Producto()
                          {
                              IdProducto = o.IdProducto,
                              Nombre = o.Nombre,
                              Descripcion = o.Descripcion,
                              oMarca = o.oMarca,
                              oCategoria = o.oCategoria,
                              Precio = o.Precio,
                              Stock = o.Stock,
                              RutaImagen = o.RutaImagen,
                              base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, o.RutaImagen))),
                              extension = Path.GetExtension(o.RutaImagen).Replace(".", ""),
                              Activo = o.Activo
                          }).ToList();
            }
            return Json(new { data = oLista });
        }
        
        
        [HttpPost]
        public async Task<JsonResult> GuardarProducto(string objeto, IFormFile imagenArchivo)
        {
            Response oresponse = new Response() { resultado = true, mensaje = "" };
            try
            {
                Producto oProducto = new Producto();
                oProducto = JsonConvert.DeserializeObject<Producto>(objeto);


                string GuardarEnRuta = "Imagenes/Productos/";
                string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "Imagenes/Productos");

                if(!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);
                
                if(oProducto.IdProducto == 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                        StringContent content = new StringContent(JsonConvert.SerializeObject(oProducto),
                           Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("insertProducto", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        int respuesta;
                        if (int.TryParse(apiResponse, out respuesta))
                        {
                            oProducto.IdProducto = respuesta;
                            oresponse.resultado = oProducto.IdProducto == 0 ? false : true;
                        }
                    }
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                        StringContent content = new StringContent(JsonConvert.SerializeObject(oProducto),
                           Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PutAsync("updateProducto", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        oresponse.resultado = Convert.ToBoolean(apiResponse.ToLower());
                    }
                }

                if (imagenArchivo != null && oProducto.IdProducto != 0)
                {
                    string extension = Path.GetExtension(imagenArchivo.FileName);
                    GuardarEnRuta = GuardarEnRuta + oProducto.IdProducto.ToString() + extension;
                    oProducto.RutaImagen = GuardarEnRuta;

                    string rutaFisicaCompleta = Path.Combine(physicalPath, oProducto.IdProducto.ToString() + extension);

                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        await imagenArchivo.CopyToAsync(stream);
                    }
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                        StringContent content = new StringContent(JsonConvert.SerializeObject(oProducto),
                           Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PutAsync("updateRutaProducto", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        oresponse.resultado = Convert.ToBoolean(apiResponse.ToLower());
                    }

                }
            }
            catch(Exception e)
            {
                oresponse.resultado = false;
                oresponse.mensaje = e.Message;
            }
            return Json(oresponse);

        }


        [HttpPost]
        public async Task<JsonResult> EliminarProducto(int id)
        {
            bool respuesta = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                HttpResponseMessage response = await client.DeleteAsync($"deleteProducto/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                respuesta = Convert.ToBoolean(apiResponse.ToLower());
            }
            return Json(new { resultado = respuesta });
        }



        public class Response
        {
            public bool resultado { get; set; }
            public string mensaje { get; set; }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}