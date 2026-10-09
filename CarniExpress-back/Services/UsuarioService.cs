
using CarniExpress_back.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CarniExpress_back.Services
{
    public class UsuarioService
    {
        // Usuarios almacenados temporalmente en memoria
        private readonly List<Usuario> _usuarios = new();

        // Servicio para proteger contraseñas
        private readonly PasswordHasher<Usuario> _hasher = new();

        // Evita conflictos al modificar usuarios
        private readonly object _lock = new();

        // =========================================
        // HU-001: BUSCAR USUARIO POR CORREO
        // =========================================

        public Usuario? BuscarPorCorreo(string correo)
        {
            lock (_lock)
            {
                return _usuarios.FirstOrDefault(u =>
                    u.Correo.Equals(
                        correo.Trim(),
                        StringComparison.OrdinalIgnoreCase));
            }
        }

        // =========================================
        // HU-001: REGISTRAR USUARIO
        // =========================================

        public Usuario? Registrar(RegistroRequest request)
        {
            lock (_lock)
            {
                string correo = request.Correo.Trim().ToLowerInvariant();

                if (_usuarios.Any(u =>
                    u.Correo.Equals(
                        correo,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    return null;
                }

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
                    usuario,
                    request.Contrasena);

                _usuarios.Add(usuario);

                return usuario;
            }
        }

        // =========================================
        // HU-002: VALIDAR CONTRASEÑA
        // =========================================

        public bool ValidarContrasena(
            Usuario usuario,
            string contrasena)
        {
            var resultado = _hasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                contrasena);

            return resultado != PasswordVerificationResult.Failed;
        }

        // =========================================
        // HU-004: BUSCAR USUARIO POR ID
        // =========================================

        public Usuario? BuscarPorId(int id)
        {
            lock (_lock)
            {
                return _usuarios.FirstOrDefault(u =>
                    u.Id == id);
            }
        }

        // =========================================
        // HU-004: ACTUALIZAR PERFIL
        // =========================================

        public (Usuario? Usuario, string? Error)
            ActualizarPerfil(
                int id,
                ActualizarPerfilRequest request)
        {
            lock (_lock)
            {
                // Buscar usuario autenticado
                var usuario = _usuarios.FirstOrDefault(u =>
                    u.Id == id);

                if (usuario == null || !usuario.Activo)
                {
                    return (
                        null,
                        "Usuario no encontrado o inactivo"
                    );
                }

                // Limpiar los datos recibidos
                string nombre = request.Nombre?.Trim() ?? "";
                string correo = request.Correo?.Trim()
                    .ToLowerInvariant() ?? "";
                string telefono = request.Telefono?.Trim() ?? "";

                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(nombre) ||
                    string.IsNullOrWhiteSpace(correo) ||
                    string.IsNullOrWhiteSpace(telefono))
                {
                    return (
                        null,
                        "Todos los campos son obligatorios"
                    );
                }

                // Validar formato del correo
                var emailValidator = new EmailAddressAttribute();

                if (!emailValidator.IsValid(correo))
                {
                    return (
                        null,
                        "El correo no tiene un formato válido"
                    );
                }

                // Validar teléfono de 10 dígitos
                if (!Regex.IsMatch(
                    telefono, @"^[0-9]{10}$"))
                {
                    return (
                        null,
                        "El teléfono debe tener 10 dígitos"
                    );
                }

                // Verificar que el correo no esté registrado
                // en otra cuenta
                bool correoDuplicado = _usuarios.Any(u =>
                    u.Id != id &&
                    u.Correo.Equals(
                        correo,
                        StringComparison.OrdinalIgnoreCase));

                if (correoDuplicado)
                {
                    return (
                        null,
                        "Este correo ya está registrado"
                    );
                }

                // Actualizar solamente datos permitidos
                usuario.Nombre = nombre;
                usuario.Correo = correo;
                usuario.Telefono = telefono;

                return (usuario, null);
            }
        }
    }
}
