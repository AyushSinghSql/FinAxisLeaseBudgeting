namespace FinAxisLeaseBudgeting.Tools
{
    public static class ToolCatalog
    {
        public static string GetFormattedToolCatalog()
        {
            return """
                **Available Tools & Supported Capabilities:**

                1. **Budget Assumptions Tool**
                   - **What it does:** Retrieves budget growth percentages, inflation defaults, market rent increases, and operational assumptions.
                   - **Example Prompts:** 
                     - *"What are the budget assumption defaults for 2026?"*
                     - *"Show me assumptions for property Burjuman Centre 1."*

                2. **Market Rent Units Tool**
                   - **What it does:** Fetches current market rent rates, unit types, square footage, and pricing strategies.
                   - **Example Prompts:** 
                     - *"What is the market rent for 2-bedroom units?"*
                     - *"List current market rates for all vacant units."*

                3. **Expiring Leases Tool**
                   - **What it does:** Tracks leases expiring within specified timeframe windows and tenant details.
                   - **Example Prompts:** 
                     - *"Which leases are expiring in the next 90 days?"*
                     - *"Show expiring tenant leases for Property Burjuman Centre 1."*

                4. **Vacant Units Tool**
                   - **What it does:** Queries available unoccupied units across entities and properties.
                   - **Example Prompts:** 
                     - *"How many vacant units?"*
                     - *"Show all unleased commercial spaces."*

                5. **Master Data Tool**
                   - **What it does:** Provides structural master data, including Entity IDs, Property lists, and Unit mappings.
                   - **Example Prompts:** 
                     - *"List all properties assigned to Entity Burjuman center LLC."*
                     - *"What entity maps to Property Burjuman Centre 1?"*
                """;
        }
    }
}