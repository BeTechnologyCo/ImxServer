using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nethereum.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Security.Policy;

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
    }

    [Index(nameof(TokenId), nameof(MoveId), IsUnique  = true)]
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