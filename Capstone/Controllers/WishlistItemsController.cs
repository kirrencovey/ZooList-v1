using System;
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
    public class WishlistItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: WishlistItems
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            // Users should only see their own wishlist items
            var applicationDbContext = _context.WishlistItems
                                        .Include(w => w.User)
                                        .Include(w => w.Zoo)
                                        .Where(w => w.UserId == user.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WishlistItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.User)
                .Include(w => w.Zoo)
                .FirstOrDefaultAsync(m => m.WishlistItemId == id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            return View(wishlistItem);
        }

        // POST: WishlistItems/Create
        // Adds a new item to a user's wishlist
        public async Task<IActionResult> Create(int? id)
        {
            var user = await GetCurrentUserAsync();

            WishlistItem wishlistItem = new WishlistItem
            {
                Zoo = _context.Zoos.FirstOrDefault(x => x.ZooId == id),
                ZooId = _context.Zoos.FirstOrDefault(x => x.ZooId == id).ZooId,
                User = user,
                UserId = user.Id
            };
            _context.WishlistItems.Add(wishlistItem);
            _context.SaveChanges();

            return RedirectToAction("Index", "Zoos");
        }

        // GET: WishlistItems/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            if (wishlistItem == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", wishlistItem.UserId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", wishlistItem.ZooId);
            return View(wishlistItem);
        }

        // POST: WishlistItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WishlistItemId,UserId,ZooId")] WishlistItem wishlistItem)
        {
            if (id != wishlistItem.WishlistItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishlistItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishlistItemExists(wishlistItem.WishlistItemId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", wishlistItem.UserId);
            ViewData["ZooId"] = new SelectList(_context.Zoos, "ZooId", "Name", wishlistItem.ZooId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.User)
                .Include(w => w.Zoo)
                .FirstOrDefaultAsync(m => m.WishlistItemId == id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            return View(wishlistItem);
        }

        // POST: WishlistItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishlistItemExists(int id)
        {
            return _context.WishlistItems.Any(e => e.WishlistItemId == id);
        }
    }
}
