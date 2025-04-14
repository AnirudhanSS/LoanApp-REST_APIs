namespace LoanApp_REST_APIs.ViewModels
{
    public class FeedbackCreateViewModel
    {
        public int CustomerId { get; set; }
        public List<FeedbackAnswerCreateViewModel> Answers { get; set; }
    }
}
