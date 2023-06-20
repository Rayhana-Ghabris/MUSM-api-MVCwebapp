using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Helpers;
using MUSM_api_MVCwebapp.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the connection string from appsettings.json file
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    //options.SignIn.RequireConfirmedAccount = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Get strongly typed settings from "appsettings.json"

var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

//creating a variable holding all the settings from appSettingsSection

var appSettings = appSettingsSection.Get<AppSettings>();

//Get the key for the JWT Token

var key = Encoding.ASCII.GetBytes(appSettings.Secret);

//Add Authentication Service: Configure how the server will validate the JWT recieved in HTTP request header

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = appSettings.Site,
        ValidAudience = appSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = false
    };
});

//Policies Configuration

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLoggedIn", policy => policy.RequireRole("Manager", "Worker","PublicUser").RequireAuthenticatedUser());
    options.AddPolicy("RequireManagerOrPublicUserRole", policy => policy.RequireRole("Manager","PublicUser").RequireAuthenticatedUser());
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager").RequireAuthenticatedUser());
    options.AddPolicy("RequireWorkerRole", policy => policy.RequireRole("Worker").RequireAuthenticatedUser());
    options.AddPolicy("RequirePublicUserRole", policy => policy.RequireRole("PublicUser").RequireAuthenticatedUser());
});

builder.Services.AddControllersWithViews();

// Add Mapper service
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new AutoMapperProfiles());
});

IMapper mapper = mappingConfig.CreateMapper();

builder.Services.AddSingleton(mapper);

builder.Services.AddTransient<VotesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();

