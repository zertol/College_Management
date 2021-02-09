using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using College_Management.DAL;
using College_Management.Models;

namespace College_Management.Controllers
{
    public class EnrollmentsController : Controller
    {
        private CollegeContext db = new CollegeContext();

        // GET: Enrollments
        public async Task<ActionResult> Index()
        {
            return View(await db.Enrollments.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Enrollments/Create
        [HttpPost]
        public async Task<JsonResult> Create([Bind(Include = "Id,SubjectId,StudentId,Grade")] Enrollment enrollment)
        {
            var student = db.Students.Where(s => s.Id == enrollment.StudentId).FirstOrDefault();
            var subject = db.Subjects.Where(s => s.SubjectId == enrollment.SubjectId).FirstOrDefault();
            var course = db.Courses.Where(s => s.Subjects.Where(b => b.SubjectId == enrollment.SubjectId).Count() > 0).FirstOrDefault();

            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();
            return Json(new
            {
                Id = db.Enrollments.ToList().LastOrDefault<Enrollment>().Id,
                Student = student.Name,
                Subject = subject.SubjectId + "-" + subject.Title,
                Course = course.CourseId + "-" + course.Title,
                Grade = enrollment.Grade
            }); ;
        }

        // GET: Enrollments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Courses/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit([Bind(Include = "Id,Grade")] Enrollment enrollment)
        {
            try
            {
                Enrollment enrollmentEdit = await db.Enrollments.FindAsync(enrollment.Id);
                enrollmentEdit.Grade = enrollment.Grade;
                db.Entry(enrollmentEdit).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { result = "ok", status = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", status = 200, message = ex.InnerException.InnerException.Message.ToString() });
            }
        }

        // GET: Enrollments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteConfirmed(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Parameter id should not be null!");
                }
                Enrollment enrollment = await db.Enrollments.FindAsync(id);
                db.Enrollments.Remove(enrollment);
                await db.SaveChangesAsync();
                return Json(new { result = "ok", status = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", status = 200, message = ex.Message.ToString() });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public JsonResult GetData()
        {
            var enrollments = db.Enrollments.ToList();
            var subjects = db.Subjects.ToList();
            var students = db.Students.ToList();
            var courses = db.Courses.ToList();

            var result = (from enrollment in enrollments
                          select new
                          {
                              Id = enrollment.Id,
                              Subject = (from subject in subjects.Where(s => s.SubjectId == enrollment.SubjectId)
                                        select subject.SubjectId + "-" + subject.Title).First(),
                              Course = (from course in courses
                                        from subject in course.Subjects.Where(s => s.SubjectId == enrollment.SubjectId)
                                        select course.CourseId + "-" + course.Title).First(),
                              Student = (from student in students.Where(s => s.Id == enrollment.StudentId)
                                         select student.Name).First(),
                               Grade = enrollment.Grade
                          }

                ).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPrerequisites()
        {
            var students = db.Students.ToList().Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            });

            var subjects = db.Subjects.ToList().Select(x => new
            {
                SubjectId = x.SubjectId,
                Title = x.Title
            });

            return Json(new { Students = students, Subjects = subjects }, JsonRequestBehavior.AllowGet);
        }
    }
}
