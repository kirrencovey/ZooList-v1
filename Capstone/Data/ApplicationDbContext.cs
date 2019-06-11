using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Capstone.Models;

namespace Capstone.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripItem> TripItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Zoo> Zoos { get; set; }
        public DbSet<Visit> Visits { get; set; }
    }
}
