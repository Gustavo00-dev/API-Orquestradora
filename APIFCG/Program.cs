using APIFCG.Configuracao;
using APIFCG.Infra.Middleware;
using APIFCG.Infra.Model;
using APIFCG.Infra.LogAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Azure.Messaging.ServiceBus;
using APIFCG.Infra.Services; // IJogoProducer / ServiceBusProducer

namespace APIFCG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configurations = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                // Configura Swagger UI
                c.SwaggerDoc("v1", new() { Title = "APIFCG", Version = "v1" });

                //Documenta��o XML para os controllers e modelos
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Define esquema de seguran�a JWT
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {seu token}"
                });

                // Exige JWT para endpoints protegidos
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            //Dependencies
            builder.Services.ResolveDependencies();

            // Service Bus (produtor)
            var sbConnection = builder.Configuration.GetValue<string>("ServiceBus:ConnectionString");
            var sbQueue = builder.Configuration.GetValue<string>("ServiceBus:QueueName");
            if (!string.IsNullOrEmpty(sbConnection) && !string.IsNullOrEmpty(sbQueue))
            {
                // Registra cliente singleton do Service Bus
                builder.Services.AddSingleton(new ServiceBusClient(sbConnection));
            }

            // Elastic logging
            builder.Services.AddElasticLogging(builder.Configuration);

            #region Configura JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettings);

            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
            });
            #endregion

            var app = builder.Build();
            app.UsePathBase("/apifcg");
            app.UseSwagger(c =>
           {
               c.PreSerializeFilters.Add((swagger, httpReq) =>
               {
                   swagger.Servers = new List<OpenApiServer>
                   {
                        new OpenApiServer
                        {
                            Url = "/apifcg"
                        }
                   };
               });
           });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/apifcg/swagger/v1/swagger.json", "APIFCG v1");
                c.RoutePrefix = "swagger";
            });


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.MapControllers();


            #region [Middler]
            app.UseCorrelationMiddleware();
            #endregion

            app.Run();
        }
    }
}
