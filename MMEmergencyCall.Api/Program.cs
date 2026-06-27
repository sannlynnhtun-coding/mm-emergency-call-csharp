using Microsoft.AspNetCore.Mvc;
using MMEmergencyCall.Databases.Dapper;
using MMEmergencyCall.Domain.Admin;
using MMEmergencyCall.Domain.Client.Features.Profile;
using MMEmergencyCall.Shared;

var builder = WebApplication.CreateBuilder(args);
const string WasmDevCorsPolicy = "WasmDevClient";

// Add services to the container.

builder.Services.AddControllers()
	.ConfigureApiBehaviorOptions(options =>
	{
		options.InvalidModelStateResponseFactory = context =>
		{
			var message = string.Join("; ", context.ModelState.Values
				.SelectMany(x => x.Errors)
				.Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage) ? "Invalid request." : x.ErrorMessage));

			return new BadRequestObjectResult(
				Result<object?>.ValidationError(string.IsNullOrWhiteSpace(message) ? "Invalid request." : message));
		};
	});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
	options.AddPolicy(WasmDevCorsPolicy, policy =>
	{
		policy
			.WithOrigins("https://localhost:7180", "http://localhost:5180")
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
	opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
	opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
},
ServiceLifetime.Transient,
ServiceLifetime.Transient);

builder.AddDapperContext();

builder.AddAdminServices();

builder.AddRegisterService();
builder.AddEmergencyServiceService();
builder.AddEmergencyServiceType();
builder.AddEmergencyRequest();
builder.AddSigninService();
builder.AddProfile();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		context.Response.StatusCode = StatusCodes.Status500InternalServerError;
		context.Response.ContentType = "application/json";
		await context.Response.WriteAsJsonAsync(Result<object?>.SystemError("Internal server error"));
	});
});

app.UseHttpsRedirection();

app.UseCors(WasmDevCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();

