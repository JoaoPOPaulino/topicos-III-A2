using A2.Data;
using A2.DTOs.Common;
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
    }
}