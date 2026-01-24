using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abschlussprojekt.Attributes
{
    // Attribut-Klasse, um API-Key-Validierung zu ermöglichen
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Überprüfen, ob der API-Key im Header vorhanden ist
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "API Key wurde nicht bereitgestellt"
                };
                return;
            }

            // Den erwarteten API-Key aus der Konfiguration (appsettings.json) lesen
            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>("ApiKey");

            // Überprüfen, ob der bereitgestellte Key mit dem konfigurierten Key übereinstimmt
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Unautorisierter Zugriff - API Key ist ungültig"
                };
                return;
            }

            await next();
        }
    }
}
