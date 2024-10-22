﻿// <auto-generated />
using System;
using ManagerGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ManagerGame.Infra.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ManagerGame.Core.Domain.Draft", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_date");

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uuid")
                        .HasColumnName("league_id");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_drafts");

                    b.HasIndex("LeagueId")
                        .HasDatabaseName("ix_drafts_league_id");

                    b.ToTable("drafts", (string)null);
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.League", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_date");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_leagues");

                    b.ToTable("leagues", (string)null);
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Manager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_managers");

                    b.ToTable("managers", (string)null);
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("country");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("Position")
                        .HasColumnType("integer")
                        .HasColumnName("position");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid")
                        .HasColumnName("team_id");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_players");

                    b.HasIndex("TeamId")
                        .HasDatabaseName("ix_players_team_id");

                    b.ToTable("players", (string)null);
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_date");

                    b.Property<Guid?>("LeagueId")
                        .HasColumnType("uuid")
                        .HasColumnName("league_id");

                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uuid")
                        .HasColumnName("manager_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_teams");

                    b.HasIndex("LeagueId")
                        .HasDatabaseName("ix_teams_league_id");

                    b.HasIndex("ManagerId")
                        .HasDatabaseName("ix_teams_manager_id");

                    b.ToTable("teams", (string)null);
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Draft", b =>
                {
                    b.HasOne("ManagerGame.Core.Domain.League", "League")
                        .WithMany("Drafts")
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_drafts_leagues_league_id");

                    b.OwnsOne("ManagerGame.Core.Domain.Draft.DraftOrder#ManagerGame.Core.Domain.DraftOrder", "DraftOrder", b1 =>
                        {
                            b1.Property<Guid>("DraftId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<int>("_current")
                                .HasColumnType("integer")
                                .HasColumnName("draftOrderCurrent");

                            b1.Property<int>("_previous")
                                .HasColumnType("integer")
                                .HasColumnName("draftOrderPrevious");

                            b1.HasKey("DraftId");

                            b1.ToTable("drafts", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("DraftId")
                                .HasConstraintName("fk_drafts_drafts_id");
                        });

                    b.Navigation("DraftOrder")
                        .IsRequired();

                    b.Navigation("League");
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Player", b =>
                {
                    b.HasOne("ManagerGame.Core.Domain.Team", null)
                        .WithMany("Players")
                        .HasForeignKey("TeamId")
                        .HasConstraintName("fk_players_teams_team_id");
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Team", b =>
                {
                    b.HasOne("ManagerGame.Core.Domain.League", null)
                        .WithMany("Teams")
                        .HasForeignKey("LeagueId")
                        .HasConstraintName("fk_teams_leagues_league_id");

                    b.HasOne("ManagerGame.Core.Domain.Manager", null)
                        .WithMany("Teams")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_teams_managers_manager_id");
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.League", b =>
                {
                    b.Navigation("Drafts");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Manager", b =>
                {
                    b.Navigation("Teams");
                });

            modelBuilder.Entity("ManagerGame.Core.Domain.Team", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
