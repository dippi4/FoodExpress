namespace ConsoleProject.Models
{
    internal class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = "";
        public string FoodName { get; set; } = "";
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{Id}|{UserId}|{FoodId}|{FoodName}|{Quantity}|{CreatedAt:O}|{RestaurantId}|{RestaurantName}";
        }
    }
}
