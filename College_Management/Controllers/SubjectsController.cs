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
            var subjects = db.Subjects.Include(s => s.TeacherSubject);
            return View(await subjects.ToListAsync());
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SubjectId,CourseId,Title")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Subjects.Add(subject);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SubjectId = new SelectList(db.Teachers, "SubjectId", "Name", subject.SubjectId);
            return View(subject);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SubjectId,CourseId,Title")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subject).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SubjectId = new SelectList(db.Teachers, "SubjectId", "Name", subject.SubjectId);
            return View(subject);
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

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Subject subject = await db.Subjects.FindAsync(id);
            db.Subjects.Remove(subject);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

            var result = (from subject in subjects
                          select new
                          {
                              SubjectId = subject.SubjectId,
                              Title = subject.Title,
                              Teacher = subject.TeacherSubject.Name,
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
                SubjectId = x.SubjectId,
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
