using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;




namespace DoorsWeb.Shared.Models
{
    public class DoorStatus
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("DoorHeader")]
        public DoorHeader Header { get; set; } = null!;
        public bool RelayA { get; set; }
        public bool RelayB { get; set; }
        public bool TwoStageRelease { get; set; }
        public bool RequestToExit { get; set; }
        public bool DoorPosition { get; set; } // Feedback
        public bool Interlocked { get; set; }

        //The following are alarms
        public bool AlarmDoorForced { get; set; }
        public bool AlarmDuress { get; set; }
        public bool AlarmHacker { get; set; }
        public bool AlarmFire { get; set; }
        public bool AlarmIntruder { get; set; }
        public bool AlarmObstruction { get; set; } //Failed to Close
        public DateTime LastUpdatedAt { get; set; }
    }
}
