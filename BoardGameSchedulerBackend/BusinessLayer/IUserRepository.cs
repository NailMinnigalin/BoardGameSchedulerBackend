using BoardGameSchedulerBackend.BusinessLayer.Entity;

namespace BoardGameSchedulerBackend.BusinessLayer
{
	public class UserCreationResult
	{
		public enum ErrorCode
		{
			DuplicateEmail,
			InvalidEmail,
			PasswordRequiresUniqueChars,
			DuplicateUserName,
			InvalidUserName,
			PasswordRequiresDigit,
			PasswordRequiresLower,
			PasswordRequiresNonAlphanumeric,
			PasswordRequiresUpper,
			PasswordTooShort
		}

		public List<ErrorCode> Errors { get; init; }
		public bool IsSuccessful => Errors.Count == 0;

		public UserCreationResult()
		{
			Errors = new List<ErrorCode>();
		}

		public UserCreationResult(List<ErrorCode> errors)
		{
			Errors = errors;
		}
	}

	public class SignInResult
	{
		public required bool IsSuccesful { get; init;}
	}

	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id);
		Task<UserCreationResult> CreateAsync(string userName, string userEmail, string password);
		Task<SignInResult> SignInAsync(string userName, string passwrd);
		Task SignOut();
	}
}
