using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace ImxServer.Models
{
    public class GameContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Player> Players { get; set; }

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
        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool RegisterImx { get; set; }

    }

    public class Token
    {
        public int TokenId { get; set; }
    }


    public class Monster
    {
        public int MonsterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Element { get; set; }
        public string AtkName1 { get; set; }
        public string AtkName2 { get; set; }
    }

    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ItemType { get; set; }

    }

}