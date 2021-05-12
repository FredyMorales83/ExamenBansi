using Comun.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WsApiexamen.Models;

namespace WsApiexamen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamenesController : ControllerBase
    {
        private readonly BdiExamenContext _context;

        public ExamenesController(BdiExamenContext context)
        {
            _context = context;
        }

        // GET: api/Examenes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Examen>>> GetExamenes()
        {
            return await _context.Examenes.ToListAsync();
        }

        //// GET: api/Examenes/Consultar/5
        [HttpGet("Consultar/{id}")]
        public async Task<ActionResult<Examen>> GetExamen(int id)
        {
            var examen = await _context.Examenes.FindAsync(id);

            if (examen == null)
            {
                return NotFound();
            }

            return examen;
        }

        // GET: api/Examenes/Consultar?nombre={nombre}&descripcion={descripcion}
        [HttpGet("Consultar/")]
        public async Task<ActionResult<IEnumerable<Examen>>> GetExamen(string nombre, string descripcion)
        {
            var examenes = await _context.Examenes.Where(e => e.Nombre.Equals(nombre) && e.Descripcion.Equals(descripcion)).ToListAsync();

            if (examenes == null)
            {
                return NotFound();
            }

            return examenes;
        }

        // PUT: api/Examenes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamen(int id, Examen examen)
        {
            if (id != examen.IdExamen)
            {
                return BadRequest();
            }

            _context.Entry(examen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Examenes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Examen>> PostExamen(Examen examen)
        {
            _context.Examenes.Add(examen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExamen", new { id = examen.IdExamen }, examen);
        }

        // DELETE: api/Examenes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamen(int id)
        {
            var examen = await _context.Examenes.FindAsync(id);
            if (examen == null)
            {
                return NotFound();
            }

            _context.Examenes.Remove(examen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamenExists(int id)
        {
            return _context.Examenes.Any(e => e.IdExamen == id);
        }
    }
}
