using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly University.Data.ApplicationDbContext _context;

        public IndexModel(University.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Department> Department { get;set; }

        public async Task OnGetAsync()
        {
            Department = await _context.Departments
                .Include(d => d.Administrator).ToListAsync();
        }
    }
}
