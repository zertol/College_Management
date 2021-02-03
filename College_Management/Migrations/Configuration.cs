namespace College_Management.Migrations
{
    using College_Management.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<College_Management.DAL.CollegeContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(College_Management.DAL.CollegeContext context)
        {
            var students = new List<Student>
            {
            new Student{Name="Carson", Birthday=DateTime.Parse("1987-11-01"), RegNumber=1234},
            new Student{Name="Meredith", Birthday=DateTime.Parse("1987-10-01"), RegNumber=12356},
            new Student{Name="Arturo", Birthday=DateTime.Parse("1987-08-01"), RegNumber=123},
            new Student{Name="Gytis", Birthday=DateTime.Parse("1987-12-01"), RegNumber=642},
            new Student{Name="Yan", Birthday=DateTime.Parse("1987-05-01"), RegNumber=87954},
            new Student{Name="Peggy", Birthday=DateTime.Parse("1987-06-01"), RegNumber=2355},
            new Student{Name="Laura", Birthday=DateTime.Parse("1988-06-01"), RegNumber=655},
            new Student{Name="Nino", Birthday=DateTime.Parse("1988-08-01"), RegNumber=3252},
            };

            students.ForEach(s => context.Students.Add(s));
            context.SaveChanges();

            var courses = new List<Course>
            {
            new Course{CourseId=1050,Title="Chemistry"},
            new Course{CourseId=4022,Title="Microeconomics"},
            new Course{CourseId=4041,Title="Macroeconomics"},
            new Course{CourseId=1045,Title="Calculus"},
            new Course{CourseId=3141,Title="Trigonometry"},
            new Course{CourseId=2021,Title="Composition"},
            new Course{CourseId=2042,Title="Literature"}
            };
            courses.ForEach(s => context.Courses.Add(s));
            context.SaveChanges();

            var subjects = new List<Subject>
            {
                new Subject{SubjectId=1001, CourseId=1050,Title="Chemisty In Depth"},
                new Subject{SubjectId=1002,CourseId=1050,Title="Chemisty For Fun"},
                new Subject{SubjectId=1003, CourseId=4022,Title="EconomicsA"},
                new Subject{SubjectId=1004,CourseId=4041,Title="MacroEcA"},
                new Subject{SubjectId=1005,CourseId=1045,Title="Advanced Statistics"},
                new Subject{SubjectId=1006,CourseId=3141,Title="Basics References"},
                new Subject{SubjectId=1007,CourseId=2021,Title="Composing 101"},
                new Subject{SubjectId=1008,CourseId=2042,Title="Modern French Writing"},
            };
            subjects.ForEach(s => context.Subjects.Add(s));
            context.SaveChanges();

            var teachers = new List<Teacher>
            {
                new Teacher{ SubjectId=1001, Name="Joe", Birthday=DateTime.Parse("1968-11-01"), Salary=1200 },
                new Teacher{ SubjectId=1002, Name="Lisa", Birthday=DateTime.Parse("1968-10-01"), Salary=900 },
                new Teacher{ SubjectId=1003, Name="Brad", Birthday=DateTime.Parse("1968-09-01"), Salary=1120 },
                new Teacher{ SubjectId=1004, Name="Michael", Birthday=DateTime.Parse("1968-08-01"), Salary=870 },
                new Teacher{ SubjectId=1005, Name="Alison", Birthday=DateTime.Parse("1968-07-01"), Salary=1300 },
                new Teacher{ SubjectId=1006, Name="Victoria", Birthday=DateTime.Parse("1968-06-01"), Salary=1000 },
                new Teacher{ SubjectId=1007, Name="Nate", Birthday=DateTime.Parse("1968-05-01"), Salary=1350 },
                new Teacher{ SubjectId=1008, Name="Johnny", Birthday=DateTime.Parse("1968-04-01"), Salary=1050 },
            };
            teachers.ForEach(s => context.Teachers.Add(s));
            context.SaveChanges();

            var enrollments = new List<Enrollment>
            {
            new Enrollment{StudentId=1,SubjectId=1001,Grade=90},
            new Enrollment{StudentId=1,SubjectId=1002,Grade=82},
            new Enrollment{StudentId=1,SubjectId=1003,Grade=63},
            new Enrollment{StudentId=2,SubjectId=1005,Grade=64},
            new Enrollment{StudentId=2,SubjectId=1006,Grade=40},
            new Enrollment{StudentId=2,SubjectId=1007,Grade=46},
            new Enrollment{StudentId=3,SubjectId=1004},
            new Enrollment{StudentId=4,SubjectId=1004,},
            new Enrollment{StudentId=4,SubjectId=1008,Grade=34},
            new Enrollment{StudentId=5,SubjectId=1001,Grade=65},
            new Enrollment{StudentId=6,SubjectId=1002},
            new Enrollment{StudentId=7,SubjectId=1003,Grade=87},
            };
            enrollments.ForEach(s => context.Enrollments.Add(s));
            context.SaveChanges();
        }
    }
}
