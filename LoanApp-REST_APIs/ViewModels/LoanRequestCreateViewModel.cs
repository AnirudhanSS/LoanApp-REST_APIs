namespace LoanApp_REST_APIs.ViewModels
{
    public class LoanRequestCreateViewModel
    {
        public int CustomerId { get; set; }
        public int? AssignedOfficerId { get; set; }
        public string LoanType { get; set; }
        public decimal Amount { get; set; }
        public string? Details { get; set; }
    }
}
