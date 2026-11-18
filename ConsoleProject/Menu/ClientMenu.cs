using ConsoleProject.Models;
using ConsoleProject.Services;

namespace ConsoleProject.Menu
{
	internal class ClientMenu
	{
		private readonly FoodService _foodService;
		private readonly RestaurantService _restaurantService;
		private readonly OrderService _orderService;

		public ClientMenu(FoodService foodService, RestaurantService restaurantService, OrderService orderService)
		{
			_foodService = foodService;
			_restaurantService = restaurantService;
			_orderService = orderService;
		}

		public void Show(User user)
		{
			while (true)
			{
				Console.WriteLine("\n--- Client menu ---");
				Console.WriteLine("1. Show foods");
				Console.WriteLine("2. Search food");
				Console.WriteLine("3. Place order");
				Console.WriteLine("4. My orders");
				Console.WriteLine("0. Logout");
				Console.Write("Choose: ");
				string choice = Console.ReadLine() ?? "";

				try
				{
					if (choice == "1") ShowFoods(_foodService.GetAllFoods());
					else if (choice == "2") SearchFoods();
					else if (choice == "3") PlaceOrder(user);
					else if (choice == "4") ShowMyOrders(user);
					else if (choice == "0") return;
					else Console.WriteLine("Invalid choice.");
				}
				catch (Exception error)
				{
					Console.WriteLine("Error: " + error.Message);
				}
			}
		}

		private void SearchFoods()
		{
			Console.Write("Search text: ");
			string text = Console.ReadLine() ?? "";
			ShowFoods(_foodService.SearchFoods(text));
		}

		private void PlaceOrder(User user)
		{
			List<Restaurant> restaurants = _restaurantService.GetAllRestaurants();
			if (restaurants.Count == 0)
			{
				Console.WriteLine("No restaurants found.");
				return;
			}

			foreach (Restaurant restaurant in restaurants)
				Console.WriteLine($"{restaurant.Id}. {restaurant.Name} - {restaurant.Address}");

			int restaurantId = ReadInt("Restaurant id: ");
			Restaurant? selectedRestaurant = _restaurantService.GetRestaurantById(restaurantId);
			if (selectedRestaurant == null)
			{
				Console.WriteLine("Restaurant was not found.");
				return;
			}

			List<Food> foods = _foodService.GetAllFoods()
				.Where(food => food.RestaurantId == selectedRestaurant.Id)
				.ToList();
			if (foods.Count == 0)
			{
				Console.WriteLine("The food list is empty.");
				return;
			}

			ShowFoods(foods);
			int foodId = ReadInt("Food id: ");
			Food? food = _foodService.GetFoodById(foodId);
			if (food == null || food.RestaurantId != selectedRestaurant.Id)
			{
				Console.WriteLine("Food was not found.");
				return;
			}

			int quantity = ReadInt("Quantity: ");
			Order order = _orderService.CreateOrder(user, food, selectedRestaurant, quantity);
			Console.WriteLine("Order created. Order id: " + order.Id);
		}

		private void ShowMyOrders(User user)
		{
			List<Order> orders = _orderService.GetOrdersForUser(user.Id);
			foreach (Order order in orders)
				Console.WriteLine($"{order.Id}. {order.RestaurantName} - {order.FoodName}, quantity: {order.Quantity}, date: {order.CreatedAt}");

			if (orders.Count == 0)
				Console.WriteLine("You have no orders.");
		}

		private static void ShowFoods(List<Food> foods)
		{
			foreach (Food food in foods)
				Console.WriteLine($"{food.Id}. {food.Name} - {food.Description}");

			if (foods.Count == 0)
				Console.WriteLine("No foods found.");
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
