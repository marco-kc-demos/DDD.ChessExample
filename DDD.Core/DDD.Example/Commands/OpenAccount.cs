namespace DDD.Example.Commands
{
    public class OpenAccount
    {
        public string Owner { get; set; }

        public OpenAccount(string owner)
        {
            Owner = owner;
        }
    }
}