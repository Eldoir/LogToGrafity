using System.Collections.Generic;

namespace LogToGrafity
{
    public struct FileResult
    {
        public IEnumerable<string> ColumnNames;
        /// <summary>
        /// Key is column name.
        /// </summary>
        public Dictionary<string, string> Rows;
    }

    public interface IRowAnalyzer
    {
        FileResult Analyze(IEnumerable<Row> rows);
    }

    public class RowAnalyzer : IRowAnalyzer
    {
        public FileResult Analyze(IEnumerable<Row> rows)
        {
            return new FileResult();
        }
    }
}
