using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Domain.Base
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
