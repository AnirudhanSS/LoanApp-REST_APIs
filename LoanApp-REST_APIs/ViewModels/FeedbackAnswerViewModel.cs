namespace LoanApp_REST_APIs.ViewModels
{
    public class FeedbackAnswerViewModel
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
    }
}
