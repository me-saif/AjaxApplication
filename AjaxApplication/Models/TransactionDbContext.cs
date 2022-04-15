using Microsoft.EntityFrameworkCore;

namespace AjaxApplication.Models
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        { }

        public DbSet<TransactionModel> Transactions { get; set; }

        public DbSet<PicUploadModel> PicUploads { get; set; }
    }
}