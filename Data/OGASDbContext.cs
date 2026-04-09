using Microsoft.EntityFrameworkCore;
using OGAS.Models;

namespace OGAS.Data
{
    public class OGASDbContext : DbContext
    {
        public OGASDbContext(DbContextOptions<OGASDbContext> options)
            : base(options)
        {
        }

        // DbSet 属性
        public DbSet<用户> 用户 { get; set; }
        public DbSet<产品> 产品s { get; set; }
        public DbSet<材料>材料s { get; set; }
        public DbSet<加工厂> 加工厂s { get; set; }
        public DbSet<产品库存> 产品库存s { get; set; }
        public DbSet<材料库存> 材料库存s { get; set; }
        public DbSet<生产工艺> 生产工艺s { get; set; }
        public DbSet<生产计划> 生产计划s { get; set; }
        public DbSet<生产记录> 生产记录s { get; set; }
        public DbSet<出口订单> 出口订单s { get; set; }
        public DbSet<生产设备> 生产设备s { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // 产品配置
            modelBuilder.Entity<产品>()
                .HasKey(p => p.产品ID);

            modelBuilder.Entity<产品>()
                .HasMany(p => p.生产工艺s)
                .WithOne(p => p.产品)
                .HasForeignKey(p => p.产品ID);

            modelBuilder.Entity<产品>()
                .HasMany(p => p.出口订单s)
                .WithOne(p => p.产品)
                .HasForeignKey(p => p.产品ID);

            modelBuilder.Entity<产品>()
                .HasMany(p => p.产品库存s)
                .WithOne(p => p.产品)
                .HasForeignKey(p => p.产品ID);

            // 出口订单配置
            modelBuilder.Entity<出口订单>()
                .HasKey(p => p.订单ID);

            modelBuilder.Entity<出口订单>()
                .HasOne(p => p.产品)  // 指定与产品的关系
                .WithMany(e => e.出口订单s)  // 如果出口订单是与产品的多对一关系
                .HasForeignKey(p => p.产品ID)  // 指定外键列
                .OnDelete(DeleteBehavior.Cascade);

            // 加工厂配置
            modelBuilder.Entity<加工厂>()
                .HasKey(p => p.加工厂ID);

            modelBuilder.Entity<加工厂>()
                .HasMany(p => p.生产计划s)
                .WithOne(p => p.加工厂)
                .HasForeignKey(p => p.加工厂ID);

            modelBuilder.Entity<加工厂>()
                .HasMany(p => p.生产记录s)
                .WithOne(p => p.加工厂)
                .HasForeignKey(p => p.加工厂ID);

            // 产品库存配置
            modelBuilder.Entity<产品库存>()
                .HasKey(p => p.产品库存ID);

            modelBuilder.Entity<产品库存>()
                .HasOne(p => p.产品)
                .WithMany(p => p.产品库存s)
                .HasForeignKey(p => p.产品ID);

            // 材料库存配置
            modelBuilder.Entity<材料库存>()
                .HasKey(p => p.材料库存ID);

            modelBuilder.Entity<材料库存>()
                .HasOne(p => p.材料)
                .WithMany(p => p.材料库存s)
                .HasForeignKey(p => p.材料ID);

            // 生产工艺配置
            modelBuilder.Entity<生产工艺>()
                .HasKey(p => p.工艺ID);

            modelBuilder.Entity<生产工艺>()
                .HasOne(p => p.产品)
                .WithMany(p => p.生产工艺s)
                .HasForeignKey(p => p.产品ID);

            modelBuilder.Entity<生产工艺>()
                .HasOne(p => p.生产设备)
                .WithMany()
                .HasForeignKey(p => p.生产设备ID);

            modelBuilder.Entity<生产工艺>()
                .HasMany(p => p.生产计划s)
                .WithOne(p => p.生产工艺)
                .HasForeignKey(p => p.工艺ID);

            modelBuilder.Entity<生产工艺>()
                .HasMany(p => p.生产记录s)
                .WithOne(p => p.生产工艺)
                .HasForeignKey(p => p.工艺ID);

            // 生产计划配置
            modelBuilder.Entity<生产计划>()
                .HasKey(p => p.计划ID);

            modelBuilder.Entity<生产计划>()
                .HasOne(p => p.加工厂)
                .WithMany(p => p.生产计划s)
                .HasForeignKey(p => p.加工厂ID);

            modelBuilder.Entity<生产计划>()
                .HasOne(p => p.生产工艺)
                .WithMany(p => p.生产计划s)
                .HasForeignKey(p => p.工艺ID);

            modelBuilder.Entity<生产计划>()
                .HasMany(p => p.生产记录s)
                .WithOne(p => p.生产计划)
                .HasForeignKey(p => p.计划ID);

            // 生产记录配置
            modelBuilder.Entity<生产记录>()
                .HasKey(p => p.生产记录ID);

            modelBuilder.Entity<生产记录>()
                .HasOne(p => p.生产工艺)
                .WithMany(p => p.生产记录s)
                .HasForeignKey(p => p.工艺ID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<生产记录>()
                .HasOne(p => p.加工厂)
                .WithMany(p => p.生产记录s)
                .HasForeignKey(p => p.加工厂ID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<生产记录>()
                .HasOne(p => p.生产计划)
                .WithMany(p => p.生产记录s)
                .HasForeignKey(p => p.计划ID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<生产记录>()
                .HasOne(p => p.生产设备)
                .WithMany()
                .HasForeignKey(p => p.生产设备ID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // 生产设备配置
            modelBuilder.Entity<生产设备>()
                .HasKey(p => p.生产设备ID);

            modelBuilder.Entity<生产设备>()
                .HasMany(p => p.加工厂s)
                .WithOne(p => p.生产设备)
                .HasForeignKey(p => p.生产设备ID);

            modelBuilder.Entity<生产设备>()
                .HasMany(p => p.生产工艺s)
                .WithOne(p => p.生产设备)
                .HasForeignKey(p => p.生产设备ID);

            modelBuilder.Entity<生产设备>()
                .HasMany(p => p.生产记录s)
                .WithOne(p => p.生产设备)
                .HasForeignKey(p => p.生产设备ID);
        }
    }
}
