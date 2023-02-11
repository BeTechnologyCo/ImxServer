﻿// <auto-generated />
using ImxServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ImxServer.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20230211231400_InitializeData")]
    partial class InitializeData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ImxServer.Models.Monster", b =>
                {
                    b.Property<int>("MonsterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MonsterId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MonsterId");

                    b.ToTable("Monsters");

                    b.HasData(
                        new
                        {
                            MonsterId = 1,
                            Name = "Fordin"
                        },
                        new
                        {
                            MonsterId = 2,
                            Name = "Kroki"
                        },
                        new
                        {
                            MonsterId = 3,
                            Name = "Devidin"
                        },
                        new
                        {
                            MonsterId = 4,
                            Name = "Aerodin"
                        },
                        new
                        {
                            MonsterId = 5,
                            Name = "Weastoat"
                        });
                });

            modelBuilder.Entity("ImxServer.Models.MonsterMove", b =>
                {
                    b.Property<int>("MonsterMoveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MonsterMoveId"));

                    b.Property<int>("MoveId")
                        .HasColumnType("integer");

                    b.Property<int>("TokenId")
                        .HasColumnType("integer");

                    b.HasKey("MonsterMoveId");

                    b.HasIndex("MoveId");

                    b.HasIndex("TokenId", "MoveId")
                        .IsUnique();

                    b.ToTable("MonsterMoves");
                });

            modelBuilder.Entity("ImxServer.Models.Move", b =>
                {
                    b.Property<int>("MoveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MoveId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MoveId");

                    b.ToTable("Moves");

                    b.HasData(
                        new
                        {
                            MoveId = 1,
                            Name = "Cut"
                        },
                        new
                        {
                            MoveId = 2,
                            Name = "Ember"
                        },
                        new
                        {
                            MoveId = 3,
                            Name = "Growl"
                        },
                        new
                        {
                            MoveId = 4,
                            Name = "PoisonPowder"
                        },
                        new
                        {
                            MoveId = 5,
                            Name = "QuickAttack"
                        },
                        new
                        {
                            MoveId = 6,
                            Name = "SandAttack"
                        },
                        new
                        {
                            MoveId = 7,
                            Name = "Scratch"
                        },
                        new
                        {
                            MoveId = 8,
                            Name = "Sing"
                        },
                        new
                        {
                            MoveId = 9,
                            Name = "SuperSonic"
                        },
                        new
                        {
                            MoveId = 10,
                            Name = "Surf"
                        },
                        new
                        {
                            MoveId = 11,
                            Name = "Tackle"
                        },
                        new
                        {
                            MoveId = 12,
                            Name = "ThunderWave"
                        },
                        new
                        {
                            MoveId = 13,
                            Name = "Vine"
                        });
                });

            modelBuilder.Entity("ImxServer.Models.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PlayerId"));

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("RegisterImx")
                        .HasColumnType("boolean");

                    b.HasKey("PlayerId");

                    b.HasIndex("Account")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("ImxServer.Models.Token", b =>
                {
                    b.Property<int>("TokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TokenId"));

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<int>("MonsterId")
                        .HasColumnType("integer");

                    b.HasKey("TokenId");

                    b.HasIndex("MonsterId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("ImxServer.Models.MonsterMove", b =>
                {
                    b.HasOne("ImxServer.Models.Move", "Move")
                        .WithMany()
                        .HasForeignKey("MoveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImxServer.Models.Token", "Token")
                        .WithMany()
                        .HasForeignKey("TokenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Move");

                    b.Navigation("Token");
                });

            modelBuilder.Entity("ImxServer.Models.Token", b =>
                {
                    b.HasOne("ImxServer.Models.Monster", "Monster")
                        .WithMany()
                        .HasForeignKey("MonsterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Monster");
                });
#pragma warning restore 612, 618
        }
    }
}
