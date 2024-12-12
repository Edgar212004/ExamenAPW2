using System;
using System.Collections.Generic;

namespace APW2.Data.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? AssignedTo { get; set; }

    public int? Estimation { get; set; }

    public string? State { get; set; }
}
