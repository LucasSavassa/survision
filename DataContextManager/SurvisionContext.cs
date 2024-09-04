using DataContextManager.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DataContextManager
{
    public class SurvisionContext : DbContext
    {
        private const string connectionString = "";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DUNOTEBOOK\SOBRINHO;Database=survision;User Id=sa;Password=qwe12344r;TrustServerCertificate=True");
        }
        public DbSet<CaptureModel> CaptureModel { get; set; }
        public DbSet<ConfigurationModel> ConfigurationModel { get; set; }
        public DbSet<CriticalMomentModel> CriticalMomentModel { get; set; }
        public DbSet<DetectionModel> DetectionModel { get; set; }
        public DbSet<InstrumentModel> InstrumentModel { get; set; }
        public DbSet<SurgeryModel> SurgeryModel { get; set; }
    }
}
