namespace FinAxisLeaseBudgeting.Models
{
    public class ExpiringLeaseDto
    {
        public string LeaseId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string PropertyId { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public DateTime? LeaseEndDate { get; set; }
        public decimal? ContractRent { get; set; }
        public string? LeaseStatus { get; set; }
    }

    public class VacantUnitDto
    {
        public string UnitId { get; set; } = string.Empty;
        public string UnitCode { get; set; } = string.Empty;
        public string PropertyId { get; set; } = string.Empty;
        public string? UnitType { get; set; }
        public string? UnitStatus { get; set; }
        public decimal? Area { get; set; }
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public decimal? MarketRent { get; set; }
    }

    public class MarketRentDto
    {
        public string UnitId { get; set; } = string.Empty;
        public string UnitCode { get; set; } = string.Empty;
        public string PropertyId { get; set; } = string.Empty;
        public string? UnitType { get; set; }
        public string? UnitStatus { get; set; }
        public decimal? Area { get; set; }
        public decimal? MarketRent { get; set; }
    }

    public class BudgetAssumptionDto
    {
        public long AssumptionId { get; set; }
        public string? EntityId { get; set; }
        public string? PropertyId { get; set; }
        public string? BuildingId { get; set; }
        public string? UnitId { get; set; }
        public string? LeaseId { get; set; }
        public string? TenantId { get; set; }
        public string AssumptionName { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public List<BudgetAssumptionDetailDto> Details { get; set; } = new();
    }

    public class BudgetAssumptionDetailDto
    {
        public long AssumptionDetailId { get; set; }
        public string AssumptionType { get; set; } = string.Empty;
        public string CalculationMethod { get; set; } = string.Empty;
        public decimal? AssumptionValue { get; set; }
        public string? ValueText { get; set; }
        public DateOnly? EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public short SortOrder { get; set; }
    }

}
