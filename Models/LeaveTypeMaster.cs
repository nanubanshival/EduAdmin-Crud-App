using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class LeaveTypeMaster
    {
        [Key]
        public int LeaveTypeId {  get; set; }
        public string LeaveTypeName {  get; set; }
        public int MaxLeavesPerYear {  get; set; }
        public bool IsActive {  get; set; }
    }
}
