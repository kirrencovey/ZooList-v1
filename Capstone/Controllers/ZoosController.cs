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
    public class ZoosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZoosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Zoos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Zoos
                                .OrderBy(z => z.State)
                                .ThenBy(z => z.Name)
                                .ToListAsync());


            // maybe load facilities in a random order each time the page is loaded so users see more facilities? but that messes up the continuous scroll-and-add functionality. tbd

            //Random rnd = new Random();

           // return View(await _context.Zoos
           //                     .OrderBy<Zoo, int>((item) => rnd.Next())
           //                     .ToListAsync());
        }

        // GET: Zoos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos
                .FirstOrDefaultAsync(m => m.ZooId == id);
            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // GET: Zoos by user search
        public async Task<IActionResult> Search(string search)
        {
            List<Zoo> matchingZoos = await _context.Zoos
                                            .Where(z => z.Name.ToUpper().Contains(search.ToUpper()) || 
                                                        z.City.ToUpper().Contains(search.ToUpper()))
                                            .OrderBy(z => z.State)
                                            //.ThenBy(z => z.Name)
                                            .ToListAsync();
            return View(matchingZoos);
        }

        // GET: Zoos grouped by state

        public async Task<IActionResult> ZoosGroupedByState ()
        {
            List<GroupedZoos> model = new List<GroupedZoos>();

            // randomize order of zoos so that different zoos will populate below each state on reload
            Random rnd = new Random();

            model = await (
                from z in _context.Zoos
                group new { z } by new { z.State } into grouped
                select new GroupedZoos
                {
                    State = grouped.Key.State,
                    ZooCount = grouped.Select(x => x.z.State).Count(),
                    Zoos = grouped.Select(x => x.z).OrderBy<Zoo, int>((item) => rnd.Next()).Take(3)
                }).ToListAsync();

            return View(model);
        }

        // GET: Zoos/State All zoos from one state
        public async Task<IActionResult> ZoosInState(string state)
        {
            var data = RouteData.Values;
            List<Zoo> matchingZoos = await _context.Zoos
                                            .Where(z => z.State.ToUpper() == state.ToUpper())
                                            .OrderBy(z => z.City)
                                            .ToListAsync();
            return View(matchingZoos);
        }

        // GET: Zoos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Zoos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZooId,Name,Address,City,State,Zipcode,Country,WebsiteURL,ImagePath")] Zoo zoo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zoo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zoo);
        }

        // GET: Zoos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos.FindAsync(id);
            if (zoo == null)
            {
                return NotFound();
            }
            return View(zoo);
        }

        // POST: Zoos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZooId,Name,Address,City,State,Zipcode,Country,WebsiteURL,ImagePath")] Zoo zoo)
        {
            if (id != zoo.ZooId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zoo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZooExists(zoo.ZooId))
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
            return View(zoo);
        }

        // GET: Zoos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos
                .FirstOrDefaultAsync(m => m.ZooId == id);
            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // POST: Zoos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zoo = await _context.Zoos.FindAsync(id);
            _context.Zoos.Remove(zoo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZooExists(int id)
        {
            return _context.Zoos.Any(e => e.ZooId == id);
        }
    }
}
