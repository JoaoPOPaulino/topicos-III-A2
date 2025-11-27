// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using A2.Data;
using A2.DTOs.Auth;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Departamento)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Ativo);

            if (usuario == null)
                return Unauthorized("Usuário não encontrado");

            return Ok(new
            {
                usuario.Id,
                usuario.NomeCompleto,
                usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Departamento = usuario.Departamento?.Nome
            });
        }
    }
}

namespace A2.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}