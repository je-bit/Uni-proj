using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace University.Models.SchoolViewModels
{
    public class CreateInstructor
    {
        public Instructor Instructor { get; set; }

        [Required]
        public string Location { get; set; }

        public string[] SelectedCourses { get; set; }
    }
}
