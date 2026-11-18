using ConsoleProject.Exceptions;

namespace ConsoleProject.Models
{
	internal class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Password { get; set; } = "";
		public bool IsLogged { get; set; }
		public string VerificationCode { get; set; } = "";
		public bool IsVerified { get; set; }
		public bool IsAdmin { get; set; }

		private string _email = "";

		public string Email
		{
			get { return _email; }
			set
			{
				if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
					throw new InvalidEmailException();

				_email = value.Trim();
			}
		}

		public override string ToString()
		{
			return $"{Id}|{Name}|{Email}|{Password}|{IsLogged}|{VerificationCode}|{IsVerified}|{IsAdmin}";
		}
	}
}
