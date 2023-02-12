using System;
namespace ImxServer.Models
{
	public class AddMonsterDto
	{

		public string Name { get; set; }
        public int Level { get; set; }
    }

    public class UpdateMonsterDto
    {

        public int TokenId { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
    }

    public class TransferMonsterDto
    {
        public int TokenId { get; set; }
        public string UserName { get; set; }
    }

    public class SendMonsterDto
    {
        public string TokenId { get; set; }
        public string ReceiverAddress { get; set; }
    }
}

