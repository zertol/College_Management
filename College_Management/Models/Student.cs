using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace College_Management.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public int RegNumber { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }


    }
}