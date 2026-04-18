namespace WebAPIGeo.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIGeo.Modelo;

[Route("api/[controller]")]
[ApiController]
public class AlumnosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlumnosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Alumnos (Obtener todos los alumnos con su ubicación)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alumno>>> GetAlumnos()
    {
        return await _context.Alumnos.ToListAsync();
    }

    // POST: api/Alumnos (Registrar nuevo alumno o ubicación)
    [HttpPost]
    public async Task<ActionResult<Alumno>> PostAlumno(Alumno alumno)
    {
        _context.Alumnos.Add(alumno);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAlumnos), new { id = alumno.Id }, alumno);
    }

    // PUT: api/Alumnos/5 (Actualizar ubicación por ID)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAlumno(int id, Alumno alumno)
    {
        if (id != alumno.Id) return BadRequest();

        _context.Entry(alumno).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Alumnos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlumno(int id)
    {
        var alumno = await _context.Alumnos.FindAsync(id);
        if (alumno == null) return NotFound();

        _context.Alumnos.Remove(alumno);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
