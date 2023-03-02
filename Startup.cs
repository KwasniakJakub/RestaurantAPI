using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using RestaurantAPI.Services;

namespace RestaurantAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationSettingsJwt = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettingsJwt);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false; //Nie wymuszamy od klienta 
                cfg.SaveToken = true; //Dany token powinien zostaæ zapisany po stronie serwera
                cfg.TokenValidationParameters = new TokenValidationParameters //parametry walidacji
                {
                    ValidIssuer = authenticationSettingsJwt.JwtIssuer,   //jwtissuer - wydawca tokenu
                    ValidAudience = authenticationSettingsJwt.JwtIssuer, //jakie podmioty mog¹ u¿ywaæ tokenu
                    IssuerSigningKey = new SymmetricSecurityKey(         //klucz prywatny 
                        Encoding.UTF8.GetBytes(authenticationSettingsJwt.JwtKey)),
                };
            });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => 
                    builder.RequireClaim("Nationality", "German", "Polish"));
                options.AddPolicy("Atleast20", builder =>
                    builder.AddRequirements(new MinimumAgeRequirement(20)));
                options.AddPolicy("CreatedAtleast2Restaurants", builder =>
                    builder.AddRequirements(new CreatedMultipleRestaurantRequirement(2)));
            });

            services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantRequirementHandler>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddSingleton(authenticationSettingsJwt);
            services.AddControllers().AddFluentValidation();
            services.AddControllers();
            services.AddScoped<RestaurantSeeder>();
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<RestaurantQuery>,RestaurantQueryValidator>();
            services.AddSwaggerGen();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddScoped<IDishService,DishService>();
            services.AddCors(options =>
            {
                options.AddPolicy("FrontendClient", builder => 
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(Configuration["AllowedOrigins"])
                    );
            });
            services.AddDbContext<RestaurantDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RestaurantDbConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
        {
            app.UseResponseCaching();
            app.UseStaticFiles(); 
            app.UseCors("FrontendClient");
            seeder.Seed();

            if (env.IsDevelopment())
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
        }
    }
}
