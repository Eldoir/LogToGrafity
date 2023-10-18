using System.Linq;

namespace LogToGrafity
{
    public record FileFormat(string Description, string Extension);

    public class FileFormatContainer
    {
        public FileFormatContainer(FileFormat[] formats)
        {
            _formats = formats;
        }

        public bool Supports(string filePath)
        {
            return _formats.Any(f => filePath.EndsWith(f.Extension));
        }

        public string ToFilter()
        {
            return string.Join("|", _formats.Select(f => $"{f.Description} (*{f.Extension})|*{f.Extension}"));
        }

        private readonly FileFormat[] _formats;
    }
}
