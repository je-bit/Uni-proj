using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;
using University.Models.SchoolViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace University.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly University.Data.ApplicationDbContext _context;

        public InstructorsController(University.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            if (id != null)
            {
                var instructor = _context.Instructors
                .Where(i => i.ID == id)
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Department)
                 .FirstOrDefault();

                this.ViewData["instructor"] = instructor;
            }

            if (courseID != null)
            {
                var courseEnrollments = _context.Enrollments
                    .Where(e => e.CourseID == courseID)
                    .Include(e => e.Student)
                    .ToArray();

                this.ViewData["courseEnrollments"] = courseEnrollments;
            }

            var instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Department)
                .OrderBy(i => i.LastName)
                .ToListAsync();

            return View(instructors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var courses = _context.Courses.ToArray();

            this.ViewData["courses"] = courses;

            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateInstructor model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var newInstructor = new Instructor()
            {
                FirstMidName = model.Instructor.FirstMidName,
                LastName = model.Instructor.LastName,
            };

            _context.Instructors.Add(newInstructor);
            _context.SaveChanges();

            var officeAssignment = new OfficeAssignment() 
            { 
                Location = model.Location, 
                InstructorID = newInstructor.ID
            };

            _context.OfficeAssignments.Add(officeAssignment);

            var listOfCourseAssignments = new List<CourseAssignment>();

            foreach (var courseId in model.SelectedCourses)
            {
                var newCourseAssignment = new CourseAssignment()
                {
                    CourseID = int.Parse(courseId),
                    InstructorID = newInstructor.ID
                };

                listOfCourseAssignments.Add(newCourseAssignment);
            }

            _context.AddRange(listOfCourseAssignments);

            _context.SaveChanges();

            return RedirectToAction("Index", "Instructors");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var courses = _context.Courses.ToArray();
            this.ViewData["courses"] = courses;

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            var newCreateInstructor = new CreateInstructor()
            {
                Instructor = instructor,
                Location = instructor.OfficeAssignment.Location,
                SelectedCourses = instructor.CourseAssignments.Select(c => c.CourseID.ToString()).ToArray()
            };

            return View(newCreateInstructor);
        }

        [HttpPost]
        public IActionResult Edit(CreateInstructor model)
        {
            var instructor = _context.Instructors.FirstOrDefault(i => i.ID == model.Instructor.ID);

            instructor.LastName = model.Instructor.LastName;
            instructor.HireDate = model.Instructor.HireDate;
            instructor.FirstMidName = model.Instructor.FirstMidName;

            _context.Instructors.Update(instructor);

            var officeAssignment = _context.OfficeAssignments.FirstOrDefault(oa => oa.InstructorID == model.Instructor.ID);

            officeAssignment.Location = model.Location;

            _context.OfficeAssignments.Update(officeAssignment);

            var instructorCourseAssignments = _context.CourseAssignments.Where(ca => ca.InstructorID == model.Instructor.ID).ToList();

            var listOfNewCourseAssignments = new List<CourseAssignment>();

            foreach (var course in instructorCourseAssignments)
            {
                if (model.SelectedCourses.Contains(course.CourseID.ToString()) == false)
                {
                    listOfNewCourseAssignments.Add(course);
                }
            }

            _context.CourseAssignments.RemoveRange(instructorCourseAssignments);
            _context.SaveChanges();

            foreach (var course in listOfNewCourseAssignments)
            {
                instructorCourseAssignments.Remove(course);
            }

            var instructorCourseAssignmentsCourseIds = instructorCourseAssignments.Select(ca => ca.CourseID).ToList();

            foreach (var courseId in model.SelectedCourses)
            {
                var newCourseAssignment = new CourseAssignment()
                {
                    CourseID = int.Parse(courseId),
                    InstructorID = model.Instructor.ID,
                    Instructor = instructor
                };

                if (instructorCourseAssignmentsCourseIds.Contains(newCourseAssignment.CourseID) == false)
                {
                    instructorCourseAssignments.Add(newCourseAssignment);
                }
            }

            _context.CourseAssignments.AddRange(instructorCourseAssignments);

            _context.SaveChanges();


            return RedirectToAction("Index", "Instructors");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var instructors = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            return View(instructors);
        }

        [HttpPost]
        public IActionResult Delete(Instructor model)
        {
            var instructor = _context.Instructors.FirstOrDefault(i => i.ID == model.ID);

            _context.Instructors.Remove(instructor);
            _context.SaveChanges();

           return RedirectToAction("Index", "Instructors");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Instructors");
            }

            var instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.ID == id);

            return View(instructor);
        }
    }
}
