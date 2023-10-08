using System.Collections.Generic;

namespace LogToGrafity
{
    public interface IRowAnalyzer
    {
        /// <summary>
        /// Returns a <see cref="EpsContainer"/> for eps1, and one for eps2.
        /// </summary>
        (EpsContainer, EpsContainer) Analyze(IEnumerable<Row> rows);
    }

    public class RowAnalyzer : IRowAnalyzer
    {
        public (EpsContainer, EpsContainer) Analyze(IEnumerable<Row> rows)
        {
            IncreaseDecreaseTracker tracker = new();
            EpsContainer eps1 = new();
            EpsContainer eps2 = new();

            foreach (Row row in rows)
            {
                tracker.AddValue((int)row.Temp);
                string columnName = tracker.GetName();
                eps1.AddEntry(row.Freq, (columnName, row.Eps1));
                eps2.AddEntry(row.Freq, (columnName, row.Eps2));
            }

            return (eps1, eps2);
        }
    }
}
