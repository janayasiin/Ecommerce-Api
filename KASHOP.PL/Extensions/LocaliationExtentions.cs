using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace KASHOP.PL.Extensions
{
    public static class LocaliationExtentions
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection Services)
        {
            Services.AddLocalization(options => options.ResourcesPath = "");
            const string defaultCulture = "en";
            
            var supportedCultures = new[] {


    new CultureInfo(defaultCulture),
    new CultureInfo("ar") };


            Services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.RequestCultureProviders.Clear();
                    //options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider
                    //{
                    //    QueryStringKey = "lang"
                    //});
                    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());


                });
            return Services;
        }
    }
}
