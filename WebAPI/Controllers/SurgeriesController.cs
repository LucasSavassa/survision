using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comuns.Classes;
using WebAPI.Context;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurgeriesController : ControllerBase
    {
        private readonly SurgeryContext _context;

        public SurgeriesController(SurgeryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Surgery>>> GetSurgeries()
        {
            return await _context.Surgeries.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Surgery>> GetSurgery(int id)
        {
            var surgery = await _context.Surgeries.FindAsync(id);

            if (surgery == null)
            {
                return NotFound();
            }

            return surgery;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurgery(int id, Surgery surgery)
        {
            if (id != surgery.Id)
            {
                return BadRequest();
            }

            _context.Entry(surgery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurgeryExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Surgery>> PostSurgery(Surgery surgery)
        {
            _context.Surgeries.Add(surgery);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSurgery", new { id = surgery.Id }, surgery);
        }

        // DELETE: api/Surgeries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurgery(int id)
        {
            var surgery = await _context.Surgeries.FindAsync(id);
            if (surgery == null)
            {
                return NotFound();
            }

            _context.Surgeries.Remove(surgery);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurgeryExists(int id)
        {
            return _context.Surgeries.Any(e => e.Id == id);
        }
    }
}
