global using BasicStructure.Models;
global using BasicStructure.DTOS.FieldDTO;
global using BasicStructure.DTOS.CommentDTO;
global using BasicStructure.DTOS.CoordinateDTO;
global using BasicStructure.DataLayer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using BasicStructure.Services.UserService;
global using BasicStructure.Services.FieldService;
global using BasicStructure.DTOS.UserDTO;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using AutoMapper;
using Services.Models;
using Services.Services;
var builder = WebApplication.CreateBuilder(args);

static string GetRequiredConfiguration(IConfiguration configuration, string key)
{
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Required configuration '{key}' is missing. Set it with an environment variable or .NET User Secrets.");
    }

    return value;
}

var jwtKey = GetRequiredConfiguration(builder.Configuration, "Jwt:Key");
var jwtIssuer = GetRequiredConfiguration(builder.Configuration, "Jwt:Issuer");
var jwtAudience = GetRequiredConfiguration(builder.Configuration, "Jwt:Audience");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Add Config for Required Email
builder.Services.Configure<IdentityOptions>(
    opts => opts.SignIn.RequireConfirmedEmail = true
    );
builder.Services.Configure<DataProtectionTokenProviderOptions>(
    opts => opts.TokenLifespan = TimeSpan.FromMinutes(60)
    );

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>( options =>
    options.Password.RequiredLength = 5
).AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes
                (jwtKey)
                ),
        RequireExpirationTime = true,
    };
});
// Add Email config
builder.Services.AddSingleton(builder.Configuration.GetSection("Smtp")
                  .Get<EmailConfiguration>() ?? new EmailConfiguration());
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<InterfaceUserService, UserService>();
builder.Services.AddScoped<InterfaceFieldService, FieldService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please Insert Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type =  ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[0]{}
        }
    });
});
builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
