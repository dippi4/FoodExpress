using ConsoleProject.Models;

namespace ConsoleProject.Repositories
{
    internal class RestaurantRepository
    {
        private readonly string _filePath;

        public RestaurantRepository()
        {
            _filePath = GetFilePath("restaurant.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();
        }

        public void SaveRestaurant(Restaurant restaurant)
        {
            File.AppendAllText(_filePath, restaurant + Environment.NewLine);
        }

        public List<Restaurant> GetAllRestaurants()
        {
            var restaurants = new List<Restaurant>();
            foreach (string line in File.ReadAllLines(_filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] values = line.Split('|');
                if (values.Length != 3 || !int.TryParse(values[0], out int id))
                    continue;

                restaurants.Add(new Restaurant { Id = id, Name = values[1], Address = values[2] });
            }
            return restaurants;
        }

        public Restaurant? GetRestaurantById(int id)
        {
            return GetAllRestaurants().FirstOrDefault(restaurant => restaurant.Id == id);
        }

        private static string GetFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", fileName));
        }
    }
}
