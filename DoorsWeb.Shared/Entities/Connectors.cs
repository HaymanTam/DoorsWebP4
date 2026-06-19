using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Connectors
{
    public int Connector { get; set; }

    public int? Site { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public bool? Inuse { get; set; }

    public string? Path { get; set; }

    public string? Status { get; set; }

    public int? ComPort { get; set; }

    public bool? ForceDistrib { get; set; }

    public bool? ForceTimeSync { get; set; }

    public string? Telnumber { get; set; }

    public int? RetryInterval { get; set; }

    public int? Retries { get; set; }

    public string? Pabx { get; set; }

    public int? ConnType { get; set; }

    public int? PingFrequency { get; set; }

    public int? CommandFrequency { get; set; }

    public int? ForcePing { get; set; }

    public bool? DownloadLogsWhenUpdating { get; set; }

    public bool? Encrypted { get; set; }

    public string? Ipaddress { get; set; }

    public int? ModemConnectionFrequency { get; set; }

    public int? ModemConnectionPeriod { get; set; }

    public string? ModemConnectionTime { get; set; }

    public bool? ModemUploadCommands { get; set; }

    public bool? ModemDownloadLogs { get; set; }

    public bool? ModemStayConnected { get; set; }
}

