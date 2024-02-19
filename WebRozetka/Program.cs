using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Interfaces;
using WebRozetka.Interfaces.Repo;
using WebRozetka.Mapper;
using WebRozetka.Repository;
using WebRozetka.Services;

namespace WebRozetka
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Додаємо сервіси.
            builder.Services.AddDbContext<AppEFContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("WebRozetkaConnection")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Додаємо Authorize на Swagger
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            builder.Services.AddSwaggerGen(c =>
            {
                var fileDoc = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
                c.IncludeXmlComments(fileDoc);
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Jwt Auth header using the Bearer scheme",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Id="Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });
            // -- Authorize на Swagger


            builder.Services.AddScoped(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AppMapProfile(provider.GetService<AppEFContext>()));
            }).CreateMapper());



            ////builder.Services.AddCors();
            ////builder.Services.AddCors(options =>
            ////{
            ////    options.AddDefaultPolicy(
            ////        policy =>
            ////        {
            ////            policy
            ////                .AllowAnyOrigin()
            ////                .AllowAnyMethod()
            ////                .AllowAnyHeader();
            ////        });
            ////});


            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowSpecificOrigins",
            //        builder =>
            //        {
            //            builder.WithOrigins("http://localhost:5173")
            //                .AllowAnyHeader()
            //                .AllowAnyMethod();
            //        });
            //});


            //-----------


            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
            builder.Services.AddScoped<INovaPoshtaService, NovaPoshtaService>();



            // IDENTITY SETTINGS
            builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
              .AddEntityFrameworkStores<AppEFContext>()
              .AddDefaultTokenProviders();
            // -- IDENTITY SETTINGS


            // JWT SETTINGS
            #region
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSecretKey")));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters()
                {

                    IssuerSigningKey = signinKey,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });
            #endregion



            var app = builder.Build();

            // Конфігурація Сваггера.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            // Статичні файли.
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(dir),
                RequestPath = "/images"
            });

            //Налаштування CORS
            app.UseCors(options => options
                .WithOrigins(new[] { "http://localhost:5173", "http://localhost:3000", "http://localhost:5135" })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );


            // -- Налаштування CORS

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.SeedData();

            app.Run();
        }
    }
}