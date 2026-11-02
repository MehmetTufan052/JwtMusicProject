using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Entities;
using JwtMusicNight.WebApi.Services.AdminServices;
using JwtMusicNight.WebApi.Services.ArtistServices;
using JwtMusicNight.WebApi.Services.GenreServices;
using JwtMusicNight.WebApi.Services.LoginServices;
using JwtMusicNight.WebApi.Services.PlaylistServices;
using JwtMusicNight.WebApi.Services.RegisterServices;
using JwtMusicNight.WebApi.Services.SongServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection bulunamadı. WebApi projesi için User Secrets değerini tanımlayın.");
var jwtKey = builder.Configuration["JwtSettings:Key"]
    ?? throw new InvalidOperationException(
        "JwtSettings:Key bulunamadı. WebApi projesi için User Secrets değerini tanımlayın.");

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<JwtContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});


// Add services to the container.
builder.Services.AddScoped<IArtistService,ArtistService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddDbContext<JwtContext>(options =>
    options.UseSqlServer(connectionString));



builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
