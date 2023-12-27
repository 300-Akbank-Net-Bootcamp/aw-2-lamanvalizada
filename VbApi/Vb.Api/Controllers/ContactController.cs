using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vb.Base.Entity;
using Vb.Data.Entity;
using Vb.Data;

namespace VbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly VbDbContext dbContext;

        public ContactController(VbDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<Contact>> Get()
        {
            return await dbContext.Set<Contact>()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Contact> Get(int id)
        {
            var contact = await dbContext.Set<Contact>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return contact;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            try
            {
                var fromdb = await dbContext.Set<Contact>().Where(x => x.Id == id).FirstOrDefaultAsync();

                if (fromdb != null)
                {
                    fromdb.ContactType = contact.ContactType;
                    fromdb.Information = contact.Information;
                    fromdb.IsDefault = contact.IsDefault;

                    await dbContext.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fromdb = await dbContext.Set<Contact>().Where(x => x.Id == id).FirstOrDefaultAsync();

            if (fromdb == null)
            {
                return NotFound();
            }

            fromdb.IsActive = false;
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}