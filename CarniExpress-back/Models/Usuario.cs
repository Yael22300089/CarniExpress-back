namespace CarniExpress_back.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Rol { get; set; } = "CLIENTE";
        public bool Activo { get; set; } = true;
    }
}