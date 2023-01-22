
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SAE471
{
    public static class LazyServiceProvider
    {
        public static IService LazyGetService<IService>() where IService : notnull
        {
            var contextAccessor = new HttpContextAccessor();
            if (contextAccessor.HttpContext != null)
            {
                return contextAccessor.HttpContext.RequestServices.GetService<IService>();
            }
            var services = new ServiceCollection();
            services.AddApplication();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IService>();
        }
    }
}