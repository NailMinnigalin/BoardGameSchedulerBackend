using BoardGameSchedulerBackend.BusinessLayer.ApplicationLayer;
using BoardGameSchedulerBackend.DataLayer;
using Microsoft.AspNetCore.Identity;

namespace BoardGameSchedulerBackend.Infrastructure
{
    public class IdentityUserRepositoryUserCreationFailedException : Exception { }
    public class IdentityUserRepositoryInvalidPasswordException : Exception { }

    public class IdentityUserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityErrorDescriber _identityErrorDescriber = new IdentityErrorDescriber();

        public IdentityUserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateAsync(User user, string password)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, password);
            if (!result.Succeeded)
            {
                ThrowException(result);
            }

            user.Id = Guid.Parse(identityUser.Id);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser == null)
                return null;

            return new User
            {
                Id = id,
                UserName = identityUser.UserName,
                Email = identityUser.Email
            };
        }

        private void ThrowException(IdentityResult result)
        {
            if (result.Errors.Any(e => IsInvalidPasswordIdentityError(e)))
            {
                throw new IdentityUserRepositoryInvalidPasswordException();
            }

            throw new IdentityUserRepositoryUserCreationFailedException();
        }

        private bool IsInvalidPasswordIdentityError(IdentityError error)
        {
            List<IdentityError> invalidPassowrdErrors =
            [
                _identityErrorDescriber.PasswordRequiresNonAlphanumeric(),
                _identityErrorDescriber.PasswordRequiresLower(),
                _identityErrorDescriber.PasswordRequiresDigit(),
                _identityErrorDescriber.PasswordRequiresUpper(),
                _identityErrorDescriber.PasswordRequiresUniqueChars(10)
            ];

            return invalidPassowrdErrors.Any(e => e.Code == error.Code);
        }
    }
}
