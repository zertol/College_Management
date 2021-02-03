using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace College_Management.Models
{
    public class Subject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubjectId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; }

        public virtual Teacher TeacherSubject { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
}