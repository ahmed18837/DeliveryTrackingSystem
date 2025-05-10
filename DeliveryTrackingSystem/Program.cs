using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Implements;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Implements;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


// Register DI for Connection String
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor(); // for IHttpContextAccessor


builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
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

builder.Services.AddResponseCaching();

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo { Title = "Your API", Version = "v3" });
    options.UseInlineDefinitionsForEnums();
    // To Run Token In Swagger
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            },
            Scheme = "Auth",
            Name = JwtBearerDefaults.AuthenticationScheme,
            In = ParameterLocation.Header
        },
            new List<string>()
    }
});
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});



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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"))
    .AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"))
    .AddPolicy("Employee", policy =>
        policy.RequireRole("Employee"))
    .AddPolicy("Driver", policy =>
        policy.RequireRole("Driver"));

builder.Services.AddAuthorization();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseRouting();
var versionDescriptionProvider =
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});

app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // íÌÈ Ãä íßæä ÞÈá UseAuthorization
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseCors("AllowAll");
app.UseStaticFiles();

app.Run();
