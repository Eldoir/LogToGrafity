using System;
using System.Linq;
using System.Text;

namespace LogToGrafity
{
    public interface IEpsContainerStringConverter
    {
        string ToString(EpsContainer container);
    }

    public class EpsContainerStringConverterFactory
    {
        public static IEpsContainerStringConverter Create(bool areColumnsTemperature)
        {
            return areColumnsTemperature
                ? new EpsContainerStringColumnTemperatureConverter()
                : new EpsContainerStringColumnFrequencyConverter();
        }
    }

    public sealed class EpsContainerStringColumnTemperatureConverter : IEpsContainerStringConverter
    {
        public string ToString(EpsContainer eps)
        {
            StringBuilder builder = new();

            builder.AppendLine(string.Join(" ", new[] { "Freq" }.Concat(eps.Temperatures)));
            foreach (string freq in eps.Frequencies)
            {
                builder.AppendLine(string.Join(" ", new[] { freq }.Concat(eps.GetValuesByFrequency(freq))));
            }

            return builder.ToString();
        }
    }

    public sealed class EpsContainerStringColumnFrequencyConverter : IEpsContainerStringConverter
    {
        public string ToString(EpsContainer eps)
        {
            StringBuilder builder = new();

            builder.AppendLine(string.Join(" ", new[] { "Temp" }.Concat(eps.Frequencies.Select(freq => $"\"{freq}\""))));
            foreach (string temp in eps.Temperatures)
            {
                builder.AppendLine(string.Join(" ", new[] { ParseIntTemp(temp) }.Concat(eps.GetValuesByTemperature(temp))));
            }

            return builder.ToString();
        }

        private static string ParseIntTemp(string temp)
        {
            int cIndex = temp.IndexOf('C');
            if (cIndex != -1)
                return temp[..cIndex];
            int hIndex = temp.IndexOf('H');
            if (hIndex != -1)
                return temp[..hIndex];

            throw new InvalidOperationException();
        }
    }
}
