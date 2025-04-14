using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class LoanVerificationAssignment
{
    public int AssignmentId { get; set; }

    public int RequestId { get; set; }

    public int OfficerId { get; set; }

    public string Status { get; set; } = null!;

    public string? VerificationDetails { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual User Officer { get; set; } = null!;

    public virtual LoanRequest Request { get; set; } = null!;
}
