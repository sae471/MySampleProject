
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;


namespace SAE471
{
    public static class AppConfiguration
    {
        public static IConfiguration Configuration
        {
            get
            {
                try
                {
                    return LazyServiceProvider.LazyGetService<IConfiguration>();
                }
                catch (System.Exception)
                {

                    var appSetting = Debugger.IsAttached ? "appsettings.Development.json" : "appsettings.json";
                    return new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile(appSetting, optional: false, reloadOnChange: true).Build();
                }
            }
        }
    }
}