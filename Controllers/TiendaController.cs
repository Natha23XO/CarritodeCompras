using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppProyecto.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AppProyecto.Controllers
{
    public class TiendaController : Controller
    {

        private readonly IWebHostEnvironment _hostingEnvironment;
        //private static Usuario oUsuario;

        public TiendaController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        private int ObtenerIdUsuario()
        {
            var nombreClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(nombreClaim, out int idUsuario))
            {
                return idUsuario;
            }
            else
            {
                return -1; // O cualquier otro valor que indique un error o valor no válido
            }
        }


        // VISTA
        public IActionResult Index()
        {
            var nombreClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (nombreClaim != null)
            {
                ViewBag.admin = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return View();
            }
            else { return View(); }
        }

        //VISTA
        public ActionResult Carrito()
        {
            var nombreClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (nombreClaim == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.admin = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                ViewBag.nombre = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                return View();
            }
        }

        //VISTA
        public ActionResult Compras()
        {
            var nombreClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (nombreClaim == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.admin = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return View();
            }
        }

        // VISTA
        public async Task<ActionResult> Producto(int idproducto = 0)
        {
            var nombreClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (nombreClaim == null)
                return RedirectToAction("Index", "Login");

            Producto oProducto = new Producto();
            List<Producto> oLista = await ObtenerProductosDeApi();

            oProducto = oLista.FirstOrDefault(o => o.IdProducto == idproducto);

            // Si el producto existe, convierte la imagen a base64
            if (oProducto != null && !string.IsNullOrEmpty(oProducto.RutaImagen))
            {
                oProducto.base64 = ConvertirImagenABase64(oProducto.RutaImagen);
                oProducto.extension = Path.GetExtension(oProducto.RutaImagen).Replace(".", "");
            }
            ViewBag.admin = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return View(oProducto);
        }


        private async Task<List<Producto>> ObtenerProductosDeApi()
        {
            using (var producto = new HttpClient())
            {
                producto.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                HttpResponseMessage responseMessage = await producto.GetAsync("getProductos");
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Producto>>(apiResponse);
            }
        }

        private string ConvertirImagenABase64(string rutaRelativa)
        {
            string rutaCompleta = Path.Combine(_hostingEnvironment.WebRootPath, rutaRelativa);
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(rutaCompleta));
        }


        [HttpPost]
        public async Task<IActionResult> ListarProducto(string objeto)
        {
            int idmarca = 0;
            if(objeto != null){
                idmarca = Convert.ToInt32(objeto);
            }

            List<Producto> oLista = new List<Producto>();
            using (var producto = new HttpClient())
            {
                producto.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                HttpResponseMessage responseMessage = await producto.GetAsync("getProductos");
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Producto>>(apiResponse).ToList();
            }

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
                          base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_hostingEnvironment.WebRootPath, o.RutaImagen))),
                          extension = Path.GetExtension(o.RutaImagen).Replace(".", ""),
                          Activo = o.Activo
                      }).ToList();

            if (idmarca != 0)
            {
                oLista = oLista.Where(x => x.oMarca.IdMarca == idmarca).ToList();
            }

            return Json(new { data = oLista });
        }



        public async Task<IActionResult> ProductosIndex()
        {
            List<Producto> prods = new List<Producto>();
            using (var producto = new HttpClient())
            {
                producto.BaseAddress = new Uri("https://localhost:7177/api/ProductoAPI/");
                HttpResponseMessage responseMessage = await producto.GetAsync("getProductos");
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                prods = JsonConvert.DeserializeObject<List<Producto>>(apiResponse).ToList();
            }
            return View(await Task.Run(() => prods));
        }


        [HttpGet]
        public async Task<JsonResult> ListarCategoria()
        {
            List<Categoria> oLista = new List<Categoria>();
            using (var categoria = new HttpClient())
            {
                categoria.BaseAddress = new Uri("https://localhost:7177/api/CategoriaAPI/");
                HttpResponseMessage responseMessage = await categoria.GetAsync("getCategorias");
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Categoria>>(apiResponse).ToList();
            }
            return Json(new { data = oLista });
        }

        [HttpGet]
        public async Task<JsonResult> ListarMarcas()
        {
            List<Marca> oLista = new List<Marca>();
            using (var categoria = new HttpClient())
            {
                categoria.BaseAddress = new Uri("https://localhost:7177/api/MarcaAPI/");
                HttpResponseMessage responseMessage = await categoria.GetAsync("getMarcas");
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Marca>>(apiResponse).ToList();
            }
            return Json(new { data = oLista });
        }



        [HttpPost]
        public async Task<JsonResult> InsertarCarrito(int idproducto, Carrito oCarrito)
        {
            int idUser = ObtenerIdUsuario();
            int idPro = idproducto;
            oCarrito.oUsuario = new Usuario() { IdUsuario = idUser };
            oCarrito.oProducto = new Producto() { IdProducto = idPro };

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("https://localhost:7177/api/CarritoAPI/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(oCarrito),
                    Encoding.UTF8, "application/json");
                HttpResponseMessage message = await cliente.PostAsync("insertCarrito", content);

                if (message.IsSuccessStatusCode)
                {
                    string apiResponse = await message.Content.ReadAsStringAsync();
                    return Json(new { respuesta = apiResponse });
                }
                else
                {
                    // Manejar la respuesta fallida de la API aquí.
                    return Json(new { respuesta = "Error al insertar en el carrito." });
                }
            }
        }

        [HttpGet]
         public async Task<IActionResult> CantidadCarrito()
         {
             int _respuesta = 0;
             int idUser = ObtenerIdUsuario();

             using (var carrito = new HttpClient())
             {
                 carrito.BaseAddress = new Uri("https://localhost:7177/api/CarritoAPI/");
                 HttpResponseMessage responseMessage = await carrito.GetAsync($"getCantidad/{idUser}"); // url api: .../getCantidad/{id}
                 string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                 _respuesta = JsonConvert.DeserializeObject<int>(apiResponse);
             }
             return Json(new { respuesta = _respuesta });
         }

        [HttpGet]
        public async Task<JsonResult> ObtenerCarrito()
        {
            List<Carrito> oLista = new List<Carrito>();
            int idUser = ObtenerIdUsuario();

            using (var carrito = new HttpClient())
            {
                carrito.BaseAddress = new Uri("https://localhost:7177/api/CarritoAPI/");
                HttpResponseMessage responseMessage = await carrito.GetAsync($"getCarrito/{idUser}"); // url api: .../getCarrito/{id}
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Carrito>>(apiResponse);
            }

            if (oLista.Count != 0)
            {
                oLista = (from d in oLista
                          select new Carrito()
                          {
                              IdCarrito = d.IdCarrito,
                              oProducto = new Producto()
                              {
                                  IdProducto = d.oProducto.IdProducto,
                                  Nombre = d.oProducto.Nombre,
                                  oMarca = new Marca() { Descripcion = d.oProducto.oMarca.Descripcion },
                                  Precio = d.oProducto.Precio,
                                  RutaImagen = d.oProducto.RutaImagen,
                                  base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_hostingEnvironment.WebRootPath, d.oProducto.RutaImagen))),
                                  extension = Path.GetExtension(d.oProducto.RutaImagen).Replace(".", ""),
                              }
                          }).ToList();
            }
            return Json(new { lista = oLista });
        }

        [HttpPost]
        public async Task<JsonResult> EliminarCarrito(string IdCarrito, string IdProducto)
        {
            bool respuesta = false;
            var carritoInfo = new { IdCarrito = IdCarrito, IdProducto = IdProducto };

            using (var carrito = new HttpClient())
            {
                carrito.BaseAddress = new Uri("https://localhost:7177/api/CarritoAPI/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(carritoInfo), Encoding.UTF8, "application/json");

                HttpResponseMessage message = await carrito.DeleteAsync("eliminarCarrito?IdCarrito="+IdCarrito+"&IdProducto="+ IdProducto);
                string apiResponse = await message.Content.ReadAsStringAsync();
                respuesta = JsonConvert.DeserializeObject<bool>(apiResponse);
            }
            return Json(new { resultado = respuesta });
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerDepartamento()
        {
            List<Departamento> oLista = new List<Departamento>();
            using (var departamento = new HttpClient())
            {
                departamento.BaseAddress = new Uri("https://localhost:7177/api/UbigeoAPI/");
                HttpResponseMessage responseMessage = await departamento.GetAsync("getDepartamento");
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Departamento>>(apiResponse).ToList();
            }
            return Json(new { lista = oLista });
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerProvincia(string _IdDepartamento)
        {
            List<Provincia> oLista = new List<Provincia>();
            using (var provincia = new HttpClient())
            {
                provincia.BaseAddress = new Uri("https://localhost:7177/api/UbigeoAPI/");
                HttpResponseMessage responseMessage = await provincia.GetAsync($"getProvincia/{_IdDepartamento}"); // url api: UbigeoAPI/getProvincia/{iddepartamento}
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Provincia>>(apiResponse).ToList();
            }
            return Json(new { lista = oLista });
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerDistrito(string _IdProvincia, string _IdDepartamento)
        {
            List<Distrito> oLista = new List<Distrito>();
            using (var distrito = new HttpClient())
            {
                distrito.BaseAddress = new Uri("https://localhost:7177/api/UbigeoAPI/");
                HttpResponseMessage responseMessage = await distrito.GetAsync($"getDistrito/{_IdProvincia}/{_IdDepartamento}"); // url api: UbigeoAPI/getDistrito/{idprovincia}/{iddepartamento}
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Distrito>>(apiResponse).ToList();
            }
            return Json(new { lista = oLista });
        }


        [HttpPost]
        public async Task<JsonResult> RegistrarCompra(string objeto)
        {
            bool respuesta = false;
            Compra oCompra = new Compra();
            oCompra = JsonConvert.DeserializeObject<Compra>(objeto);

            oCompra.IdUsuario = ObtenerIdUsuario();


            using (var compra = new HttpClient())
            {
                compra.BaseAddress = new Uri("https://localhost:7177/api/CompraAPI/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(oCompra), Encoding.UTF8, "application/json");

                HttpResponseMessage message = await compra.PostAsync("insertCompra", content);
                string apiResponse = await message.Content.ReadAsStringAsync();
                respuesta = JsonConvert.DeserializeObject<bool>(apiResponse);
            }
            return Json(new { resultado = respuesta });
        }


        [HttpGet]
        public async Task<JsonResult> ObtenerCompra()
        {
            List<Compra> oLista = new List<Compra>();
            int idUser = ObtenerIdUsuario();

            using (var compra = new HttpClient())
            {
                compra.BaseAddress = new Uri("https://localhost:7177/api/CarritoAPI/");
                HttpResponseMessage responseMessage = await compra.GetAsync($"getCompra/{idUser}"); // url api: CarritoAPI/getCompra/{id}
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                oLista = JsonConvert.DeserializeObject<List<Compra>>(apiResponse).ToList();
            }

            oLista = (from c in oLista
                      select new Compra()
                      {
                          Total = c.Total,
                          FechaTexto = c.FechaTexto,
                          oDetalleCompra = (from dc in c.oDetalleCompra
                                            select new DetalleCompra()
                                            {
                                                oProducto = new Producto()
                                                {
                                                    oMarca = new Marca() { Descripcion = dc.oProducto.oMarca.Descripcion },
                                                    Nombre = dc.oProducto.Nombre,
                                                    RutaImagen = dc.oProducto.RutaImagen,
                                                    base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(_hostingEnvironment.WebRootPath, dc.oProducto.RutaImagen))),
                                                    extension = Path.GetExtension(dc.oProducto.RutaImagen).Replace(".", ""),
                                                },
                                                Total = dc.Total,
                                                Cantidad = dc.Cantidad
                                            }).ToList()
                      }).ToList();
            return Json(new { lista = oLista });
        }

    }
}
