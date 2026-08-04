using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("org_account")]
    public class Account
    {
        [Key]
        [Column("account_id")]
        public string AccountId { get; set; } = string.Empty;

        [Column("account_code")]
        public string AccountCode { get; set; } = string.Empty;

        [Column("account_name")]
        public string AccountName { get; set; } = string.Empty;

        [Column("account_type")]
        public string AccountType { get; set; } = string.Empty;

        [Column("account_category")]
        public string? AccountCategory { get; set; }

        [Column("budget_sheet")]
        public string? BudgetSheet { get; set; }

        [Column("department_id")]
        public string? DepartmentId { get; set; }

        [Column("cost_center_id")]
        public string? CostCenterId { get; set; }

        [Column("expense_category")]
        public string? ExpenseCategory { get; set; }

        [Column("driver_type")]
        public string? DriverType { get; set; }

        [Column("calculation_method")]
        public string? CalculationMethod { get; set; }

        [Column("seasonality_id")]
        public string? SeasonalityId { get; set; }

        [Column("allow_manual_override")]
        public bool AllowManualOverride { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }

    public class AccountDTO
    {
        public string AccountId { get; set; } = string.Empty;

        public string AccountCode { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public string ExpenseCategory { get; set; } = string.Empty;

        public string DriverType { get; set; } = string.Empty;

        public string CalculationMethod { get; set; } = string.Empty;
    }

    public class DropdownDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
