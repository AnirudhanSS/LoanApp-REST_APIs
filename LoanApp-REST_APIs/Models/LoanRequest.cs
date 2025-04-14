using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class LoanRequest
{
    public int RequestId { get; set; }

    public int CustomerId { get; set; }

    public int? AssignedOfficerId { get; set; }

    public string LoanType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime RequestDate { get; set; }

    public string? Details { get; set; }

    public virtual User? AssignedOfficer { get; set; }

    public virtual BackgroundVerificationAssignment? BackgroundVerificationAssignment { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual LoanVerificationAssignment? LoanVerificationAssignment { get; set; }
}
