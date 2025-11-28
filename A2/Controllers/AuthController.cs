using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using A2.Data;
using A2.DTOs;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                return Unauthorized("Credenciais inválidas.");

            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Departamento)
                    .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Ativo);

                if (usuario == null)
                {
                    _logger.LogWarning($"Tentativa de login falhou para email: {dto.Email} (Usuário não encontrado ou inativo).");
                    return Unauthorized("Credenciais inválidas.");
                }

                // ⚠️ SIMULAÇÃO DE SENHA: 
                // Como não há hash no modelo, usamos uma senha fixa "123456" para testes.
                // Na produção, a senha real seria verificada contra um hash (e.g., BCrypt).
                if (dto.Senha != "123456")
                {
                    _logger.LogWarning($"Tentativa de login falhou para email: {dto.Email} (Senha incorreta).");
                    return Unauthorized("Credenciais inválidas.");
                }

                _logger.LogInformation($"Login bem-sucedido: ID {usuario.Id}, Perfil {usuario.Perfil}.");

                // Retorna o token/dados do usuário logado
                return Ok(new
                {
                    usuario.Id,
                    usuario.NomeCompleto,
                    usuario.Email,
                    Perfil = usuario.Perfil.ToString(), // Perfil Colaborador ou RH
                    Departamento = usuario.Departamento?.Nome
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno durante o processamento do login.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno de servidor.");
            }
        }
    }
}