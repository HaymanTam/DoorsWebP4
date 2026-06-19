using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Users
{
    public int Code { get; set; }

    public string Description { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Administrator { get; set; }
}

