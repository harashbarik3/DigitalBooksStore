using UserServiceLib;
using BookService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Builder;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddTransient<IBookRepository,BookRepository>();
builder.Services.AddTransient<ICategoryRepository,CategoryRepository>();
builder.Services.AddTransient<IPublisherRepository, PublisherRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenHandler, BookService.Repositories.TokenHandler>();
builder.Services.AddDbContext<DigitalbookstoreContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreDBConnection")));
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

//    }) ;

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(option =>
//    {
//        option.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Aud"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
               AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration["jwt:Issuer"],
                       ValidAudience = builder.Configuration["jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"]))
                   };
               });

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