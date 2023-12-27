using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vb.Data;
using Vb.Data.Entity;

namespace VbApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountTransactionController : ControllerBase
{
    private readonly VbDbContext dbContext;

    public AccountTransactionController(VbDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<List<AccountTransaction>> Get()
    {
        return await dbContext.Set<AccountTransaction>()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<AccountTransaction> Get(int id)
    {
        var transaction = await dbContext.Set<AccountTransaction>()
            .Include(x => x.Account)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        return transaction;
    }

    [HttpPost]
    public async Task Post([FromBody] AccountTransaction transaction)
    {
        await dbContext.Set<AccountTransaction>().AddAsync(transaction);
        await dbContext.SaveChangesAsync();
    }

    [HttpPut("{id}")]
    public async Task Put(int id, [FromBody] AccountTransaction transaction)
    {
        var fromdb = await dbContext.Set<AccountTransaction>().Where(x => x.Id == id).FirstOrDefaultAsync();
        
        if (fromdb != null)
        {
            fromdb.AccountId = transaction.AccountId;
            fromdb.ReferenceNumber = transaction.ReferenceNumber;
            fromdb.TransactionDate = transaction.TransactionDate;
            fromdb.Amount = transaction.Amount;
            fromdb.Description = transaction.Description;
            fromdb.TransferType = transaction.TransferType;

            await dbContext.SaveChangesAsync();
        }
    }

    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        var fromdb = await dbContext.Set<AccountTransaction>().Where(x => x.Id == id).FirstOrDefaultAsync();

        if (fromdb != null)
        {
            fromdb.IsActive = false;
            await dbContext.SaveChangesAsync();
        }
    }
}