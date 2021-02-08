using College_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace College_Management.DAL
{
    public class CollegeContext: DbContext
    {
        public CollegeContext() : base("CollegeContext")
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Entity<Subject>()
            //    .HasMany(s => s.chi)
            //    .WithRequired( XmlSiteMapProvider=)
            //modelBuilder.Entity<Subject>()
            //            .HasOptional(a => a.TeacherSubject)
            //            .WithOptionalDependent()
            //            .WillCascadeOnDelete(true);
        }

        
    }
}