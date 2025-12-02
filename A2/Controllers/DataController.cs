using A2.Data;
using A2.DTOs.Common;
using A2.Models;
using A2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataController> _logger;

        public DataController(ApplicationDbContext context, ILogger<DataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ============================================
        // USERS (Usuários)
        // ============================================

        [HttpGet("Users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersLookup()
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.Ativo)
                .OrderBy(u => u.NomeCompleto)
                .Select(u => new UsuarioLookupDto
                {
                    Id = u.Id,
                    NomeCompleto = u.NomeCompleto
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // ✨ NOVO: POST Users
        [HttpPost("Users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NomeCompleto))
            {
                return BadRequest(new { message = "Nome completo é obrigatório." });
            }

            try
            {
                // Gera um email temporário baseado no nome
                var emailBase = dto.NomeCompleto
                    .ToLower()
                    .Replace(" ", ".")
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u")
                    .Replace("ã", "a")
                    .Replace("õ", "o")
                    .Replace("ç", "c");

                var email = $"{emailBase}@empresa.com";

                // Verifica se o email já existe
                var emailExists = await _context.Usuarios.AnyAsync(u => u.Email == email);
                if (emailExists)
                {
                    // Adiciona um número ao final do email
                    var count = await _context.Usuarios.CountAsync(u => u.Email.StartsWith(emailBase));
                    email = $"{emailBase}{count + 1}@empresa.com";
                }

                // Gera um CPF temporário (não validado, apenas para cumprir a constraint)
                var cpfTemporario = $"{DateTime.Now.Ticks.ToString().Substring(0, 11)}";

                // Busca o primeiro departamento ativo ou usa um ID padrão
                var departamentoPadrao = await _context.Departamentos
                    .Where(d => d.Ativo)
                    .OrderBy(d => d.Id)
                    .FirstOrDefaultAsync();

                if (departamentoPadrao == null)
                {
                    return BadRequest(new { message = "Nenhum departamento ativo encontrado. Crie um departamento primeiro." });
                }

                var novoUsuario = new Usuario
                {
                    NomeCompleto = dto.NomeCompleto.Trim(),
                    Email = email,
                    Cpf = cpfTemporario,
                    DepartamentoId = departamentoPadrao.Id,
                    Perfil = PerfilUsuario.Colaborador, // Perfil padrão
                    Ativo = true,
                    CriadoEm = DateTime.UtcNow
                };

                _context.Usuarios.Add(novoUsuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Novo usuário criado: ID {novoUsuario.Id}, Nome: {novoUsuario.NomeCompleto}");

                return CreatedAtAction(
                    nameof(GetUsersLookup),
                    new { id = novoUsuario.Id },
                    new { id = novoUsuario.Id, nomeCompleto = novoUsuario.NomeCompleto }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao criar usuário." });
            }
        }

        // ============================================
        // CURRENCIES (Moedas)
        // ============================================

        [HttpGet("Currencies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrenciesLookup()
        {
            var moedas = await _context.Moedas
                .OrderBy(m => m.Codigo)
                .Select(m => new MoedaLookupDto
                {
                    Id = m.Id,
                    Codigo = m.Codigo,
                    Nome = m.Nome,
                    Simbolo = m.Simbolo
                })
                .ToListAsync();

            return Ok(moedas);
        }

        // ============================================
        // DEPARTMENTS (Departamentos)
        // ============================================

        [HttpGet("Departments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartmentsLookup()
        {
            var departamentos = await _context.Departamentos
                .Where(d => d.Ativo)
                .OrderBy(d => d.Nome)
                .Select(d => new { Id = d.Id, Nome = d.Nome })
                .ToListAsync();

            return Ok(departamentos);
        }

        // ✨ NOVO: POST Departments
        [HttpPost("Departments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new { message = "Nome do departamento é obrigatório." });
            }

            try
            {
                // Verifica se já existe um departamento com esse nome
                var nomeExists = await _context.Departamentos
                    .AnyAsync(d => d.Nome.ToLower() == dto.Nome.ToLower());

                if (nomeExists)
                {
                    return BadRequest(new { message = "Já existe um departamento com este nome." });
                }

                // Gera um Centro de Custo baseado no nome
                var centroDeCusto = $"CC-{dto.Nome.ToUpper().Replace(" ", "-").Substring(0, Math.Min(10, dto.Nome.Length))}";

                var novoDepartamento = new Departamento
                {
                    Nome = dto.Nome.Trim(),
                    CentroDeCusto = centroDeCusto,
                    GestorId = null, // Pode ser definido depois
                    Ativo = true
                };

                _context.Departamentos.Add(novoDepartamento);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Novo departamento criado: ID {novoDepartamento.Id}, Nome: {novoDepartamento.Nome}");

                return CreatedAtAction(
                    nameof(GetDepartmentsLookup),
                    new { id = novoDepartamento.Id },
                    new { id = novoDepartamento.Id, nome = novoDepartamento.Nome }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar departamento.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao criar departamento." });
            }
        }
    }

    // ============================================
    // DTOs para criação
    // ============================================

    public class CreateUserDto
    {
        public string NomeCompleto { get; set; } = string.Empty;
    }

    public class CreateDepartmentDto
    {
        public string Nome { get; set; } = string.Empty;
    }
}