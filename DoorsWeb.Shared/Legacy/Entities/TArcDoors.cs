using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TArcDoors
{
    public string? Name { get; set; }

    public int Door { get; set; }

    public string? Key { get; set; }

    public string? ControllerId { get; set; }

    public int? Site { get; set; }

    public int? Connector { get; set; }

    public int? LocalDoorNumber { get; set; }

    public bool? Inuse { get; set; }

    public string? Rtctime { get; set; }

    public string? Rtcdate { get; set; }

    public int? LogCount { get; set; }

    public int? VdiskDirectories { get; set; }

    public int? LogUpdateInterval1 { get; set; }

    public int? LogUpdateInterval2 { get; set; }

    public int? StatusUpdateInterval1 { get; set; }

    public int? StatusUpdateInterval2 { get; set; }

    public int? DoorType { get; set; }

    public int? ReleaseTime { get; set; }

    public int? ReleaseDelay { get; set; }

    public int? Pdo { get; set; }

    public int? TechnologyA { get; set; }

    public int? RelayBmode { get; set; }

    public int? LockDriveMode { get; set; }

    public bool? LogInA { get; set; }

    public bool? LogoutA { get; set; }

    public int? TechnologyB { get; set; }

    public bool? LogInB { get; set; }

    public bool? LogoutB { get; set; }

    public bool? CarIn { get; set; }

    public bool? CarOut { get; set; }

    public int? KeyboardTech { get; set; }

    public bool? AutoRelock { get; set; }

    public int? RelayBtimeZone { get; set; }

    public int? CardandPintimeZone { get; set; }

    public bool? ControllerIp { get; set; }

    public string? DoorIpaddress { get; set; }

    public int? AutoDelayVal { get; set; }

    public int? ReleaseTimeB { get; set; }

    public int? TimeLock { get; set; }

    public int? LastCard { get; set; }

    public int? LastEvent { get; set; }

    public string? LastCardId { get; set; }

    public DateTime? LastDate { get; set; }

    public int? Status1 { get; set; }

    public int? Status2 { get; set; }

    public float? Xplace { get; set; }

    public float? Yplace { get; set; }

    public int? PlanNumber { get; set; }

    public int? AlarmZoneNumber { get; set; }

    public DateTime? Modified { get; set; }

    public DateTime? Updated { get; set; }

    public string? ReaderAname { get; set; }

    public string? ReaderBname { get; set; }

    public string? KeyboardName { get; set; }

    public int? FloorPlan { get; set; }

    public int? FloorPlanX { get; set; }

    public int? FloorPlanY { get; set; }

    public int? KeypadStarMode { get; set; }

    public int? RandomSearchFreq { get; set; }

    public int? ConFbVolume { get; set; }

    public int? ConAlmVolume { get; set; }

    public int? AccessCodeLen { get; set; }

    public int? AccessCodeDig1 { get; set; }

    public int? AccessCodeDig2 { get; set; }

    public int? AccessCodeDig3 { get; set; }

    public int? AccessCodeDig4 { get; set; }

    public int? AccessCodeDig5 { get; set; }

    public int? AccessCodeDig6 { get; set; }

    public int? AccessCodeDig7 { get; set; }

    public int? AccessCodeDig8 { get; set; }

    public bool? BioEnrolA { get; set; }

    public bool? BioEnrolB { get; set; }

    public int? RdrBrightnessA { get; set; }

    public int? RdrBrightnessB { get; set; }

    public int? RdrVolumeA { get; set; }

    public int? RdrVolumeB { get; set; }

    public int? IdSequenceA { get; set; }

    public int? IdSequenceB { get; set; }

    public int? ValidFromTimeHh { get; set; }

    public int? ValidFromTimeMm { get; set; }

    public int? ValidToTimeHh { get; set; }

    public int? ValidToTimeMm { get; set; }

    public string? Notes { get; set; }
}

