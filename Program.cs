using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using RestaurantAPI.Services;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using RestaurantAPI;


var builder = WebApplication.CreateBuilder();
//NLog: Setup Nlog for Dependency Injection
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

//configure builder.Services

var authenticationSettingsJwt = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettingsJwt);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false; //Nie wymuszamy od klienta 
    cfg.SaveToken = true; //Dany token powinien zosta� zapisany po stronie serwera
    cfg.TokenValidationParameters = new TokenValidationParameters //parametry walidacji
    {
        ValidIssuer = authenticationSettingsJwt.JwtIssuer,   //jwtissuer - wydawca tokenu
        ValidAudience = authenticationSettingsJwt.JwtIssuer, //jakie podmioty mog� u�ywa� tokenu
        IssuerSigningKey = new SymmetricSecurityKey(         //klucz prywatny 
            Encoding.UTF8.GetBytes(authenticationSettingsJwt.JwtKey)),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasNationality", builder =>
        builder.RequireClaim("Nationality", "German", "Polish"));
    options.AddPolicy("Atleast20", builder =>
        builder.AddRequirements(new MinimumAgeRequirement(20)));
    options.AddPolicy("CreatedAtleast2Restaurants", builder =>
        builder.AddRequirements(new CreatedMultipleRestaurantRequirement(2)));
});

builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantRequirementHandler>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddSingleton(authenticationSettingsJwt);
builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddControllers();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policyBuilder =>
        policyBuilder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(builder.Configuration["AllowedOrigins"])
        );
});
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection")));

// configure

var app = builder.Build();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();

app.UseResponseCaching();
app.UseStaticFiles();
app.UseCors("FrontendClient");
seeder.Seed();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
});
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();