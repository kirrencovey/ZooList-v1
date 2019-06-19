﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Capstone.Controllers
{
    public class VisitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VisitsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Visits
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // User should only see their own visit history
            var user = await GetCurrentUserAsync();

            var applicationDbContext = _context.Visits
                                        .Include(v => v.User)
                                        .Include(v => v.Zoo)
                                        .Where(v => v.UserId == user.Id)
                                        .OrderByDescending(v => v.Date);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Visits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits
                .Include(v => v.User)
                .Include(v => v.Zoo)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        // GET: Visits/Create
        [Authorize]
        public IActionResult Create(int id)
        {
            ViewBag.Zoo = _context.Zoos.FirstOrDefault(z => z.ZooId == id);
            return View();
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("VisitId,Date,Comments,UserId,ZooId,Zoo")] Visit visit)
        {
            var user = await GetCurrentUserAsync();

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                visit.ZooId = id;
                visit.Zoo = _context.Zoos.FirstOrDefault(x => x.ZooId == id);
                visit.User = user;
                visit.UserId = user.Id;
                _context.Add(visit);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", visit.UserId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", visit.ZooId);
            return View(visit);
        }

        // GET: Visits/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits.FindAsync(id);
            _context.Update(visit);

            ViewBag.Zoo = _context.Zoos.FirstOrDefault(z => z.ZooId == visit.ZooId);

            if (visit == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", visit.UserId);
            ViewData["ZooId"] = new SelectList(_context.Zoos.Where(z => z.ZooId == visit.ZooId), "ZooId", "Name", visit.ZooId);
            return View(visit);
        }

        // POST: Visits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisitId,Date,Comments,UserId,ZooId")] Visit visit)
        {
            if (id != visit.VisitId)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitExists(visit.VisitId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", visit.UserId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", visit.ZooId);
            return View(visit);
        }

        // GET: Visits/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits
                .Include(v => v.User)
                .Include(v => v.Zoo)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visit = await _context.Visits.FindAsync(id);
            _context.Visits.Remove(visit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitExists(int id)
        {
            return _context.Visits.Any(e => e.VisitId == id);
        }
    }
}
