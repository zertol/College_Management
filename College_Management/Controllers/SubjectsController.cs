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
    public class SubjectsController : Controller
    {
        private CollegeContext db = new CollegeContext();

        // GET: Subjects
        public async Task<ActionResult> Index()
        {
            return View(await db.Subjects.ToListAsync());
        }

        // GET: Subjects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = await db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(db.Teachers, "SubjectId", "Name");
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        public async Task<JsonResult> Create([Bind(Include = "SubjectId,TeacherId,CourseId,Title")] Subject subject)
        {
            db.Subjects.Add(subject);
            await db.SaveChangesAsync();
            subject = await db.Subjects.FindAsync(subject.SubjectId);
            return Json(subject);
        }

        // GET: Subjects/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = await db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubjectId = new SelectList(db.Teachers, "SubjectId", "Name", subject.SubjectId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit([Bind(Include = "SubjectId,TeacherId,CourseId,Title")] Subject subject)
        {
            try
            {
                db.Entry(subject).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { result = "ok", status = 200});
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", status = 200, message = ex.InnerException.Message.ToString() });
            }
        }

        // GET: Subjects/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = await db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteConfirmed(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Parameter id should not be null!");
                }
                Subject subject = await db.Subjects.FindAsync(id);
                db.Subjects.Remove(subject);
                await db.SaveChangesAsync();
                return Json(new { result = "ok", status = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", status = 200, message = ex.Message.ToString() });
            }
        }

        [HttpGet]
        public JsonResult GetData()
        {
            var students = db.Students.ToList();
            var subjects = db.Subjects.ToList();
            var teachers = db.Teachers.ToList();

            var result = (from subject in subjects
                          select new
                          {
                              SubjectId = subject.SubjectId,
                              Title = subject.Title,
                              CourseId = subject.CourseId,
                              Teacher = subject.TeacherId != null 
                              ? teachers.Where(t => t.Id == subject.TeacherId).Select(t => new { Id = t.Id, Name = t.Name}).FirstOrDefault() 
                              : new { Id= -1, Name="N/A" },
                              Students = from student in students
                                         from s in student.Enrollments.Where(x => x.SubjectId == subject.SubjectId)
                                         select new { Name = student.Name, Grade = s.Grade }
                          }

                ).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTeachersCourses()
        {
            var teachers = db.Teachers.ToList().Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            });

            var courses = db.Courses.ToList().Select(x=> new
            {
                CourseId = x.CourseId,
                Title = x.Title
            });

            return Json(new { Courses = courses, Teachers = teachers }, JsonRequestBehavior.AllowGet);
        }

    }
}
