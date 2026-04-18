using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIGeo.Modelo;

namespace WebAPIGeo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Usuarios20453Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public Usuarios20453Controller(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios20453>>> GetUsuarios()
        {
            return await _context.Usuarios20453.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios20453>> GetUsuarios(int id)
        {
            var usuario = await _context.Usuarios20453.FindAsync(id);
            if (usuario == null) return NotFound();
            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuarios20453>> PostUsuarios(Usuarios20453 usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                usuario.Email = "sin email";
            }
            
            _context.Usuarios20453.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(int id, Usuarios20453 usuario)
        {
            if (id != usuario.Id) return BadRequest();

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(int id)
        {
            var usuario = await _context.Usuarios20453.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios20453.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
