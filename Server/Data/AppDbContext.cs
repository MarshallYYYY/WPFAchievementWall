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
                // 主键
                entity.HasKey(e => e.Id);
                // 非空，唯一
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
                      .IsRequired()
                      // 默认值为 SQL Server 的当前时间
                      .HasDefaultValueSql("GETDATE()");

                // 添加唯一索引
                entity.HasIndex(e => e.UserName).IsUnique();
            });

            // 配置 Achievement 实体
            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Title: 非空
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                // Content: 非空
                entity.Property(e => e.Content)
                      .IsRequired()
                      .HasMaxLength(1000);

                // AchieveDate: 非空，Goal 需要特殊处理
                entity.Property(e => e.AchieveDate)
                      .IsRequired();

                entity.Property(e => e.Level)
                      .IsRequired();
                entity.Property(e => e.ImagePath)
                      .IsRequired()
                      .HasMaxLength(500);
                entity.Property(e => e.Category)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasDefaultValue("默认");
            });

            // 配置 Goal 实体
            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Title: 非空
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                // Content: 非空
                entity.Property(e => e.Content)
                      .IsRequired()
                      .HasMaxLength(1000);

                // AchieveDate: 非空，Goal 需要特殊处理
                entity.Property(e => e.AchieveDate)
                      .IsRequired();

                // TargetDate: 目标日期，非空
                entity.Property(e => e.TargetDate)
                      .IsRequired();

                entity.Property(e => e.AchieveDate)
                      .HasDefaultValue("1-1-1");
            });
        }
    }
}