﻿namespace AppProyecto.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public string? ConfirmarContrasena { get; set; }
        public bool EsAdministrador { get; set; }
        public bool Activo { get; set; }
    }
}
