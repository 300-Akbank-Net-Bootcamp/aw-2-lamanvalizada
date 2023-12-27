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
    public class EftTransactionController : ControllerBase
    {
        private readonly VbDbContext dbContext;

        public EftTransactionController(VbDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<EftTransaction>> Get()
        {
            return await dbContext.Set<EftTransaction>()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<EftTransaction> Get(int id)
        {
            var eftTransaction = await dbContext.Set<EftTransaction>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return eftTransaction;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EftTransaction eftTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await dbContext.Set<EftTransaction>().AddAsync(eftTransaction);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = eftTransaction.Id }, eftTransaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EftTransaction eftTransaction)
        {
            if (id != eftTransaction.Id)
            {
                return BadRequest();
            }

            try
            {
                var fromdb = await dbContext.Set<EftTransaction>().Where(x => x.Id == id).FirstOrDefaultAsync();

                if (fromdb != null)
                {
                    fromdb.AccountId = eftTransaction.AccountId;
                    fromdb.ReferenceNumber = eftTransaction.ReferenceNumber;
                    fromdb.TransactionDate = eftTransaction.TransactionDate;
                    fromdb.Amount = eftTransaction.Amount;
                    fromdb.Description = eftTransaction.Description;
                    fromdb.SenderAccount = eftTransaction.SenderAccount;
                    fromdb.SenderIban = eftTransaction.SenderIban;
                    fromdb.SenderName = eftTransaction.SenderName;

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
            var fromdb = await dbContext.Set<EftTransaction>().Where(x => x.Id == id).FirstOrDefaultAsync();

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