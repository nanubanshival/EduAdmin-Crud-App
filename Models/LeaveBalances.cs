using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class LeaveBalances
    {
        [Key]
        public int LeaveBalanceId {  get; set; }
        public int TeacherId { get; set; }
        public int LeaveTypeId {  get; set; }
        public int Year { get; set; }
        public int TotalLeaves { get; set; }
        public int UsedLeaves {  get; set; }
        public int RemainingLeaves { get; set; }
    }
}
