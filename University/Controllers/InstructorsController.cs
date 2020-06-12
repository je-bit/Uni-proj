using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpGet]
        ////public async Task<IActionResult> Index()
        ////{

        //    return View(instructors);
        //}


    }
}
