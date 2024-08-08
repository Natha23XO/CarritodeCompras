namespace AppProyecto
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configura la sesión
            services.AddSession(options =>
            {
                options.Cookie.Name = ".TuAplicacion.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Otros servicios
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Habilita el middleware de sesión
            app.UseSession();

            // Otros middlewares y configuraciones
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
