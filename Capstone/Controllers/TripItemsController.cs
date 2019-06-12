using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;
using Capstone.Models;

namespace Capstone.Controllers
{
    public class TripItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TripItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TripItems.Include(t => t.Trip).Include(t => t.Zoo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TripItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripItem = await _context.TripItems
                .Include(t => t.Trip)
                .Include(t => t.Zoo)
                .FirstOrDefaultAsync(m => m.TripItemId == id);
            if (tripItem == null)
            {
                return NotFound();
            }

            return View(tripItem);
        }

        // GET: TripItems/Create
        public IActionResult Create([FromRoute]int id)
        {
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "Name");
            ViewData["Zoo"] = _context.Zoos.FirstOrDefault(z => z.ZooId == id);
            return View();
        }

        // POST: TripItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("TripItemId,ZooId,TripId")] TripItem tripItem)
        {
            if (ModelState.IsValid)
            {
                tripItem.Zoo = _context.Zoos.FirstOrDefault(z => z.ZooId == id);
                tripItem.ZooId = _context.Zoos.FirstOrDefault(z => z.ZooId == id).ZooId;
                _context.Add(tripItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Trips");
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "Name", tripItem.TripId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", tripItem.ZooId);
            return View(tripItem);
        }

        // GET: TripItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripItem = await _context.TripItems.FindAsync(id);
            if (tripItem == null)
            {
                return NotFound();
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "Name", tripItem.TripId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", tripItem.ZooId);
            return View(tripItem);
        }

        // POST: TripItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TripItemId,ZooId,TripId")] TripItem tripItem)
        {
            if (id != tripItem.TripItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tripItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripItemExists(tripItem.TripItemId))
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
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "Name", tripItem.TripId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", tripItem.ZooId);
            return View(tripItem);
        }

        // GET: TripItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripItem = await _context.TripItems
                .Include(t => t.Trip)
                .Include(t => t.Zoo)
                .FirstOrDefaultAsync(m => m.TripItemId == id);
            if (tripItem == null)
            {
                return NotFound();
            }

            return View(tripItem);
        }

        // POST: TripItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tripItem = await _context.TripItems.FindAsync(id);
            _context.TripItems.Remove(tripItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripItemExists(int id)
        {
            return _context.TripItems.Any(e => e.TripItemId == id);
        }
    }
}
