using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class FeedbackSubmission
{
    public int SubmissionId { get; set; }

    public int CustomerId { get; set; }

    public DateTime SubmissionDate { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<FeedbackAnswer> FeedbackAnswers { get; set; } = new List<FeedbackAnswer>();
}
