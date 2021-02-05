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
    public class StudentsController : Controller
    {
        private CollegeContext db = new CollegeContext();

        // GET: Students
        public async Task<ActionResult> Index()
        {
            return View(await db.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        public async Task<JsonResult> Create([Bind(Include = "Id,Name,Birthday,RegNumber")] Student student)
        {
            db.Students.Add(student);
            await db.SaveChangesAsync();
            return Json(db.Students.ToList().LastOrDefault<Student>());

        }

        // GET: Students/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit([Bind(Include = "Id,Name,Birthday,RegNumber")] Student student)
        {
            try
            {
                db.Entry(student).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { result = "ok", status = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", status = 200, message = ex.Message.ToString() });
            }
               
            
        }

        // GET: Students/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                throw new Exception("Parameter id should not be null!");
            }
            try
            {
                Student student = await db.Students.FindAsync(id);
                db.Students.Remove(student);
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

            var result = (from student in students
                          select new
                          {
                              Id = student.Id,
                              Name = student.Name,
                              Birthday = student.Birthday,
                              RegNumber = student.RegNumber,
                              Subjects = from subject in subjects
                                         from s in subject.Enrollments.Where(x => x.StudentId == student.Id)
                                         select new { SubjectId = s.SubjectId, Title = subject.Title, Grade = s.Grade }
                          }

                ).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
