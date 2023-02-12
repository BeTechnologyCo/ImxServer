using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nethereum.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Security.Policy;
using static ImxServer.Models.GameContext;
using System.Xml.Linq;

namespace ImxServer.Models
{
    public class GameContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Player> Players { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<MonsterMove> MonsterMoves { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<Monster> Monsters { get; set; }

        public GameContext(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DatabaseConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Monster>()
        .HasData(
            new Monster
            {
                MonsterId = 1,
                Name = "Fordin"
            },
            new Monster
            {
                MonsterId = 2,
                Name = "Kroki"
            }, new Monster
            {
                MonsterId = 3,
                Name = "Devidin"
            }, new Monster
            {
                MonsterId = 4,
                Name = "Aerodin"
            }, new Monster
            {
                MonsterId = 5,
                Name = "Weastoat"
            });

            modelBuilder.Entity<Move>()
.HasData(
            new Move
            {
                MoveId = 1,
                Name = "Cut"
            },
new Move
{
    MoveId = 2,
    Name = "Ember"
}, new Move
{
    MoveId = 3,
    Name = "Growl"
}, new Move
{
    MoveId = 4,
    Name = "PoisonPowder"
}, new Move
{
    MoveId = 5,
    Name = "QuickAttack"
}, new Move
{
    MoveId = 6,
    Name = "SandAttack"
}, new Move
{
    MoveId = 7,
    Name = "Scratch"
}, new Move
{
    MoveId = 8,
    Name = "Sing"
}, new Move
{
    MoveId = 9,
    Name = "SuperSonic"
}, new Move
{
    MoveId = 10,
    Name = "Surf"
}, new Move
{
    MoveId = 11,
    Name = "Tackle"
}, new Move
{
    MoveId = 12,
    Name = "ThunderWave"
}, new Move
{
    MoveId = 13,
    Name = "Vine"
});

        }
    }

    [Index(nameof(Account), IsUnique = true)]
    [Index(nameof(Name), IsUnique = true)]
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool RegisterImx { get; set; }

    }

    public class Token
    {
        public int TokenId { get; set; }
        public int MonsterId { get; set; }
        public Monster Monster { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
    }

    [Index(nameof(TokenId), nameof(MoveId), IsUnique = true)]
    public class MonsterMove
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MonsterMoveId { get; set; }
        public int TokenId { get; set; }
        public Token Token { get; set; }
        public int MoveId { get; set; }
        public Move Move { get; set; }
    }

    public class Monster
    {
        public int MonsterId { get; set; }
        public string Name { get; set; }
    }

    public class Move
    {
        public int MoveId { get; set; }
        public string Name { get; set; }
    }
}
