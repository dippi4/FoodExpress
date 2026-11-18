using ConsoleProject.Models;
using ConsoleProject.Services;

namespace ConsoleProject.Menu
{
	internal class AdminMenu
	{
		private readonly FoodService _foodService;
		private readonly RestaurantService _restaurantService;
		private readonly OrderService _orderService;

		public AdminMenu(FoodService foodService, RestaurantService restaurantService, OrderService orderService)
		{
			_foodService = foodService;
			_restaurantService = restaurantService;
			_orderService = orderService;
		}

		public void Show(User user)
		{
			while (true)
			{
				Console.WriteLine("\n--- Admin menu ---");
				Console.WriteLine("1. Show foods");
				Console.WriteLine("2. Add food");
				Console.WriteLine("3. Update food");
				Console.WriteLine("4. Delete food");
				Console.WriteLine("5. Show all orders");
				Console.WriteLine("6. Show restaurants");
				Console.WriteLine("7. Add restaurant");
				Console.WriteLine("0. Logout");
				Console.Write("Choose: ");
				string choice = Console.ReadLine() ?? "";

				try
				{
					if (choice == "1") ShowFoods();
					else if (choice == "2") AddFood();
					else if (choice == "3") UpdateFood();
					else if (choice == "4") DeleteFood();
					else if (choice == "5") ShowOrders();
					else if (choice == "6") ShowRestaurants();
					else if (choice == "7") AddRestaurant();
					else if (choice == "0") return;
					else Console.WriteLine("Invalid choice.");
				}
				catch (Exception error)
				{
					Console.WriteLine("Error: " + error.Message);
				}
			}
		}

		private void ShowFoods()
		{
			List<Food> foods = _foodService.GetAllFoods();
			foreach (Food food in foods)
			{
				string restaurant = _restaurantService.GetRestaurantById(food.RestaurantId)?.Name ?? "Unassigned";
				Console.WriteLine($"{food.Id}. {food.Name} - {food.Description} [{restaurant}]");
			}

			if (foods.Count == 0)
				Console.WriteLine("No foods found.");
		}

		private void AddFood()
		{
			Console.Write("Food name: ");
			string name = Console.ReadLine() ?? "";
			Console.Write("Description: ");
			string description = Console.ReadLine() ?? "";
			int restaurantId = ReadRestaurantId();

			_foodService.AddFood(name, description, restaurantId);
			Console.WriteLine("Food added.");
		}

		private void UpdateFood()
		{
			int id = ReadInt("Food id: ");
			Console.Write("New name: ");
			string name = Console.ReadLine() ?? "";
			Console.Write("New description: ");
			string description = Console.ReadLine() ?? "";
			int restaurantId = ReadRestaurantId();

			if (_foodService.UpdateFood(id, name, description, restaurantId))
				Console.WriteLine("Food updated.");
			else
				Console.WriteLine("Food was not found.");
		}

		private void DeleteFood()
		{
			int id = ReadInt("Food id: ");
			if (_foodService.DeleteFood(id))
				Console.WriteLine("Food deleted.");
			else
				Console.WriteLine("Food was not found.");
		}

		private void ShowOrders()
		{
			List<Order> orders = _orderService.GetAllOrders();
			foreach (Order order in orders)
				Console.WriteLine($"{order.Id}. User: {order.UserId}, Restaurant: {order.RestaurantName}, Food: {order.FoodName}, Quantity: {order.Quantity}");

			if (orders.Count == 0)
				Console.WriteLine("No orders found.");
		}

		private void ShowRestaurants()
		{
			List<Restaurant> restaurants = _restaurantService.GetAllRestaurants();
			foreach (Restaurant restaurant in restaurants)
				Console.WriteLine($"{restaurant.Id}. {restaurant.Name} - {restaurant.Address}");
			if (restaurants.Count == 0)
				Console.WriteLine("No restaurants found.");
		}

		private void AddRestaurant()
		{
			Console.Write("Restaurant name: ");
			string name = Console.ReadLine() ?? "";
			Console.Write("Address: ");
			string address = Console.ReadLine() ?? "";
			_restaurantService.AddRestaurant(name, address);
			Console.WriteLine("Restaurant added.");
		}

		private int ReadRestaurantId()
		{
			ShowRestaurants();
			return ReadInt("Restaurant id: ");
		}

		private static int ReadInt(string message)
		{
			Console.Write(message);
			if (int.TryParse(Console.ReadLine(), out int number))
				return number;
			throw new ArgumentException("Please enter a number.");
		}
	}
}
