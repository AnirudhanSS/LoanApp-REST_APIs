using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BackgroundVerificationAssignment> BackgroundVerificationAssignments { get; set; } = new List<BackgroundVerificationAssignment>();

    public virtual ICollection<FeedbackSubmission> FeedbackSubmissions { get; set; } = new List<FeedbackSubmission>();

    public virtual ICollection<LoanRequest> LoanRequestAssignedOfficers { get; set; } = new List<LoanRequest>();

    public virtual ICollection<LoanRequest> LoanRequestCustomers { get; set; } = new List<LoanRequest>();

    public virtual ICollection<LoanVerificationAssignment> LoanVerificationAssignments { get; set; } = new List<LoanVerificationAssignment>();

    public virtual Role Role { get; set; } = null!;
}
