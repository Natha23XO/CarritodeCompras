using AppProyecto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AppProyecto.Controllers
{
    public class LoginController : Controller
    {
       //GET Login
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string NCorreo, string NContrasena)
        {
            Usuario oUsuario = new Usuario();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7177/api/UsuarioAPI/");
                HttpResponseMessage response = await client.GetAsync("getUsuario/"+NCorreo+"/"+ NContrasena);
                string apiReponse = await response.Content.ReadAsStringAsync();
                oUsuario = JsonConvert.DeserializeObject<Usuario>(apiReponse);
            }
            if (oUsuario == null)
            {
                ViewBag.Error = "Correo o contraseña no correcta";
                return View();
            }

            string rol = string.Empty;
            if (oUsuario.EsAdministrador == true)
            {
                rol = "ADMIN";
            }
            else
            {
                rol = "USER";
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, oUsuario.Nombres),
                new Claim("Correo", oUsuario.Correo),
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.NameIdentifier, oUsuario.IdUsuario.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (oUsuario.EsAdministrador == true)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Tienda");
            }

        }

        public ActionResult Registrarse()
        {
            return View(new Usuario() { Nombres = "", Apellidos = "", Correo = "", Contrasena = "", ConfirmarContrasena = "" });
        }


        [HttpPost]
        public async Task<ActionResult> Registrarse(string NNombres, string NApellidos, string NCorreo, string NContrasena, string NConfirmarContrasena)
        {
            Usuario oUsuario = new Usuario()
            {
                Nombres = NNombres,
                Apellidos = NApellidos,
                Correo = NCorreo,
                Contrasena = NContrasena,
                ConfirmarContrasena = NConfirmarContrasena,
                EsAdministrador = false
            };

            if (NContrasena != NConfirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View(oUsuario);
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7177/api/UsuarioAPI/");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(oUsuario),
                        Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("insertUsuario", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    int respuesta;
                    if(int.TryParse(apiResponse, out respuesta))
                    {
                        if(respuesta == 0)
                        {
                            ViewBag.Error = "Error al registrar";
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login");
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Error al registrar";
                        return View();
                    }
                }
            }


        }


        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
