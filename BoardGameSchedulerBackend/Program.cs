using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.Infrastructure;
using BoardGameSchedulerBackend.InfrastructureLayer;
using BoardGameSchedulerBackend.InfrastructureLayer.Decouple;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
	options.Filters.Add<CleanDbLockFilter>();
})
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
	});

builder.Services.AddHostedService<MigrationHostedService>();

var allowFrontendPolicy = "AllowFrontend";
builder.Services.AddCors(option =>
{
	option.AddPolicy(allowFrontendPolicy, builder =>
	{
		builder.WithOrigins("http://localhost:3000") //Frontend URL
			   .AllowAnyMethod()
			   .AllowAnyHeader()
			   .AllowCredentials();
	});
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsEnvironment("Test"))
{
	builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

	builder.Services.AddCustomIdentity();
}

builder.Services.AddScoped<IUserRepository, IdentityUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDbManager, DbManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allowFrontendPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
