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
    public class CoursesController : Controller
    {
        private CollegeContext db = new CollegeContext();

        // GET: Courses
        public async Task<ActionResult> Index()
        {
            return View(await db.Courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create([Bind(Include = "CourseId,Title")] Course course)
        {
            db.Courses.Add(course);
            await db.SaveChangesAsync();
            course = await db.Courses.FindAsync(course.CourseId);
            return Json(course);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit([Bind(Include = "CourseId,Title")] Course course)
        {
            try
            {
                db.Entry(course).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { result = "ok", status = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", status = 200, message = ex.InnerException.InnerException.Message.ToString() });
            }
        }

        // GET: Courses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteConfirmed(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Parameter id should not be null!");
                }
                Course course = await db.Courses.FindAsync(id);
                db.Courses.Remove(course);
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
            var students = db.Students.ToList();
            var subjects = db.Subjects.ToList();
            var teachers = db.Teachers.ToList();
            var courses = db.Courses.ToList();

            var result = (from course in courses
                          select new
                          {
                              CourseId = course.CourseId,
                              Title = course.Title,
                              NbTeachers = (from teacher in teachers
                                         from subject in teacher.Subjects.Where(s => s.CourseId == course.CourseId)
                                         select new {Id = teacher.Id, Name= teacher.Name}).Count(),
                              Students = (from student in students
                                         from s in student.Enrollments.Where(x => course.Subjects.Contains(subjects.Where(sb => sb.SubjectId == x.SubjectId).FirstOrDefault()))
                                         group s by s.Id into g
                                         select new { Total = g.Count(), AverageGrade = g.Average(x => x.Grade) }).FirstOrDefault()
                          }

                ).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
