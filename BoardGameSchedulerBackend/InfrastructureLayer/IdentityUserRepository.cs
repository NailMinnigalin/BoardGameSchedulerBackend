using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.DataLayer;
using Microsoft.AspNetCore.Identity;

namespace BoardGameSchedulerBackend.Infrastructure
{
	/// <summary>
	/// Low-level user manager. Decouple business layer from Identity framework
	/// </summary>
	public class IdentityUserRepository : IUserRepository
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IdentityErrorDescriber _identityErrorDescriber = new IdentityErrorDescriber();

		public IdentityUserRepository(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
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

		private UserCreationResult BuildUserCreationResult(IdentityResult identityResult)
		{
			if (identityResult.Succeeded)
				return new UserCreationResult();

			List<UserCreationResult.ErrorCode> errors = identityResult.Errors.Select(e => ToUserCreationErrorCode(e)).ToList();
			return new UserCreationResult(errors);
		}

		private UserCreationResult.ErrorCode ToUserCreationErrorCode(IdentityError error)
		{
			if (_identityErrorDescriber.DuplicateEmail("anyEmail").Code == error.Code)
				return UserCreationResult.ErrorCode.DuplicateEmail;

			throw new ArgumentOutOfRangeException($"Can't find matched UserCreationErrorCode for IdentityError with code = {error.Code}");
		}
	}
}
