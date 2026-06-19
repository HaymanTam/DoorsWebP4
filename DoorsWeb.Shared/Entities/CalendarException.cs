using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class CalendarException
{
    public int Code { get; set; }

    public DateTime ExceptionDate { get; set; }
}

