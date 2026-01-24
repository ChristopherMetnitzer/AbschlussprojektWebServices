using System;

namespace Abschlussprojekt.Services
{
    // Beispiel für einen externen, instabilen Service
    public class ExternalUnstableService
    {
        private static readonly Random _random = new Random();

        public async Task<string> CallAsync()
        {
            // Simuliere Instabilität: 50% Chance auf Fehler
            if (_random.NextDouble() < 0.5)
            {
                throw new HttpRequestException("External service failure!");
            }

            return "External service data";
        }
    }
}