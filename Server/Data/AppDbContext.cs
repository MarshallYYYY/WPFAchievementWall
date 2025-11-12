using Microsoft.EntityFrameworkCore;
using Models;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Goal> Goals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API

            // 配置 User 实体
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName)
                      .IsRequired()         //必填，非空
                      .HasMaxLength(50);
                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(255);
                entity.Property(e => e.AvatarPath)
                      .IsRequired()
                      .HasMaxLength(500);
                entity.Property(e => e.CreateTime)
                      .HasDefaultValueSql("GETDATE()");  // 默认值为 SQL Server 的当前时间

                // 添加索引
                entity.HasIndex(e => e.UserName).IsUnique();
            });

            // 配置 Achievement 实体
            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Content)
                      .HasMaxLength(1000);
                entity.Property(e => e.ImagePath)
                      .HasMaxLength(500);
                entity.Property(e => e.Level)
                      .IsRequired()
                      .HasDefaultValue(3);
                entity.Property(e => e.Category)
                      .HasMaxLength(50);

                // 添加索引
                entity.HasIndex(e => e.AchieveDate);
                entity.HasIndex(e => e.Category);
            });

            // 配置 Goal 实体
            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Content)
                      .HasMaxLength(1000);

                // 添加索引
                entity.HasIndex(e => e.TargetDate);
            });
        }
    }
}