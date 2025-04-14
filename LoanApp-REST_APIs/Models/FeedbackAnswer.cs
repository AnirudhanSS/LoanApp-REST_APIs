using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class FeedbackAnswer
{
    public int AnswerId { get; set; }

    public int SubmissionId { get; set; }

    public int QuestionId { get; set; }

    public string Answer { get; set; } = null!;

    public virtual FeedbackQuestion Question { get; set; } = null!;

    public virtual FeedbackSubmission Submission { get; set; } = null!;
}
