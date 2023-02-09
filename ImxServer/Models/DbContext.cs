namespace ImxServer.Models
{
    public class DbContext
    {
        public int MyProperty { get; set; }
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