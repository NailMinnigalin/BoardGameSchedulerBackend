using BoardGameSchedulerBackend.BusinessLayer;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameSchedulerBackend.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RegistrationController : ControllerBase
	{
		private readonly IUserService _userService;

		public RegistrationController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("/")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Register([FromBody] RegistrationData registrationData)
		{
			var userCreationResult = await _userService.RegisterUserAsync(registrationData.UserName, registrationData.Email, registrationData.Password);
			if (userCreationResult.IsSuccessful)
				return Created("", userCreationResult);
			return BadRequest(userCreationResult);
		}
	}

	public class RegistrationData()
	{
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
	}
}
