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
                                        .Where(w => w.UserId == user.Id)
                                        .OrderBy(w => w.Zoo.State);
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(int id)
        {
            var user = await GetCurrentUserAsync();

            // Check if user has already added this zoo to their wishlist
            if (_context.WishlistItems.FirstOrDefault(x => x.ZooId == id && x.UserId == user.Id) != null)
            {
                // redirect user to the same place they were on the page for better user experience
                return Redirect($"{Url.RouteUrl(new { controller = "Zoos", action = "Index" })}#{id}");
            }
            else
            {
                WishlistItem wishlistItem = new WishlistItem
                {
                    Zoo = _context.Zoos.FirstOrDefault(x => x.ZooId == id),
                    ZooId = id,
                    User = user,
                    UserId = user.Id
                };
                _context.WishlistItems.Add(wishlistItem);
                _context.SaveChanges();

                return Redirect($"{Url.RouteUrl(new { controller = "Zoos", action = "Index" })}#{id}");
            }

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
        [Authorize]
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
