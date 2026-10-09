using CarniExpress_back.Models;
using Microsoft.AspNetCore.Identity;

namespace CarniExpress_back.Services
{
    public class UsuarioService
    {
        private readonly List<Usuario> _usuarios = new();
        private readonly PasswordHasher<Usuario> _hasher = new();
        private readonly object _lock = new();

        public Usuario? BuscarPorCorreo(string correo)
        {
            lock (_lock)
            {
                return _usuarios.FirstOrDefault(u =>
                    u.Correo.Equals(correo,
                        StringComparison.OrdinalIgnoreCase));
            }
        }

        public Usuario? Registrar(RegistroRequest request)
        {
            lock (_lock)
            {
                string correo = request.Correo.Trim().ToLowerInvariant();

                if (_usuarios.Any(u => u.Correo == correo))
                    return null;

                var usuario = new Usuario
                {
                    Id = _usuarios.Count + 1,
                    Nombre = request.Nombre.Trim(),
                    Correo = correo,
                    Telefono = request.Telefono,
                    Rol = "CLIENTE",
                    Activo = true
                };

                usuario.PasswordHash = _hasher.HashPassword(
                    usuario, request.Contrasena);

                _usuarios.Add(usuario);

                return usuario;
            }
        }

        public bool ValidarContrasena(
            Usuario usuario, string contrasena)
        {
            var resultado = _hasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                contrasena);

            return resultado != PasswordVerificationResult.Failed;
        }
    }
}