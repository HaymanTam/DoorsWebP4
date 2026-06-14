

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace DoorsWeb.Shared.Models
{
    [Index(nameof(ControllerId), IsUnique = true)]
    public class DoorHeader
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int ControllerId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [MaxLength(16)]
        public byte[] IPAddressBytes { get; set; } = null!;

        // Helper property
        [NotMapped]
        public IPAddress IPAddress
        {
            get => new IPAddress(IPAddressBytes);
            set => IPAddressBytes = value.GetAddressBytes();
        }

        [NotMapped]
        public string IPAddressString
        {
            get 
            {
                IPAddress address = new IPAddress(IPAddressBytes);
                return address.ToString();
            }
            set
            {
                IPAddress address = IPAddress.Parse(value);
                IPAddressBytes = address.GetAddressBytes();
            } 
        }
        public DoorSettings? Settings { get; set; }
        public DoorStatus? Status { get; set; }
        public ICollection<AccessLevel> AccessLevels { get; set; } = new List<AccessLevel>();
        public ICollection<AccessLevelElement> ALElements { get; set; } = new List<AccessLevelElement>();
        public DateTime LastUpdatedAt { get; set; }
    }
}
