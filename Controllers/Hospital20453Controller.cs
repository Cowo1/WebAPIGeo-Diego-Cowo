using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIGeo.Modelo;
using System.ComponentModel.DataAnnotations;

namespace WebAPIGeo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Hospital20453Controller : ControllerBase
    {
        private readonly AppDbContext _context;
        private static readonly string[] MedicosAutorizados = { "MED-1010", "MED-2020", "MED-3030", "MED-4040", "MED-5050" };

        public Hospital20453Controller(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-conexion")]
        public async Task<ActionResult> TestConexion()
        {
            try
            {
                bool canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { estado = "Conexión exitosa", mensaje = "La BD está conectada correctamente" });
                }
                else
                {
                    return StatusCode(503, new { estado = "Error", mensaje = "No se puede conectar a la BD" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { estado = "Error", mensaje = ex.Message, detalles = ex.InnerException?.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hospital20453>>> GetHospital()
        {
            try
            {
                var pacientes = await _context.Hospital20453.ToListAsync();

                if (pacientes.Count <= 1)
                {
                    return Ok(pacientes);
                }

                // Algoritmo de ordenamiento por Selección
                // Criterio: Primero gravedad 5, descendiendo hasta 1, a igual gravedad el más antiguo tiene prioridad
                for (int i = 0; i < pacientes.Count - 1; i++)
                {
                    int indiceMax = i;
                    for (int j = i + 1; j < pacientes.Count; j++)
                    {
                        if (pacientes[j].NivelGravedad > pacientes[indiceMax].NivelGravedad)
                        {
                            indiceMax = j;
                        }
                        else if (pacientes[j].NivelGravedad == pacientes[indiceMax].NivelGravedad && 
                                 pacientes[j].FechaIngreso < pacientes[indiceMax].FechaIngreso)
                        {
                            indiceMax = j;
                        }
                    }

                    if (indiceMax != i)
                    {
                        var temp = pacientes[i];
                        pacientes[i] = pacientes[indiceMax];
                        pacientes[indiceMax] = temp;
                    }
                }

                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener pacientes", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hospital20453>> GetHospital(string id)
        {
            var hospital = await _context.Hospital20453.FindAsync(id);
            if (hospital == null) return NotFound();
            return hospital;
        }

        [HttpPost]
        public async Task<ActionResult<Hospital20453>> PostHospital([FromBody] Hospital20453 hospital)
        {
            // Validar que NombrePaciente no sea nulo
            if (string.IsNullOrWhiteSpace(hospital.NombrePaciente))
            {
                return BadRequest(new { error = "El nombre del paciente es requerido." });
            }

            // Validar NivelGravedad (1-5)
            if (hospital.NivelGravedad < 1 || hospital.NivelGravedad > 5)
            {
                return BadRequest(new { error = "El nivel de gravedad debe estar entre 1 y 5." });
            }

            // Validar Estado
            string[] estadosValidos = { "En espera", "Atendido", "Derivado" };
            if (string.IsNullOrWhiteSpace(hospital.Estado) || !estadosValidos.Contains(hospital.Estado))
            {
                return BadRequest(new { error = "El estado debe ser: En espera, Atendido o Derivado." });
            }

            // Validar Autorización del Médico
            if (string.IsNullOrWhiteSpace(hospital.MedicoResponsable) || !MedicosAutorizados.Contains(hospital.MedicoResponsable))
            {
                return Unauthorized(new { error = "El carnet del médico no está autorizado." });
            }

            // Validar Capacidad Crítica
            if (hospital.NivelGravedad == 5)
            {
                var pacientesCriticos = await _context.Hospital20453
                    .Where(p => p.NivelGravedad == 5 && p.Estado == "En espera")
                    .CountAsync();

                if (pacientesCriticos >= 5)
                {
                    return BadRequest(new { error = "Capacidad máxima alcanzada. Redirección inmediata a otro hospital sugerida" });
                }
            }

            // Generar ID automáticamente (PAC-2026-XXX)
            var ultimoPaciente = await _context.Hospital20453
                .OrderByDescending(p => p.IdPaciente)
                .FirstOrDefaultAsync();

            int numeroCorrelativo = 1;
            if (ultimoPaciente != null)
            {
                var ultimoNumero = int.Parse(ultimoPaciente.IdPaciente.Substring(9));
                numeroCorrelativo = ultimoNumero + 1;
            }

            hospital.IdPaciente = $"PAC-2026-{numeroCorrelativo:D3}";
            hospital.FechaIngreso = DateTime.Now;

            _context.Hospital20453.Add(hospital);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHospital), new { id = hospital.IdPaciente }, hospital);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHospital(string id, Hospital20453 hospital)
        {
            if (id != hospital.IdPaciente) return BadRequest();

            // Validar que NombrePaciente no sea nulo
            if (string.IsNullOrWhiteSpace(hospital.NombrePaciente))
            {
                return BadRequest(new { error = "El nombre del paciente es requerido." });
            }

            // Validar NivelGravedad (1-5)
            if (hospital.NivelGravedad < 1 || hospital.NivelGravedad > 5)
            {
                return BadRequest(new { error = "El nivel de gravedad debe estar entre 1 y 5." });
            }

            // Validar Estado
            string[] estadosValidos = { "En espera", "Atendido", "Derivado" };
            if (string.IsNullOrWhiteSpace(hospital.Estado) || !estadosValidos.Contains(hospital.Estado))
            {
                return BadRequest(new { error = "El estado debe ser: En espera, Atendido o Derivado." });
            }

            // Validar Autorización del Médico
            if (string.IsNullOrWhiteSpace(hospital.MedicoResponsable) || !MedicosAutorizados.Contains(hospital.MedicoResponsable))
            {
                return Unauthorized(new { error = "El carnet del médico no está autorizado." });
            }

            _context.Entry(hospital).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(string id)
        {
            var hospital = await _context.Hospital20453.FindAsync(id);
            if (hospital == null) return NotFound();

            _context.Hospital20453.Remove(hospital);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
