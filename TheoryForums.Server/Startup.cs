using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using TheoryForums.Shared.Data;
using TheoryForums.Shared.Helpers;
using TheoryForums.Shared.Models;
using TheoryForums.Shared.Repositories;

namespace TheoryForums.Server
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
            services.AddControllers(x => {
                // Users by default must be authenticated to use controllers
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(x => {
                        // Checks to see if Security Stamp is still good
                        using (var scope = services.BuildServiceProvider().CreateScope())
                        {
                            var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<User>>();
                            return signInManager.ValidateSecurityStampAsync(x.User).Result != null;
                        }
                    })
                    .Build();

                x.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddDbContext<DataContext>(x => {
                x.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("TheoryForums.Server")
                );
            });

            services.AddDbContext<IdentityDataContext>(x => {
                x.UseSqlite(
                    Configuration.GetConnectionString("IdentityConnection"),
                    b => b.MigrationsAssembly("TheoryForums.Server")
                );
            });

            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.ClaimsIdentity.SecurityStampClaimType = "security_stamp";
            })
            .AddRoles<Role>()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserValidator<UserValidator<User>>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                byte[] key = Encoding.ASCII.GetBytes(Configuration["AppSettings:Secret"]);
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateAudience = true,
                    RequireAudience = true,
                    ValidIssuer = Configuration["AppSettings:Issuer"],
                    ValidAudience = Configuration["AppSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            //.AddCookie(IdentityConstants.ApplicationScheme, x => {
            //    x.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
            //    x.Cookie.HttpOnly = true;
            //    x.Cookie.SameSite = SameSiteMode.Strict;
            //})
            //.AddCookie(IdentityConstants.ExternalScheme, x => {
            //    x.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
            //})
            //.AddCookie(IdentityConstants.TwoFactorUserIdScheme, x => { 
            //    x.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
            //})
            //.AddCookie(IdentityConstants.TwoFactorRememberMeScheme, x => { 
            //    x.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
            //});

            services.AddAuthorization(options => {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("StaffOnly", policy => policy.RequireRole("Admin", "Moderator"));
                options.AddPolicy("MemberOnly", policy => policy.RequireRole("Admin", "Moderator", "Member"));
            });

            services
            .AddTransient<IForumsRepository, ForumsRepository>()
            .AddTransient<IUsersRepository, UsersRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(
                    new ExceptionHandlerOptions
                    {
                        ExceptionHandler = async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
            }
            app.UseRouting();
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
