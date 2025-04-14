namespace LoanApp_REST_APIs.ViewModels
{
    public class LoanRequestViewModel
    {
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? AssignedOfficerId { get; set; }
        public string? AssignedOfficerName { get; set; }
        public string LoanType { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Details { get; set; }
    }
}
