namespace LoanApp_REST_APIs.ViewModels
{
    public class FeedbackSubmissionViewModel
    {
        public int SubmissionId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public List<FeedbackAnswerViewModel> Answers { get; set; }
    }
}
