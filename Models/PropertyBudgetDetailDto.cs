namespace FinAxisLeaseBudgeting.Models
{
    public class PropertyBudgetDetailDto
    {
        // Core Layout Metrics
        public string Property { get; set; } = string.Empty;
        public string Book { get; set; } = "Budget 25";
        public string BudgetDates { get; set; } = "1/2025 - 12/2025";
        public string ModelProperty { get; set; } = string.Empty;
        public string CurrencyArea { get; set; } = "inr |";
        public string MarketType { get; set; } = "Commercial";
        public string ForecastMonth { get; set; } = "01/2025";
        public string AccountTree { get; set; } = "ysi_bf";
        public string Workflow { get; set; } = "";
        public string Status { get; set; } = "";
        public string AccountingMethod { get; set; } = "Accrual";
        public string NextStep { get; set; } = "";
        public string BudgetNotes { get; set; } = "";
        public string Password { get; set; } = "";
        public string SearchQuery { get; set; } = "";
        public string EntriesCount { get; set; } = "10";
        public string ReviewCode { get; set; } = "soptest:Budget 25:2025-1";

        // Tab 1: Budget Info
        public string PropName { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = "";
        public string City { get; set; } = "";
        public string StateZip { get; set; } = "";
        public string Country { get; set; } = "in";
        public string AttributeSet { get; set; } = "";
        public string Attribute { get; set; } = "";
        public string AttributeValue { get; set; } = "";
        public string UnitStatus { get; set; } = "Applied";
        public string UnitConfig { get; set; } = "Review";
        public string LeasingOptions { get; set; } = "Review";
        public string Exceptions { get; set; } = "0";
        public string LastModified { get; set; } = "";
        public string LastCommercialForecast { get; set; } = "01/20/2025 5:47 AM";
        public string LastRefresh { get; set; } = "01/16/2025 9:29 AM";

        // Tab 2: Rentable Items
        public List<RentableItemDto> RentableItems { get; set; } = new();

        // Tab 3: Debt
        public List<DebtItemDto> DebtItems { get; set; } = new()
        {
            new DebtItemDto
            {
                Id = 1,
                Debt = "Senior Term Loan A",
                Type = "Fixed Mortgage",
                StartDate = "01/01/2025",
                PostDate = "01/15/2025",
                Post = "Yes",
                Unpost = "No"
            }
        };

        // Tab 4: Jobs
        public List<JobItemDto> JobItems { get; set; } = new()
        {
            new JobItemDto
            {
                Id = 1,
                Code = "JOB-204",
                Desc = "HVAC Boiler Replacement Phase 1",
                Q = "Q1",
                Status = "Active",
                Type = "Capital Improvement",
                FromDate = "02/01/2025",
                ToDate = "05/30/2025",
                ProjCost = "145000.00",
                FcstNum = "F-02",
                FcstDate = "01/15/2025",
                DetailsDesc = "Quarterly review rollup asset cost tracking",
                DetailsStatus = "Approved",
                PostDate = "01/20/2025",
                CtrlNum = "9482",
                Worksheet = "WS-HVAC-01",
                Post = "Yes",
                Unpost = "No",
                Detach = "No"
            }
        };

        // Tab 5: Roll Up
        public List<RollUpItemDto> RollUpItems { get; set; } = new()
        {
            new RollUpItemDto
            {
                Id = 1,
                CtrlNum = "741",
                SourceBudget = "RETAIL-B25",
                Property = "North Plaza Comms",
                Book = "Budget 25",
                StartDate = "01/01/2025",
                EndDate = "12/31/2025",
                ParentProperty = "SOP Test Consolidation",
                Percent = "100.00",
                AcctTree = "ysi_com_tree"
            }
        };

        // Tab 6: Sources of Data
        public string SortBy { get; set; } = "Ctrl. #";
        public List<SourceOfDataDto> SourcesOfData { get; set; } = new()
        {
            new SourceOfDataDto
            {
                Id = 1,
                Checked = false,
                CtrlNum = "113",
                SourceType = "Commercial Revenue Forecast",
                Desc = ":Commercial Revenue Projection",
                FuncGroup = "",
                User = "y1000003",
                Date = "01/20/2025 5:47 AM"
            }
        };

        // Tab 7: Workflow History
        public List<WorkflowHistoryDto> WorkflowHistory { get; set; } = new()
        {
            new WorkflowHistoryDto
            {
                Id = 1,
                Workflow = "Standard Review",
                Step = "Initial Forecast Build",
                Status = "Completed",
                ApprovedBy = "y1000003",
                Notes = "System generation based on operational model run variables.",
                StartDate = "01/16/2025",
                StartTime = "09:00 AM",
                CompleteDate = "01/16/2025",
                CompleteTime = "09:29 AM"
            }
        };

        // Tab 8: Workflow Approval
        public List<WorkflowApprovalDto> WorkflowApproval { get; set; } = new()
        {
            new WorkflowApprovalDto
            {
                Id = 1,
                Approver = "Jonathan Vance",
                Roles = "Regional Finance Director",
                Steps = "Step 2 - Executive Verification Tier",
                Preferred = "Yes"
            }
        };
    }

    public class RentableItemDto
    {
        public int Id { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string TypeCode { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string MarketRent { get; set; } = string.Empty;
        public string OccTable { get; set; } = "OCC_STD";
        public string Items { get; set; } = "1";
        public string ChargeCode { get; set; } = "PRK_CHG";
        public string GlAccount { get; set; } = "4100-02";
        public string InfMethod { get; set; } = "Fixed %";
        public string InfTable { get; set; } = "INF_2026";
        public string InfRate { get; set; } = "3.5";
    }

    public class DebtItemDto
    {
        public int Id { get; set; }
        public string Debt { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string PostDate { get; set; } = string.Empty;
        public string Post { get; set; } = string.Empty;
        public string Unpost { get; set; } = string.Empty;
    }

    public class JobItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string Q { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string ProjCost { get; set; } = string.Empty;
        public string FcstNum { get; set; } = string.Empty;
        public string FcstDate { get; set; } = string.Empty;
        public string DetailsDesc { get; set; } = string.Empty;
        public string DetailsStatus { get; set; } = string.Empty;
        public string PostDate { get; set; } = string.Empty;
        public string CtrlNum { get; set; } = string.Empty;
        public string Worksheet { get; set; } = string.Empty;
        public string Post { get; set; } = string.Empty;
        public string Unpost { get; set; } = string.Empty;
        public string Detach { get; set; } = string.Empty;
    }

    public class RollUpItemDto
    {
        public int Id { get; set; }
        public string CtrlNum { get; set; } = string.Empty;
        public string SourceBudget { get; set; } = string.Empty;
        public string Property { get; set; } = string.Empty;
        public string Book { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string ParentProperty { get; set; } = string.Empty;
        public string Percent { get; set; } = string.Empty;
        public string AcctTree { get; set; } = string.Empty;
    }

    public class SourceOfDataDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
        public string CtrlNum { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string FuncGroup { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class WorkflowHistoryDto
    {
        public int Id { get; set; }
        public string Workflow { get; set; } = string.Empty;
        public string Step { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string CompleteDate { get; set; } = string.Empty;
        public string CompleteTime { get; set; } = string.Empty;
    }

    public class WorkflowApprovalDto
    {
        public int Id { get; set; }
        public string Approver { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty;
        public string Preferred { get; set; } = string.Empty;
    }
}
