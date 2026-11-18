using ConsoleProject.Exceptions;
using ConsoleProject.Models;

namespace ConsoleProject.Repositories
{
	internal class UserRepository
	{
		private readonly string _filePath;

		public UserRepository()
		{
			_filePath = GetFilePath("user.txt");
			CreateFileIfItDoesNotExist();
		}

		public void SaveUser(User user)
		{
			File.AppendAllText(_filePath, user + Environment.NewLine);
		}

		public List<User> GetAllUsers()
		{
			var users = new List<User>();
			string[] lines = File.ReadAllLines(_filePath);

			foreach (string line in lines)
			{
				if (string.IsNullOrWhiteSpace(line))
					continue;

				string[] values = line.Split('|');
				if (values.Length != 7 && values.Length != 8)
					continue;

				if (!int.TryParse(values[0], out int id))
					continue;
				if (!bool.TryParse(values[4], out bool isLogged))
					continue;
				if (!bool.TryParse(values[6], out bool isVerified))
					continue;

				var user = new User();
				user.Id = id;
				user.Name = values[1];
				user.Email = values[2];
				user.Password = values[3];
				user.IsLogged = isLogged;
				user.VerificationCode = values[5];
				user.IsVerified = isVerified;

				if (values.Length == 8)
				{
					bool.TryParse(values[7], out bool isAdmin);
					user.IsAdmin = isAdmin;
				}

				users.Add(user);
			}

			return users;
		}

		public User? GetUserByEmail(string email)
		{
			List<User> users = GetAllUsers();

			foreach (User user in users)
			{
				if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
					return user;
			}

			return null;
		}

		public void VerifyUser(string email, string code)
		{
			List<User> users = GetAllUsers();
			User? user = GetUserByEmail(email);

			if (user == null)
				throw new UserNotFoundException();

			if (user.VerificationCode != code)
				throw new InvalidVerificationCodeException();

			user.IsVerified = true;
			SaveAllUsers(users);
		}

		public void SaveAllUsers(List<User> users)
		{
			List<string> lines = new List<string>();
			foreach (User user in users)
				lines.Add(user.ToString());

			File.WriteAllLines(_filePath, lines);
		}

		private static string GetFilePath(string fileName)
		{
			return Path.GetFullPath(Path.Combine(
				AppContext.BaseDirectory, "..", "..", "..", "Data", fileName));
		}

		private void CreateFileIfItDoesNotExist()
		{
			Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
			if (!File.Exists(_filePath))
				File.Create(_filePath).Close();
		}
	}
}
