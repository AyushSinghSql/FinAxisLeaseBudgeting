using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("unit_master")]
    public class UnitMaster
    {
        [Key]
        [Column("unit_id")]
        public string UnitId { get; set; } = string.Empty; // FIXED: int -> string

        [Column("unit_code")]
        public string UnitCode { get; set; } = string.Empty;

        [Column("property_id")]
        public string PropertyId { get; set; } = string.Empty; // FIXED: int -> string

        [Column("unit_type")]
        public string? UnitType { get; set; }

        [Column("unit_status")]
        public string? UnitStatus { get; set; }

        [Column("area")]
        public decimal? Area { get; set; }

        [Column("bedrooms")]
        public int? Bedrooms { get; set; }

        [Column("bathrooms")]
        public int? Bathrooms { get; set; }

        [Column("building")]
        public string? Building { get; set; }

        [Column("floor")]
        public string? Floor { get; set; }

        [Column("zone")]
        public string? Zone { get; set; }

        [Column("market_rent")]
        public decimal? MarketRent { get; set; }

        [Column("inception_date")]
        public DateTime? InceptionDate { get; set; }

        [Column("forecast_termination_date")]
        public DateTime? ForecastTerminationDate { get; set; } // FIXED: string? -> DateTime? (matches date type)

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
    }

    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}