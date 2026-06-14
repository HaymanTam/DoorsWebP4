using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class DoorListDto
    {
        public Guid Id { get; set; }
        public int ControllerId { get; set; }
        public string Name { get; set; } = null!;
        public string IPAddressString { get; set; } = null!;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

}
