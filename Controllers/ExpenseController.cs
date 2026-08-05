using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly FinAxisDbContext _context;

        public ExpenseController(FinAxisDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAll()
        {
            var result = await _context.Accounts
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.AccountCode)
                .Select(x => new AccountDTO
                {
                    AccountId = x.AccountId,
                    AccountCode = x.AccountCode,
                    AccountName = x.AccountName,
                    AccountType = x.AccountType,
                    ExpenseCategory = x.ExpenseCategory ?? string.Empty,
                    DriverType = x.DriverType ?? string.Empty,
                    CalculationMethod = x.CalculationMethod ?? string.Empty
                })
                .ToListAsync();

            return Ok(result);
        }
        [HttpGet]
        [Route("GetExpenseAccounts")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetExpenseAccounts()
        {
            var result = await _context.Accounts
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.AccountType == "Expense")
                .OrderBy(x => x.AccountCode)
                .Select(x => new AccountDTO
                {
                    AccountId = x.AccountId,
                    AccountCode = x.AccountCode,
                    AccountName = x.AccountName,
                    AccountType = x.AccountType,
                    ExpenseCategory = x.ExpenseCategory ?? string.Empty,
                    DriverType = x.DriverType ?? string.Empty,
                    CalculationMethod = x.CalculationMethod ?? string.Empty
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetExpenseAccountDropdown")]
        public async Task<ActionResult<IEnumerable<DropdownDto>>> GetExpenseAccountDropdown([FromQuery] string budgetType)
        {
            List<DropdownDto> result = new List<DropdownDto>();
            var normalizedBudgetType = budgetType?.ToLower();

            if (normalizedBudgetType == "Expense")
            {
                result = await _context.Accounts
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        x.AccountType == "expense")
                    .OrderBy(x => x.AccountCode)
                    .Select(x => new DropdownDto
                    {
                        Id = x.AccountId,
                        Name = x.AccountName
                    })
                    .ToListAsync();
            }
            else if (normalizedBudgetType == "revenue")
            {
                result = await _context.ChargeCdGlAccounts
                    .AsNoTracking()
                    .OrderBy(x => x.ChargeCode)
                    .Select(x => new DropdownDto
                    {
                        Id = x.GlAccount,
                        Name = x.ChargeCode
                    })
                    .ToListAsync();
            }
            else
            {
                var accounts = await _context.Accounts
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        x.AccountType == "Expense")
                    .OrderBy(x => x.AccountCode)
                    .Select(x => new DropdownDto
                    {
                        Id = x.AccountId,
                        Name = x.AccountName
                    })
                    .ToListAsync();

                var chargeCodes = await _context.ChargeCdGlAccounts
                    .AsNoTracking()
                    .OrderBy(x => x.ChargeCode)
                    .Select(x => new DropdownDto
                    {
                        Id = x.GlAccount,
                        Name = x.ChargeCode
                    })
                    .ToListAsync();

                result = accounts.Concat(chargeCodes).ToList();
            }

            return Ok(result);
        }


    }
}
