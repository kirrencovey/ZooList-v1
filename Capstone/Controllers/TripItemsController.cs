using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Capstone.Controllers
{
    public class TripItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TripItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: TripItems
        [Authorize]
        public async Task<IActionResult> Index([FromRoute] int id)
        {
            // should display a list of zoos that are all associated with the same trip
            var applicationDbContext = _context.TripItems
                                        .Include(t => t.Trip)
                                        .Include(t => t.Zoo)
                                        .Where(t => t.TripId == id);
            ViewData["Trip"] = _context.Trips.FirstOrDefault(t => t.TripId == id);

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
        [Authorize]
        public async Task<IActionResult> Create([FromRoute]int id)
        {
            var user = await GetCurrentUserAsync();

            // check if user has existing trips. if not, direct to create trip form
            if (_context.Trips.Where(t => t.UserId == user.Id).Count() == 0)
            {
                return RedirectToAction("Create", "Trips");
            }

            ViewData["TripId"] = new SelectList(_context.Trips.Where(t => t.UserId == user.Id), "TripId", "Name");
            ViewData["Zoo"] = _context.Zoos.FirstOrDefault(z => z.ZooId == id);
            return View();
        }

        // POST: TripItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
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
                return RedirectToAction("Index", "Zoos");
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "Name", tripItem.TripId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", tripItem.ZooId);
            return View(tripItem);
        }

        // GET: TripItems/Delete/5
        [Authorize]
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
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tripItem = await _context.TripItems.FindAsync(id);
            _context.TripItems.Remove(tripItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Trips");
        }

        private bool TripItemExists(int id)
        {
            return _context.TripItems.Any(e => e.TripItemId == id);
        }
    }
}
