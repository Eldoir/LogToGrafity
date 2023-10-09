using System.Globalization;

namespace LogToGrafity
{
    public record Row(string Freq, double Temp, string Eps1, string Eps2);

    public interface IRowParser
    {
        Result<Row> Parse(string row);
    }

    public class RowParser : IRowParser
    {
        public Result<Row> Parse(string row)
        {
            string[] values = row.Trim().Split('\t');
            if (values.Length < 8)
                return Result.Fail<Row>("Row needs at least 8 values since, eps1 and eps2 are the 7th and 8th values");

            string tempStr = values[1];
            if (!double.TryParse(tempStr, CultureInfo.InvariantCulture, out double temp))
                return Result.Fail<Row>($"Unknown temperature: {tempStr}");

            string freq = values[0];
            string eps1 = values[6];
            string eps2 = values[7];
            return Result.Ok(new Row(freq, temp, eps1, eps2));
        }
    }
}
