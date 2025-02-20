using BoardGameSchedulerBackend.BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameSchedulerBackend.Controllers
{
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		[Route("/register")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Register([FromBody] RegistrationData registrationData)
		{
			var userCreationResult = await _userService.RegisterUserAsync(registrationData.UserName, registrationData.Email, registrationData.Password);
			if (userCreationResult.IsSuccessful)
				return Created("", userCreationResult);
			return BadRequest(userCreationResult);
		}

		[HttpPost]
		[Route("/signin")]
		public async Task<IActionResult> SignIn([FromBody] SignInData signInData)
		{
			var result = await _userService.SignInAsync(signInData.UserName, signInData.Password);
			if (result.IsSuccesful)
				return Ok();
			return BadRequest();
		}

		[HttpGet]
		[Route("/signout")]
		public async Task UserSignOut()
		{

		}

		[HttpGet]
		[Route("/testauth")]
		[Authorize]
		public IActionResult TestAuth()
		{
			return Ok();
		}
	}

	public class RegistrationData()
	{
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
	}

	public class SignInData()
	{
		public required string UserName { get; set; }
		public required string Password { get; set; }
	}
}
