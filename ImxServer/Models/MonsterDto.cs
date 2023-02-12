using System;
namespace ImxServer.Models
{
	public class AddMonsterDto
	{

		public string Name { get; set; }
        public int Level { get; set; }
        public List<string> Moves { get; set; }
    }

    public class UpdateMonsterDto
    {

        public int TokenId { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public List<string> Moves { get; set; }
    }
}

