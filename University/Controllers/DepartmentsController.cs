using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace University.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly University.Data.ApplicationDbContext _context;

        public DepartmentsController(University.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                  .Include(d => d.Administrator)
                  .ToListAsync();

            return View(departments);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Create()
        {
            var departments = _context.Departments.ToList();

            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FirstMidName");
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department model)
        {
            if (!this.ModelState.IsValid)
            {
                var instructors = _context.Instructors.ToList();

                this.ViewData["Instructors"] = instructors;

                return this.View(model);
            }

            var emptyDepartment = new Department()
            {
                Name = model.Name,
                Budget = model.Budget,
                StartDate = model.StartDate,
                InstructorID = model.InstructorID,

            };


            if (await TryUpdateModelAsync<Department>(
                 emptyDepartment,
                 "department",   // Prefix for form value.
                 s => s.InstructorID, s => s.Name, s => s.Budget, s => s.StartDate))
            {
                _context.Departments.Add(emptyDepartment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Departments");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var instructors = _context.Instructors.ToList();

            this.ViewData["Instructors"] = instructors;

            var department = await _context.Departments
                .Include(d => d.Administrator)  
                .AsNoTracking()                 
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            return View(department);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (!this.ModelState.IsValid)
            {
                var instructors = _context.Instructors.ToList();

                this.ViewData["Instructors"] = instructors;

                return this.View();
            }

            var departmentToUpdate = await _context.Departments
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (await TryUpdateModelAsync<Department>(
               departmentToUpdate,
               "Department",
               s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The department was deleted by another user.");
                        return View();
                    }
                }
            }

            return RedirectToAction("Index", "Departments");

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            return View(department);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department model)
        {
            if (!this.ModelState.IsValid)
            {

                return this.View(model);
            }

            var department = await _context.Departments.FindAsync(model.DepartmentID);

            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Departments");

        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Departments");
            }
            var department = await _context.Departments
                .Include(d => d.Administrator).FirstOrDefaultAsync(m => m.DepartmentID == id);

            return View(department);
        }


    }
}
