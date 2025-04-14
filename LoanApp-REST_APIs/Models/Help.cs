using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class Help
{
    public int HelpId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
