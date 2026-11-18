namespace ConsoleProject.Models
{
    internal class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";

        public override string ToString()
        {
            return $"{Id}|{Name}|{Address}";
        }
    }
}
