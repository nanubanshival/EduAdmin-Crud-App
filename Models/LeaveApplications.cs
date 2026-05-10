using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace CrudProject.Models
{
    public class LeaveApplications
    {
        [Key]
        public int ApplicationId { get; set; }
        public int TeacherId {  get; set; }
        public int LeaveTypeId { get; set; }
        public DateOnly FromDate {  get; set; }
        public DateOnly ToDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string? Status { get; set; }
        public DateOnly? AppliedDate {  get; set; }
        public int? ApprovedBy { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public string? RejectionReason {  get; set; }
        public DateOnly? ModifiedDate { get; set; }
    }
}
