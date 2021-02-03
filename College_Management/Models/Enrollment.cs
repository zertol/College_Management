using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace College_Management.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public int StudentId { get; set; }
        
        public int? Grade { get; set; }

    }
}