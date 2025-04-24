using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Implements;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Implements;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


// Register DI for Connection String
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // Generic Repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>(); // Customer Repository
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>(); // Shipment Repository
builder.Services.AddScoped<IShipmentStatusHistoryRepository, ShipmentStatusHistoryRepository>(); // ShipmentStatusHistory Repository
builder.Services.AddScoped<IUserRepository, UserRepository>(); // User Repository

// Add Services
builder.Services.AddScoped<ICustomerService, CustomerService>(); // Customer Service
builder.Services.AddScoped<IShipmentService, ShipmentService>(); // Shipment Service
builder.Services.AddScoped<IShipmentStatusHistoryService, ShipmentStatusHistoryService>(); // ShipmentStatusHistory Service
builder.Services.AddScoped<IUserService, UserService>(); // User Service
builder.Services.AddScoped<IFileService, FileService>(); // File Service
builder.Services.AddScoped<IEmailService, EmailService>(); // Email Service
builder.Services.AddScoped<IAuthService, AuthService>(); // Auth Service

// Add AutoMapper (if you're using it)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "Roles",
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization(options =>
{
    // ÓíÇÓÉ ÇáÜ Admin
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));

    // ÓíÇÓÉ ÇáÜ SuperAdmin
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));

    // ÓíÇÓÉ ÇáÜ Employee
    options.AddPolicy("Employee", policy =>
        policy.RequireRole("Employee"));

    // ÓíÇÓÉ ÇáÜ Driver
    options.AddPolicy("Driver", policy =>
        policy.RequireRole("Driver"));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseHttpsRedirection();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();
app.Run();
