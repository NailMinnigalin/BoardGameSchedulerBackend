using BoardGameSchedulerBackend.Controllers;
using System.Net.Http.Json;

namespace BGSIntegrationTesting
{
	[TestClass]
	public class UserControllerTest
	{
		BGSWebApplicationFactory<Program> _factory = default!;
		private HttpClient _client = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_factory = new BGSWebApplicationFactory<Program>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_client.Dispose();
			_factory.Dispose();
		}

		[TestMethod]
		public async Task ApiHasRegisterEndPointThatRequersRegistrationData()
		{
			_client = _factory.CreateClient();
			var registrationData = new RegistrationData { Email = "testEmail@email.com", Password = "testPassword1!", UserName = "TestName" };
			var jsonContent = JsonContent.Create(registrationData);

			var response = await _client.PostAsync("/register", jsonContent);

			response.EnsureSuccessStatusCode();
		}

		[TestMethod]
		public async Task RegisterEndPointReturn4400ForBadData()
		{
			_client = _factory.CreateClient();
			var registrationData = new RegistrationData { Email = "testEmail@email.com", Password = "testPassword", UserName = "TestName" };
			var jsonContent = JsonContent.Create(registrationData);

			var response = await _client.PostAsync("/register", jsonContent);

			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
		}

		[TestMethod]
		public async Task ApiHasSignInEndPointThatRequersRegistrationData()
		{
			_client = _factory.CreateClient();
			var signInData = new SignInData { UserName = "TestName", Password = "testPassword" };
			var jsonContent = JsonContent.Create(signInData);

			var response = await _client.PostAsync("/signin", jsonContent);

			Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.NotFound);
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
