using System.Text;
using FishSupplyChain;
using FishSupplyChain.Data;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Temporarily unconfigure the 20 seconds request timeout for debugging purposes.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
});

// Add services to the container.

// JWT Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // disable in production
    options.SaveToken = true; // hmmmm
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT-Config:Key"])),
        ValidIssuer = builder.Configuration["JWT-Config:Issuer"],
        ValidAudience = builder.Configuration["JWT-Config:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

// Database Services
builder.Services.AddDbContext<FishSupplyChainDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FishSupplyChainDatabase"));
});

// Blockchain configuration settings
builder.Services.Configure<BlockchainSettings>(
    builder.Configuration.GetSection("BlockchainSettings"));
builder.Services.AddSingleton<FishSupplyChainContractService>();

// Helper Services
builder.Services.AddScoped<PasswordHandlerService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<FishSupplyChainDbContext>();
    var passwordHandler = services.GetRequiredService<PasswordHandlerService>();
    await PopulateDb.PopulateAsync(dbContext, passwordHandler);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
