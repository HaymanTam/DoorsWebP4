using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TBackup
{
    public bool ScheduleOn { get; set; }

    public int ScheduleNumber { get; set; }

    public int ScheduleFrequency { get; set; }

    public string ScheduleTime { get; set; } = null!;

    public bool MaximumOn { get; set; }

    public int MaximumNumber { get; set; }

    public bool DeleteToRecycleBin { get; set; }

    public bool KeepOn { get; set; }

    public int KeepNumber { get; set; }

    public int SaveDays { get; set; }

    public bool IncludePhotos { get; set; }

    public bool IncludePlans { get; set; }

    public DateTime? LastBackup { get; set; }
}

