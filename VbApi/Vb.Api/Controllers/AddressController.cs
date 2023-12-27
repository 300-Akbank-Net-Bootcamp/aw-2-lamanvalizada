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
    public class AddressController : ControllerBase
    {
        private readonly VbDbContext dbContext;

        public AddressController(VbDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<Address>> Get()
        {
            return await dbContext.Set<Address>()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Address> Get(int id)
        {
            var address = await dbContext.Set<Address>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return address;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await dbContext.Set<Address>().AddAsync(address);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = address.Id }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            try
            {
                var fromdb = await dbContext.Set<Address>().Where(x => x.Id == id).FirstOrDefaultAsync();

                if (fromdb != null)
                {
                    fromdb.Address1 = address.Address1;
                    fromdb.Address2 = address.Address2;
                    fromdb.Country = address.Country;
                    fromdb.City = address.City;
                    fromdb.County = address.County;
                    fromdb.PostalCode = address.PostalCode;
                    fromdb.IsDefault = address.IsDefault;

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
            var fromdb = await dbContext.Set<Address>().Where(x => x.Id == id).FirstOrDefaultAsync();

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