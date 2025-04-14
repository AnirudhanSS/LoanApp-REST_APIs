using System;
using System.Collections.Generic;

namespace LoanApp_REST_APIs.Models;

public partial class FeedbackQuestion
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<FeedbackAnswer> FeedbackAnswers { get; set; } = new List<FeedbackAnswer>();
}
