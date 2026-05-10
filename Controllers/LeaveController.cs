using CrudProject.Data;
using CrudProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudProject.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> LeaveTypes()
        {
            var leaveTypes = await _context.LeaveTypeMasters.ToListAsync();
            return View(leaveTypes);
        }

        public async Task<IActionResult> CreateLeaveType()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveType(LeaveTypeMaster model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var exists = await _context.LeaveTypeMasters
                .AnyAsync(l => l.LeaveTypeName.ToLower() == model.LeaveTypeName.ToLower());

            if (exists)
            {
                ModelState.AddModelError("LeaveTypeName", "This leave type already exists.");
                return View(model);
            }

            _context.LeaveTypeMasters.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LeaveTypes));
        }

        public async Task<IActionResult> EditLeaveType(int id)
        {
            var leaveType = await _context.LeaveTypeMasters.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            return View(leaveType);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeaveType(LeaveTypeMaster model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.LeaveTypeMasters.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(LeaveTypes));
        }


        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            var leaveType = await _context.LeaveTypeMasters.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            _context.LeaveTypeMasters.Remove(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(LeaveTypes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeaveTypeConfirmed(int id)
        {
            var leaveType = await _context.LeaveTypeMasters.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            _context.LeaveTypeMasters.Remove(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(LeaveTypes));
        }


        //Leave Application Pages
        public async Task<IActionResult> LeaveApplications()
        {
            ViewBag.Teachers = await _context.Teachers.ToListAsync();
            ViewBag.LeaveTypes = await _context.LeaveTypeMasters
                                               .Where(x => x.IsActive)
                                               .ToListAsync();

            var leaveApplications = await _context.LeaveApplications.ToListAsync();
            return View(leaveApplications);
        }


        [HttpPost]
        public async Task<IActionResult> LeaveApplications(LeaveApplications model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = await _context.Teachers.ToListAsync();
                ViewBag.LeaveTypes = await _context.LeaveTypeMasters
                                                   .Where(x => x.IsActive)
                                                   .ToListAsync();

                var leaveApplications = await _context.LeaveApplications.ToListAsync();
                return View(leaveApplications);
            }

            model.Status = "Pending";
            model.AppliedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.LeaveApplications.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LeaveApplications));
        }






        //Leave Balance Pages
        public async Task<IActionResult> LeaveBalances()
        {
            int? teacherId = HttpContext.Session.GetInt32("TeacherId");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account"); // safety
            }

            var balances = await _context.LeaveBalances
                                         .Where(x => x.TeacherId == teacherId)
                                         .Join(_context.LeaveTypeMasters,
                                               lb => lb.LeaveTypeId,
                                               lt => lt.LeaveTypeId,
                                               (lb, lt) => new
                                               {
                                                   lt.LeaveTypeName,
                                                   lb.Year,
                                                   lb.TotalLeaves,
                                                   lb.UsedLeaves,
                                                   lb.RemainingLeaves
                                               })
                                         .ToListAsync();

            return View(balances);
        }


    }
}
