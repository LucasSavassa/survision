using Microsoft.EntityFrameworkCore;
using Comuns.Classes;

namespace WebAPI.Context
{
    public class SurgeryContext : DbContext
    {
        public SurgeryContext(DbContextOptions<SurgeryContext> options) : base(options) { }

        public DbSet<Surgery> Surgeries { get; set; }
    }
}
