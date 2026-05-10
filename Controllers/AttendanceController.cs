using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudProject.Models;
using CrudProject.Data;

namespace CrudProject.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> BulkAttendance(DateOnly? AttendanceDate)
        {
            DateOnly date = AttendanceDate ?? DateOnly.FromDateTime(DateTime.Now);
            var teachers = await _context.Teachers.Where(t => t.IsActive == true)
                .OrderBy(t => t.Name)
                .Select(t => new
                {
                    t.TeacherId,
                    t.Name
                })
                .ToListAsync();

            ViewBag.TodayDate = date;
            ViewBag.Teachers = teachers;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveBulkAttendance(
    DateOnly AttendanceDate,
    int[] TeacherIds
)
        {
            if (AttendanceDate > DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["Error"] = "Future date attendance allowed nahi hai.";
                return RedirectToAction("BulkAttendance");
            }

            int markedBy = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            // Remove old attendance
            var oldAttendance = await _context.Attendances
                .Where(a => a.AttendanceDate == AttendanceDate)
                .ToListAsync();

            if (oldAttendance.Any())
            {
                _context.Attendances.RemoveRange(oldAttendance);
                await _context.SaveChangesAsync();
            }

            foreach (int teacherId in TeacherIds)
            {
                // Read Present/Absent
                string actionValue = Request.Form["Action_" + teacherId];
                bool isPresent = actionValue == "true";

                // Read HalfDay
                bool isHalfDay = Request.Form["HalfDay"].Contains(teacherId.ToString());

                if (!isPresent)
                    isHalfDay = false;

                var attendance = new Attendance
                {
                    TeacherId = teacherId,
                    AttendanceDate = AttendanceDate,
                    Action = isPresent,
                    IsHalfDay = isHalfDay,
                    MarkedBy = markedBy,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Attendance successfully saved.";
            return RedirectToAction("BulkAttendance", new { AttendanceDate });
        }

    }
}