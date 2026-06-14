using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class AccessLevelNameListDto
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
