using System.Reflection;

namespace DiningPlanBot;

public static class StartupExtensions
{
    public static IServiceCollection AddAllImplementationsAsSingletons<TInterface>(this IServiceCollection services)
    {
        var implementations = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => !x.IsAbstract && x.IsClass && typeof(TInterface).IsAssignableFrom(x));

        foreach (var imlementation in implementations)
        {
            services.Add(new ServiceDescriptor(typeof(TInterface), imlementation, ServiceLifetime.Singleton));
        }
        
        return services;
    }
}