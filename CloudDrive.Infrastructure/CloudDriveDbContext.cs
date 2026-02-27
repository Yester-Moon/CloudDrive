using CloudDrive.Common.Models;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudDrive.Infrastructure
{
    /// <summary>
    /// CloudDrive 数据库上下文
    /// </summary>
    public class CloudDriveDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        private readonly IMediator? _mediator;

        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<ShareLink> ShareLinks { get; set; }

        public CloudDriveDbContext(DbContextOptions<CloudDriveDbContext> options, IMediator? mediator = null)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 FileItem 实体
            modelBuilder.Entity<FileItem>(entity =>
            {
                entity.ToTable("FileItems");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Extension)
                    .HasMaxLength(50);

                entity.Property(e => e.MimeType)
                    .HasMaxLength(100);

                entity.Property(e => e.Tags)
                    .HasMaxLength(500);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.ThumbnailUrl)
                    .HasMaxLength(500);

                // 值对象配置
                entity.OwnsOne(e => e.Size, size =>
                {
                    size.Property(s => s.bytesize).HasColumnName("Size");
                });

                entity.OwnsOne(e => e.StoragePath, path =>
                {
                    path.Property(p => p.path).HasColumnName("StoragePath").HasMaxLength(500);
                });

                entity.OwnsOne(e => e.Hash, hash =>
                {
                    hash.Property(h => h.hash).HasColumnName("Hash").HasMaxLength(64);
                    hash.HasIndex(h => h.hash).HasDatabaseName("IX_FileItems_Hash");
                });

                // 索引配置
                entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_FileItems_OwnerId");
                entity.HasIndex(e => e.ParentFolderId).HasDatabaseName("IX_FileItems_ParentFolderId");
                entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_FileItems_IsDeleted");
                entity.HasIndex(e => e.CreationTime).HasDatabaseName("IX_FileItems_CreationTime");

                // 关系配置
                entity.HasOne(e => e.Owner)
                    .WithMany(u => u.FileItems)
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 查询过滤器（全局查询过滤器 - 自动过滤软删除数据）
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // 配置 ShareLink 实体
            modelBuilder.Entity<ShareLink>(entity =>
            {
                entity.ToTable("ShareLinks");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ShareCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Title)
                    .HasMaxLength(200);

                entity.Property(e => e.AccessPassword)
                    .HasMaxLength(50);

                // 索引配置
                entity.HasIndex(e => e.ShareCode).IsUnique().HasDatabaseName("IX_ShareLinks_ShareCode");
                entity.HasIndex(e => e.FileItemId).HasDatabaseName("IX_ShareLinks_FileItemId");
                entity.HasIndex(e => e.CreatorId).HasDatabaseName("IX_ShareLinks_CreatorId");
                entity.HasIndex(e => e.IsCancelled).HasDatabaseName("IX_ShareLinks_IsCancelled");
                entity.HasIndex(e => e.ExpirationTime).HasDatabaseName("IX_ShareLinks_ExpirationTime");

                // 关系配置
                entity.HasOne(e => e.FileItem)
                    .WithMany()
                    .HasForeignKey(e => e.FileItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Creator)
                    .WithMany(u => u.ShareLinks)
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 查询过滤器
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // 配置 User 实体
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(100);

                entity.Property(e => e.AvatarUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.BanReason)
                    .HasMaxLength(500);

                // 索引配置
                entity.HasIndex(e => e.VipLevel).HasDatabaseName("IX_Users_VipLevel");
                entity.HasIndex(e => e.IsBanned).HasDatabaseName("IX_Users_IsBanned");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");

                // 忽略领域事件（不存储到数据库）
                entity.Ignore(e => e.DomainEvents);
            });

            // 配置 Identity 表名（可选）
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }

        /// <summary>
        /// 保存更改并发布领域事件
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 收集 AggregateRootEntity（FileItem、ShareLink）的领域事件
            var aggregateEvents = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.Entity.GetNotifications().Any())
                .SelectMany(e => e.Entity.GetNotifications())
                .ToList();

            // 收集 User 的领域事件
            var userEvents = ChangeTracker.Entries<User>()
                .Where(e => e.Entity.DomainEvents.Any())
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // 先保存数据
            var result = await base.SaveChangesAsync(cancellationToken);

            // 保存成功后发布领域事件
            if (_mediator != null)
            {
                foreach (var domainEvent in aggregateEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                foreach (var domainEvent in userEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }

            // 清除已发布的事件
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                entry.Entity.ClearNotifications();
            }

            foreach (var entry in ChangeTracker.Entries<User>())
            {
                entry.Entity.ClearDomainEvents();
            }

            return result;
        }
    }
}

