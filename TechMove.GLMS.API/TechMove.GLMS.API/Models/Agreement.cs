using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.GLMS.API.Models
{
    public enum AgreementStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }

    public enum ServiceLevelType
    {
        Standard,
        Premium,
        Enterprise
    }

    public class Agreement
    {
        [Key]
        public int AgreementId { get; set; }

        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public AgreementStatus Status { get; set; } = AgreementStatus.Draft;

        [Required]
        public ServiceLevelType ServiceLevel { get; set; } = ServiceLevelType.Standard;

        [StringLength(500)]
        public string? Terms { get; set; }

        public string? SignedAgreementPath { get; set; }

        public virtual Client? Client { get; set; }
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        [NotMapped]
        public bool IsValidForServiceRequests => Status == AgreementStatus.Active;
    }
}