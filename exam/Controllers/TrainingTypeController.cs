using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using exam.Data;
using exam.Models;
using Microsoft.AspNetCore.Authorization;

namespace exam.Controllers
{
    [Authorize(Roles = "manager")]
    public class TrainingTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainingType
        public async Task<IActionResult> Index()
        {
            return View(await _context.TTypes.ToListAsync());
        }

        // GET: TrainingType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tType = await _context.TTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tType == null)
            {
                return NotFound();
            }

            return View(tType);
        }

        // GET: TrainingType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] TType tType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tType);
        }

        // GET: TrainingType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tType = await _context.TTypes.FindAsync(id);
            if (tType == null)
            {
                return NotFound();
            }
            return View(tType);
        }

        // POST: TrainingType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TType tType)
        {
            if (id != tType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTypeExists(tType.Id))
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
            return View(tType);
        }

        // GET: TrainingType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tType = await _context.TTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tType == null)
            {
                return NotFound();
            }

            return View(tType);
        }

        // POST: TrainingType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tType = await _context.TTypes.FindAsync(id);
            _context.TTypes.Remove(tType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTypeExists(int id)
        {
            return _context.TTypes.Any(e => e.Id == id);
        }
    }
}
