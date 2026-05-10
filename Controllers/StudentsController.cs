using CrudProject.Data;
using CrudProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace CrudProject.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }


        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View(await _context.Students.OrderByDescending(e => e.StudentId).ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
            {
                TempData["Error"] = "Only Admin has right to do this.";
                return RedirectToAction("Index");
            }

            ViewData["Course"] = _context.CourseManagement.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                TempData["Error"] = "Only Admin has right to do this.";
                return RedirectToAction("Index");
            }

            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            // Remove validation for fields set server-side
            ModelState.Remove("Course");
            ModelState.Remove("Fees");

            // Check for existing User
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(u => u.Email == model.Email || u.Phone == model.Phone);

            if (existingStudent != null)
                ModelState.AddModelError("", "Email or Phone already exist.");
            
            if(string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("Email", "Email is Required");
            }
            else if (!Validation.IsValidEmail(model.Email))
                ModelState.AddModelError("Email", "Invalid Email");

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError("Phone", "Phone is required");
            }
            else if (!Validation.IsValidPhone(model.Phone))
                ModelState.AddModelError("Phone", "Invalid Phone Number");

            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Address is required");
            


            var course = await _context.CourseManagement
                .FirstOrDefaultAsync(k => k.CourseId == model.CourseId);

            if (course == null)
            {
                ViewData["Course"] = _context.CourseManagement.ToList();
                return View(model);
            }


            if (!ModelState.IsValid)
            {
                ViewData["Course"] = _context.CourseManagement.ToList();
                return View(model);
            }

            model.Course = course.CourseName;
            model.Fees = course.Fees;

            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Teacher")
            {
                TempData["Error"] = "Only Admin or Teacher has rights to do this.";
                return RedirectToAction("Index");
            }


            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Course"] = _context.CourseManagement.ToList();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student ,Student Model)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (role != "Admin" && role != "Teacher")
            {
                TempData["Error"] = "Only Admin or Teacher has rights to do this.";
                return RedirectToAction("Index");
            }


            ModelState.Remove("Course");
            ModelState.Remove("Fees");

            if (id != student.StudentId)
            {
                return NotFound();
            }

            if(string.IsNullOrWhiteSpace(Model.Email))
                ModelState.AddModelError("Email","Email is required");
            else if (!Validation.IsValidEmail(student.Email))
                ModelState.AddModelError("Email", "Invalid Email Format");

            if (string.IsNullOrWhiteSpace(Model.Phone))
                ModelState.AddModelError("Phone", "Phone is required");
            else if (!Validation.IsValidPhone(student.Phone))
                ModelState.AddModelError("Phone", "Invalid Phone Number");

            if (string.IsNullOrWhiteSpace(Model.Address))
                ModelState.AddModelError("Address", "Address is required");

            var course = await _context.CourseManagement
                .FirstOrDefaultAsync(k => k.CourseId == student.CourseId);

            if (course == null)
            {
                ModelState.AddModelError("CourseId", "Please select a valid course");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Course"] = _context.CourseManagement.ToList();
                return View(student);
            }

            try
            {
                var existingStudent = await _context.Students.FindAsync(id);
                if (existingStudent == null)
                {
                    return NotFound();
                }

                existingStudent.Name = student.Name;
                existingStudent.Email = student.Email;
                existingStudent.CourseId = student.CourseId;
                existingStudent.Course = course.CourseName;
                existingStudent.Fees = course.Fees;
                existingStudent.EnrollmentDate = student.EnrollmentDate;
                existingStudent.Phone = student.Phone;
                existingStudent.Address = student.Address;
                existingStudent.Semester = student.Semester;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.StudentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                TempData["Error"] = "Only Admin has rights to do this.";
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }


        [HttpGet]
        public IActionResult GetCoursesBySemester(string semester)
        {
            var courses = _context.CourseManagement
                .Where(c => c.Semester == semester)
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName
                })
                .ToList();

            return Json(courses);
        }


        [HttpGet]
        public IActionResult GetCourseDetails(int courseId)
        {
            var course = _context.CourseManagement
                                 .FirstOrDefault(c => c.CourseId == courseId);

           

            if (course == null)
                return Json(0);

            var teachers = (from m in _context.TeacherCourseMapping
                            join t in _context.Teachers
                                on m.TeacherId equals t.TeacherId
                            where m.CourseId == courseId
                                  && m.IsActive == true
                                  && t.IsActive == true
                            select t.Name).ToList();

            return Json(new
            {
                fees = course.Fees,
                Teachers = teachers
            });
        }


        public class Validation
        {
            public static bool IsValidEmail(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }
                for (int i = 0; i < email.Length; i++)
                {
                    if (email[i] == ' ')
                        return false;
                }

                int atCnt = 0;
                int atIndex = -1;

                for (int i = 0; i < email.Length; i++)
                {
                    if (email[i] == '@')
                    {
                        atCnt++;
                        atIndex = i;
                    }
                }

                if (atCnt != 1)
                    return false;

                if (atIndex == 0 || atIndex == email.Length - 1)
                    return false;

                bool dotFound = false;

                for (int i = atIndex + 1; i < email.Length; i++)
                {
                    if (email[i] == '.')
                    {
                        dotFound = true;
                        break;
                    }
                }

                if (!dotFound)
                    return false;

                if (email[atIndex + 1] == '.' || email[email.Length - 1] == '.')
                    return false;

                if (!char.IsLetterOrDigit(email[0]))
                    return false;

                for (int i = 0; i < email.Length; i++)
                {
                    char c = email[i];

                    if (!(char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '_' || c == '-'))
                    {
                        return false;
                    }
                }

                int dotCount = 0;

                for (int i = 0; i < email.Length; i++)
                {
                    if (email[i] == '.')
                    {
                        dotCount++;
                    }
                }

                if (dotCount >= 3)
                    return false;


                return true;


            }



            public static bool IsValidPhone(string phone)
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return false;

                if (phone.Length != 10)
                    return false;

                for (int i = 0; i < phone.Length; i++)
                {
                    if (!char.IsDigit(phone[i]))
                        return false;
                }

                return true;
            }
        }

    }
}

