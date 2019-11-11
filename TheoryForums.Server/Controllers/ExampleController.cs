using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheoryForums.Shared.Data;
using TheoryForums.Shared.Models;

namespace TheoryForums.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private readonly DataContext _context;

        public ExampleController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Threads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thread>>> GetThreads()
        {
            return await _context.Threads.ToListAsync();
        }

        // GET: api/Threads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Thread>> GetThread(int id)
        {
            var thread = await _context.Threads.FindAsync(id);

            if (thread == null)
            {
                return NotFound();
            }

            return thread;
        }

        // PUT: api/Threads/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutThread(int id, Thread thread)
        {
            if (id != thread.Id)
            {
                return BadRequest();
            }

            _context.Entry(thread).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThreadExists(id))
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

        // POST: api/Threads
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Thread>> PostThread(Thread thread)
        {
            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetThread", new { id = thread.Id }, thread);
        }

        // DELETE: api/Threads/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Thread>> DeleteThread(int id)
        {
            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                return NotFound();
            }

            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();

            return thread;
        }

        private bool ThreadExists(int id)
        {
            return _context.Threads.Any(e => e.Id == id);
        }
    }
}
