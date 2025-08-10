using FishSupplyChain;
using FishSupplyChain.Data;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

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

// Configuration settings
builder.Services.Configure<BlockchainSettings>(builder.Configuration.GetSection("BlockchainSettings"));
builder.Services.Configure<HiveMQqtSettings>(builder.Configuration.GetSection("HiveMQConnectionSettings"));

builder.Services.AddSingleton<FishSupplyChainContractService>();

// Helper Services
builder.Services.AddScoped<PasswordHandlerService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();

builder.Services.AddControllers().AddNewtonsoftJson();

// Http Client for IPFS
builder.Services.AddHttpClient<IIpfsService, IpfsService>();

// Documentation services
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add the HiveMQtt service
builder.Services.AddHostedService<HiveMqttService>();

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
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
