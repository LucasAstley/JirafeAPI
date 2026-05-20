using Microsoft.EntityFrameworkCore;
using JirafeAPI.Entities;

namespace JirafeAPI.Data;

public class TaskBoardDbContext : DbContext
{
    public TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardMember> BoardMembers { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardMember> CardMembers { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<CardLabel> CardLabels { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(e => e.RefreshTokens)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.WorkspaceMembers)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BoardMembers)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.CardMembers)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Comments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsRevoked).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasIndex(e => e.Token).IsUnique();
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasMany(e => e.WorkspaceMembers)
                .WithOne(e => e.Workspace)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Boards)
                .WithOne(e => e.Workspace)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkspaceMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WorkspaceId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
            entity.Property(e => e.JoinedAt).IsRequired();
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.WorkspaceId).IsRequired();

            entity.HasMany(e => e.BoardMembers)
                .WithOne(e => e.Board)
                .HasForeignKey(e => e.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Lists)
                .WithOne(e => e.Board)
                .HasForeignKey(e => e.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Labels)
                .WithOne(e => e.Board)
                .HasForeignKey(e => e.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BoardMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BoardId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.JoinedAt).IsRequired();

            entity.HasIndex(e => e.BoardId);
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Position).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.BoardId).IsRequired();

            entity.HasIndex(e => e.Position);

            entity.HasMany(e => e.Cards)
                .WithOne(e => e.List)
                .HasForeignKey(e => e.ListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Description).HasColumnType("TEXT");
            entity.Property(e => e.DueDate);
            entity.Property(e => e.Position).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ListId).IsRequired();

            entity.HasIndex(e => new { e.ListId, e.Position });
            entity.HasIndex(e => e.DueDate);

            entity.HasMany(e => e.CardMembers)
                .WithOne(e => e.Card)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.CardLabels)
                .WithOne(e => e.Card)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Comments)
                .WithOne(e => e.Card)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CardMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CardId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ColorHex).HasMaxLength(7).IsRequired();
            entity.Property(e => e.BoardId).IsRequired();

            entity.HasMany(e => e.CardLabels)
                .WithOne(e => e.Label)
                .HasForeignKey(e => e.LabelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CardLabel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CardId).IsRequired();
            entity.Property(e => e.LabelId).IsRequired();
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasColumnType("TEXT").IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.CardId).IsRequired();
            entity.Property(e => e.UserId);
        });
    }
}

