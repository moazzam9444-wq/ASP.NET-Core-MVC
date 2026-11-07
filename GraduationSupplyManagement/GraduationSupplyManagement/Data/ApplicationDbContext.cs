using GraduationSupplyManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace GraduationSupplyManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SelectedOffer> SelectedOffers { get; set; }
    }
}