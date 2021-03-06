﻿using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Infrastructure.Repository;
using RealEstateCore.Core.Repository;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.Services;
using RealEstateCore.Core.Security.Implementation;
using RealEstateCore.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;

namespace RealEstateCore
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => builder.AllowAnyOrigin()
                                                                           .AllowAnyMethod()
                                                                           .AllowAnyHeader()
                                                                           .AllowCredentials());
            });

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetSection("Database:ConnectionString").Value));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = Configuration.GetSection("AppConfiguration:Issuer").Value,
                            ValidAudience = Configuration.GetSection("AppConfiguration:Audience").Value,
                            IssuerSigningKey = JwtSecurityKey.Create(Configuration.GetSection("JwtSettings:SecurityKey").Value)
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                //Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                //Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(
                        options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "RealEstate API", Version = "v1" });
            });

            services.AddScoped<UserIdFilter>();

            services.AddTransient<IResponseModel, ResponseModel>();
            services.AddTransient<IUserModel, UserModel>();
            services.AddTransient<IRepository, Repository>();
            
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IPropertyService, PropertyService>();
            services.AddSingleton<IRenterService, RenterService>();
            services.AddSingleton<IRoomService, RoomService>();
            services.AddSingleton<ITransactionService, TransactionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstate API V1");
            });
        }
    }
}
