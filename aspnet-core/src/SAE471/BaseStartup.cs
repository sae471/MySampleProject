
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SAE471.Application.Services;
using SAE471.Domain.Repositories;
using SAE471.Domain.Services;

namespace SAE471
{
    public static class BaseStartup
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var loadedAssemblies = new List<Assembly>();
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory,"*.dll").ToList();
            referencedPaths.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddHttpContextAccessor();
            services.AddAutoMapper(loadedAssemblies);


            foreach (Assembly assembly in loadedAssemblies)
            {
                assembly
                .GetTypes()
                .Where(item => item.IsClass && item.IsSubclassOf(typeof(DbContext)))
                .ToList()
                .ForEach(appDbContext =>
                {
                    services.AddScoped(typeof(DbContext), appDbContext);
                });

                assembly
                .GetTypes()
                .Where(item => item.GetInterfaces()
                .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(IDomainService<>)) && !item.IsAbstract && !item.IsInterface)
                .ToList()
                .ForEach(assignedTypes =>
                {
                    var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IDomainService<>));
                    services.AddScoped(serviceType, assignedTypes);
                });

                assembly
                .GetTypes()
                .Where(item => item.GetInterfaces()
                .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(IRepository<>)) && !item.IsAbstract && !item.IsInterface)
                .ToList()
                .ForEach(assignedTypes =>
                {
                    var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IRepository<>));
                    services.AddScoped(serviceType, assignedTypes);
                });


                assembly
                .GetTypes()
                .Where(item => item.GetInterfaces()
                .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(IBaseAppService<,,>)) && !item.IsAbstract && !item.IsInterface)
                .ToList()
                .ForEach(assignedTypes =>
                {
                    var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IBaseAppService<,,>));
                    services.AddScoped(serviceType, assignedTypes);
                });

            }
        }

        public static void InitializeApplication(this IApplicationBuilder app)
        {
            // app.UseMiddleware<ExceptionMiddleware>();
        }

    }
}