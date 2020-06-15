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
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly University.Data.ApplicationDbContext _context;

        public StudentsController(University.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string searchToken)
        {
            Student[] students;
            var queryStudents = _context.Students;

            if (String.IsNullOrWhiteSpace(searchToken) == false)
            {
                this.ViewData["searchToken"] = searchToken;
                searchToken = searchToken.ToUpper();

                students = queryStudents.Where(s => s.LastName.ToUpper().Contains(searchToken)).ToArray();
            }
            else
            {
                students = queryStudents.ToArray();
            }

            return View(students);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            _context.Students.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index", "Students");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var student = await _context.Students.FindAsync(id);

            return View(student);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var studentToUpdate = await _context.Students.FindAsync(model.ID);

            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "student",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {

                studentToUpdate.FirstMidName = model.FirstMidName;
                studentToUpdate.LastName = model.LastName;
                studentToUpdate.EnrollmentDate = model.EnrollmentDate;

                _context.Students.Update(studentToUpdate);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Students");

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            return View(student);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Student model)
        {
            var student = await _context.Students.FindAsync(model.ID);

            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Students");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Students");
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            return View(student);
        }

    }
}
