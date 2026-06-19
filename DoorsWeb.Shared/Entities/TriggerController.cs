using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TriggerController
{
    public int Code { get; set; }

    public int ControllerCode { get; set; }

    public int InputIndex { get; set; }
}

