using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.BusinessLayer.Entity;
using Microsoft.AspNetCore.Identity;
using SignInResult = BoardGameSchedulerBackend.BusinessLayer.SignInResult;

namespace BoardGameSchedulerBackend.Infrastructure
{
	/// <summary>
	/// Low-level user manager. Decouple business layer from Identity framework
	/// </summary>
	public class IdentityUserRepository : IUserRepository
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IdentityErrorDescriber _identityErrorDescriber = new IdentityErrorDescriber();

		public IdentityUserRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<UserCreationResult> CreateAsync(string userName, string userEmail, string password)
		{
			var identityUser = new IdentityUser
			{
				UserName = userName,
				Email = userEmail,
			};

			var result = await _userManager.CreateAsync(identityUser, password);
			return BuildUserCreationResult(result);
		}

		public async Task<User?> GetByIdAsync(Guid id)
		{
			var identityUser = await _userManager.FindByIdAsync(id.ToString());
			if (identityUser == null)
				return null;

			return new User
			{
				Id = id,
				UserName = identityUser.UserName!,
				Email = identityUser.Email!
			};
		}

		public async Task<SignInResult> SignInAsync(string userName, string passwrd)
		{
			var result = await _signInManager.PasswordSignInAsync(userName, passwrd, false, false);
			return new SignInResult { IsSuccesful = result.Succeeded };
		}

		public async Task SignOut()
		{
			await _signInManager.SignOutAsync();
		}

		private UserCreationResult BuildUserCreationResult(IdentityResult identityResult)
		{
			if (identityResult.Succeeded)
				return new UserCreationResult();

			List<UserCreationResult.ErrorCode> errors = identityResult.Errors.Select(e => ToUserCreationErrorCode(e)).ToList();
			return new UserCreationResult(errors);
		}

		private UserCreationResult.ErrorCode ToUserCreationErrorCode(IdentityError error)
		{
			const string email = "email";
			const string userName = "userName";

			if (_identityErrorDescriber.DuplicateEmail(email).Code == error.Code)
				return UserCreationResult.ErrorCode.DuplicateEmail;
			else if (_identityErrorDescriber.InvalidEmail(email).Code == error.Code)
				return UserCreationResult.ErrorCode.InvalidEmail;
			else if (_identityErrorDescriber.PasswordRequiresUniqueChars(1).Code == error.Code)
				return UserCreationResult.ErrorCode.PasswordRequiresUniqueChars;
			else if (_identityErrorDescriber.DuplicateUserName(userName).Code == error.Code)
				return UserCreationResult.ErrorCode.DuplicateUserName;
			else if (_identityErrorDescriber.InvalidUserName(userName).Code == error.Code)
				return UserCreationResult.ErrorCode.InvalidUserName;
			else if (_identityErrorDescriber.PasswordRequiresDigit().Code == error.Code)
				return UserCreationResult.ErrorCode.PasswordRequiresDigit;
			else if (_identityErrorDescriber.PasswordRequiresLower().Code == error.Code)
				return UserCreationResult.ErrorCode.PasswordRequiresLower;
			else if (_identityErrorDescriber.PasswordRequiresNonAlphanumeric().Code == error.Code)
				return UserCreationResult.ErrorCode.PasswordRequiresNonAlphanumeric;
			else if (_identityErrorDescriber.PasswordRequiresUpper().Code == error.Code)
				return UserCreationResult.ErrorCode.PasswordRequiresUpper;
			else if (_identityErrorDescriber.PasswordTooShort(1).Code == error.Code)
				return UserCreationResult.ErrorCode.PasswordTooShort;

			throw new ArgumentOutOfRangeException($"Can't find matched UserCreationErrorCode for IdentityError with code = {error.Code}");
		}
	}
}
