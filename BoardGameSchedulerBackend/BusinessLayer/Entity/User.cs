namespace BoardGameSchedulerBackend.BusinessLayer.Entity
{
	public class User
	{
		public Guid Id { get; set; }
		public required string UserName { get; set; }
		public required string Email { get; set; }
	}
}
