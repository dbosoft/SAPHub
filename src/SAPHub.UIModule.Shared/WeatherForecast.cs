using System;

namespace SAPHub.UI.Shared
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public class Company
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
