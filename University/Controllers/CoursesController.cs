using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace University.Controllers
{
    public class CoursesController : Controller
    {
        private readonly University.Data.ApplicationDbContext _context;

        public CoursesController(University.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> Index()
        {
          var courses = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .ToListAsync();

            return View(courses);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Create()
        {
            var departments = _context.Departments.ToList();

            this.ViewData["Departments"] = departments;
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course model)
        {
            if (!this.ModelState.IsValid)
            {
                var departments = _context.Departments.ToList();

                this.ViewData["Departments"] = departments;

                return this.View(model);
            }

            var emptyCourse = new Course()
            {
                Title = model.Title,
                DepartmentID = model.DepartmentID,
                Credits = model.Credits,
                CourseID = model.CourseID
            };


            if (await TryUpdateModelAsync<Course>(
                 emptyCourse,
                 "course",   // Prefix for form value.
                 s => s.CourseID, s => s.DepartmentID, s => s.Title, s => s.Credits))
            {
                _context.Courses.Add(emptyCourse);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Courses");
        }

        [Authorize(Roles ="Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var departments = _context.Departments.ToList();

            this.ViewData["Departments"] = departments;

            var course = await _context.Courses
                .Include(c => c.Department).FirstOrDefaultAsync(m => m.CourseID == id);

            return View(course);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course model)
        {
            if (!this.ModelState.IsValid)
            {
                var departments = _context.Departments.ToList();

                this.ViewData["Departments"] = departments;

                return this.View(model);
            }

            var courseToUpdate = await _context.Courses.FindAsync(model.CourseID);

            if (await TryUpdateModelAsync<Course>(
                 courseToUpdate,
                 "course",   // Prefix for form value.
                   c => c.Credits, c => c.DepartmentID, c => c.Title))
            {
                courseToUpdate.Credits = model.Credits;
                courseToUpdate.DepartmentID = model.DepartmentID;
                courseToUpdate.Title = model.Title;
                _context.Courses.Update(courseToUpdate);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Courses");

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.CourseID == id);

            return View(course);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Course model)
        {
            if (!this.ModelState.IsValid)
            {

                return this.View(model);
            }

            var course = await _context.Courses.FindAsync(model.CourseID);

            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Courses");

        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Courses");
            }
            var course = await _context.Courses
                .Include(c => c.Department).FirstOrDefaultAsync(m => m.CourseID == id);

            return View(course);
        }
    }
}
