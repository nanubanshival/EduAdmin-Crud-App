using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }
        public bool Action { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public int TeacherId { get; set; }
        public int MarkedBy { get; set; }
        public bool IsHalfDay { get; set; }
    }
}
