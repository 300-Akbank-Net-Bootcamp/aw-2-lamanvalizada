using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vb.Base.Entity;
using Vb.Data.Entity;
using Vb.Data;

namespace VbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly VbDbContext dbContext;

        public AccountController(VbDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<Account>> Get()
        {
            return await dbContext.Set<Account>()
                .Include(a => a.AccountTransactions)
                .Include(a => a.EftTransactions)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Account> Get(int id)
        {
            var account = await dbContext.Set<Account>()
                .Include(a => a.AccountTransactions)
                .Include(a => a.EftTransactions)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return account;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await dbContext.Set<Account>().AddAsync(account);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = account.Id }, account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            try
            {
                var fromdb = await dbContext.Set<Account>().Where(x => x.Id == id).FirstOrDefaultAsync();

                if (fromdb != null)
                {
                    fromdb.CustomerId = account.CustomerId;
                    fromdb.AccountNumber = account.AccountNumber;
                    fromdb.IBAN = account.IBAN;
                    fromdb.Balance = account.Balance;
                    fromdb.CurrencyType = account.CurrencyType;
                    fromdb.Name = account.Name;
                    fromdb.OpenDate = account.OpenDate;

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
            var fromdb = await dbContext.Set<Account>().Where(x => x.Id == id).FirstOrDefaultAsync();

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