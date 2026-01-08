using APIFCG.Infra.LogAPI;
using APIFCG.Infra.Services;
using APIFCG.Service;

namespace APIFCG.Configuracao
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {   
            services.AddHttpContextAccessor();
            services.AddCorrelationIdGenerator();
            services.AddTransient(typeof(BaseLogger<>));

            #region Services/Repository   
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IJogoService, JogoService>();
            // Producer de Service Bus
            services.AddScoped<IJogoProducer, ServiceBusProducer>();
            #endregion

            return services;
        }
    }
}
