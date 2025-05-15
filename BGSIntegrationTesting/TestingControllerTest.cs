using BGSIntegrationTesting;
using System.Net.Http.Json;

namespace BGSIntegrationTesting
{
	[TestClass]
	public class TestingControllerTest
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
		public async Task CleanDbCleansAllUsersTablesAfterUserWasAdded()
		{
			_client = _factory.CreateClient();
			const string email = "testEmail@email.com";
			const string password = "testPassword1!";
			const string userName = "TestName";
			var registerJsonContent = JsonContent.Create(new RegistrationData { Email = email, Password = password, UserName = userName });
			await _client.PostAsync("/register", registerJsonContent);

			await _client.DeleteAsync("/testing/cleandb");

			var response = await _client.PostAsync("/signin", JsonContent.Create(new SignInData { UserName = userName, Password = password }));
			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
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
}
