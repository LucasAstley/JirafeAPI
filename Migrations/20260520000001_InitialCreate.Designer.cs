using System;
using JirafeAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JirafeAPI.Migrations
{
    [DbContext(typeof(TaskBoardDbContext))]
    [Migration("20260520000001_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.8");

            modelBuilder.Entity("JirafeAPI.Entities.Board", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("TEXT");

                b.Property<int>("WorkspaceId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("WorkspaceId");

                b.ToTable("Boards");
            });

            modelBuilder.Entity("JirafeAPI.Entities.BoardMember", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("BoardId")
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("JoinedAt")
                    .HasColumnType("TEXT");

                b.Property<int>("UserId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("BoardId");

                b.HasIndex("UserId");

                b.ToTable("BoardMembers");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Card", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Description")
                    .HasColumnType("TEXT");

                b.Property<DateTime?>("DueDate")
                    .HasColumnType("TEXT");

                b.Property<int>("ListId")
                    .HasColumnType("INTEGER");

                b.Property<int>("Position")
                    .HasColumnType("INTEGER");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("DueDate");

                b.HasIndex("ListId", "Position");

                b.ToTable("Cards");
            });

            modelBuilder.Entity("JirafeAPI.Entities.CardLabel", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("CardId")
                    .HasColumnType("INTEGER");

                b.Property<int>("LabelId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("CardId");

                b.HasIndex("LabelId");

                b.ToTable("CardLabels");
            });

            modelBuilder.Entity("JirafeAPI.Entities.CardMember", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("CardId")
                    .HasColumnType("INTEGER");

                b.Property<int>("UserId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("CardId");

                b.HasIndex("UserId");

                b.ToTable("CardMembers");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Comment", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("CardId")
                    .HasColumnType("INTEGER");

                b.Property<string>("Content")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("TEXT");

                b.Property<int?>("UserId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("CardId");

                b.HasIndex("UserId");

                b.ToTable("Comments");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Label", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("BoardId")
                    .HasColumnType("INTEGER");

                b.Property<string>("ColorHex")
                    .IsRequired()
                    .HasMaxLength(7)
                    .HasColumnType("TEXT");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("BoardId");

                b.ToTable("Labels");
            });

            modelBuilder.Entity("JirafeAPI.Entities.List", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("BoardId")
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<int>("Position")
                    .HasColumnType("INTEGER");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("BoardId");

                b.HasIndex("Position");

                b.ToTable("Lists");
            });

            modelBuilder.Entity("JirafeAPI.Entities.RefreshToken", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<DateTime>("ExpiresAt")
                    .HasColumnType("TEXT");

                b.Property<bool>("IsRevoked")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER")
                    .HasDefaultValue(false);

                b.Property<string>("Token")
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("TEXT");

                b.Property<int>("UserId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("Token")
                    .IsUnique();

                b.HasIndex("UserId");

                b.ToTable("RefreshTokens");
            });

            modelBuilder.Entity("JirafeAPI.Entities.User", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("TEXT");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("TEXT");

                b.Property<string>("Role")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("TEXT");

                b.Property<string>("Username")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.HasIndex("Username")
                    .IsUnique();

                b.ToTable("Users");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Workspace", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Description")
                    .HasMaxLength(500)
                    .HasColumnType("TEXT");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Workspaces");
            });

            modelBuilder.Entity("JirafeAPI.Entities.WorkspaceMember", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("JoinedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Role")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("TEXT");

                b.Property<int>("UserId")
                    .HasColumnType("INTEGER");

                b.Property<int>("WorkspaceId")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.HasIndex("WorkspaceId");

                b.ToTable("WorkspaceMembers");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Board", b =>
            {
                b.HasOne("JirafeAPI.Entities.Workspace", "Workspace")
                    .WithMany("Boards")
                    .HasForeignKey("WorkspaceId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Workspace");
            });

            modelBuilder.Entity("JirafeAPI.Entities.BoardMember", b =>
            {
                b.HasOne("JirafeAPI.Entities.Board", "Board")
                    .WithMany("BoardMembers")
                    .HasForeignKey("BoardId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("JirafeAPI.Entities.User", "User")
                    .WithMany("BoardMembers")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Board");

                b.Navigation("User");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Card", b =>
            {
                b.HasOne("JirafeAPI.Entities.List", "List")
                    .WithMany("Cards")
                    .HasForeignKey("ListId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("List");
            });

            modelBuilder.Entity("JirafeAPI.Entities.CardLabel", b =>
            {
                b.HasOne("JirafeAPI.Entities.Card", "Card")
                    .WithMany("CardLabels")
                    .HasForeignKey("CardId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("JirafeAPI.Entities.Label", "Label")
                    .WithMany("CardLabels")
                    .HasForeignKey("LabelId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Card");

                b.Navigation("Label");
            });

            modelBuilder.Entity("JirafeAPI.Entities.CardMember", b =>
            {
                b.HasOne("JirafeAPI.Entities.Card", "Card")
                    .WithMany("CardMembers")
                    .HasForeignKey("CardId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("JirafeAPI.Entities.User", "User")
                    .WithMany("CardMembers")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Card");

                b.Navigation("User");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Comment", b =>
            {
                b.HasOne("JirafeAPI.Entities.Card", "Card")
                    .WithMany("Comments")
                    .HasForeignKey("CardId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("JirafeAPI.Entities.User", "User")
                    .WithMany("Comments")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.SetNull);

                b.Navigation("Card");

                b.Navigation("User");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Label", b =>
            {
                b.HasOne("JirafeAPI.Entities.Board", "Board")
                    .WithMany("Labels")
                    .HasForeignKey("BoardId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Board");
            });

            modelBuilder.Entity("JirafeAPI.Entities.List", b =>
            {
                b.HasOne("JirafeAPI.Entities.Board", "Board")
                    .WithMany("Lists")
                    .HasForeignKey("BoardId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Board");
            });

            modelBuilder.Entity("JirafeAPI.Entities.RefreshToken", b =>
            {
                b.HasOne("JirafeAPI.Entities.User", "User")
                    .WithMany("RefreshTokens")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("JirafeAPI.Entities.WorkspaceMember", b =>
            {
                b.HasOne("JirafeAPI.Entities.User", "User")
                    .WithMany("WorkspaceMembers")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("JirafeAPI.Entities.Workspace", "Workspace")
                    .WithMany("WorkspaceMembers")
                    .HasForeignKey("WorkspaceId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");

                b.Navigation("Workspace");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Board", b =>
            {
                b.Navigation("BoardMembers");

                b.Navigation("Labels");

                b.Navigation("Lists");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Card", b =>
            {
                b.Navigation("CardLabels");

                b.Navigation("CardMembers");

                b.Navigation("Comments");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Label", b =>
            {
                b.Navigation("CardLabels");
            });

            modelBuilder.Entity("JirafeAPI.Entities.List", b =>
            {
                b.Navigation("Cards");
            });

            modelBuilder.Entity("JirafeAPI.Entities.User", b =>
            {
                b.Navigation("BoardMembers");

                b.Navigation("CardMembers");

                b.Navigation("Comments");

                b.Navigation("RefreshTokens");

                b.Navigation("WorkspaceMembers");
            });

            modelBuilder.Entity("JirafeAPI.Entities.Workspace", b =>
            {
                b.Navigation("Boards");

                b.Navigation("WorkspaceMembers");
            });
#pragma warning restore 612, 618
        }
    }
}

