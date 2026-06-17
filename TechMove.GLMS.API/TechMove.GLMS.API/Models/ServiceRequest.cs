using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.GLMS.API.Models
{
    public enum RequestStatusType
    {
        Pending,
        Approved,
        InProgress,
        Completed,
        Cancelled
    }

    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestId { get; set; }

        [Required]
        [ForeignKey("Agreement")]
        public int AgreementId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountUSD { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountZAR { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Required]
        public RequestStatusType Status { get; set; } = RequestStatusType.Pending;

        [Column(TypeName = "decimal(10,4)")]
        public decimal ExchangeRateUsed { get; set; }

        public virtual Agreement? Agreement { get; set; }
    }
}