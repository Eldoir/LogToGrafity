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

            int eps1Idx, eps2Idx;

            if (values.Length == 4)
            {
                eps1Idx = 2;
                eps2Idx = 3;
            }
            else if (values.Length >= 8)
            {
                eps1Idx = 6;
                eps2Idx = 7;
            }
            else
                return Result.Fail<Row>("Row is expecting either 4 values, or at least 8 values");

            string tempStr = values[1];
            if (!double.TryParse(tempStr, CultureInfo.InvariantCulture, out double temp))
                return Result.Fail<Row>($"Unknown temperature: {tempStr}");

            string freq = values[0];
            string eps1 = values[eps1Idx];
            string eps2 = values[eps2Idx];
            return Result.Ok(new Row(freq, temp, eps1, eps2));
        }
    }
}
