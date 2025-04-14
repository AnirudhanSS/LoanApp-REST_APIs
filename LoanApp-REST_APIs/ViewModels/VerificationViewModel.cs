namespace LoanApp_REST_APIs.ViewModels
{
    public class VerificationViewModel
    {
        public int AssignmentId { get; set; }
        public int RequestId { get; set; }
        public int OfficerId { get; set; }
        public string OfficerName { get; set; }
        public string Status { get; set; }
        public string? VerificationDetails { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
