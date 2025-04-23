using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Training.Data;
using Training.Models;

namespace Training.Controllers
{
    public class CourseModelsController : Controller
    {
        private readonly TrainingDbContext _context;

        public CourseModelsController(TrainingDbContext context)
        {
            _context = context;
        }

        // GET: CourseModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: CourseModels/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseModel = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (courseModel == null)
            {
                return NotFound();
            }

            return View(courseModel);
        }

        // GET: CourseModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseName,StartDate,EndDate,TuitionFee,MaxStudents,CourseImagePath,Description,Lecture")] CourseModel courseModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(courseModel);
        }

        // GET: CourseModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseModel = await _context.Courses.FindAsync(id);
            if (courseModel == null)
            {
                return NotFound();
            }
            return View(courseModel);
        }

        // POST: CourseModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CourseId,CourseName,StartDate,EndDate,TuitionFee,MaxStudents,CourseImagePath,Description,Lecture")] CourseModel courseModel)
        {
            if (id != courseModel.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseModelExists(courseModel.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(courseModel);
        }

        // GET: CourseModels/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseModel = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (courseModel == null)
            {
                return NotFound();
            }

            return View(courseModel);
        }

        // POST: CourseModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var courseModel = await _context.Courses.FindAsync(id);
            if (courseModel != null)
            {
                _context.Courses.Remove(courseModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseModelExists(string id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
