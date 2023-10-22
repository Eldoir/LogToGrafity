using System;
using System.Collections.Generic;
using System.Linq;

namespace LogToGrafity
{
    public class EpsContainer
    {
        public void AddEntry(string freq, (string ColumnName, string Value) pair)
        {
            if (!_dic.ContainsKey(freq))
                _dic.Add(freq, new List<(string, string)>());

            if (_dic[freq].Contains(pair))
                throw new InvalidOperationException();

            _dic[freq].Add(pair);
        }

        public IEnumerable<string> GetValuesByFrequency(string frequency)
        {
            return _dic[frequency].Select(pair => pair.Value);
        }

        public IEnumerable<string> GetValuesByTemperature(string temp)
        {
            List<string> values = new();
            foreach (string freq in _dic.Keys)
            {
                values.Add(_dic[freq].Single(pair => pair.ColumnName == temp).Value);
            }
            return values;
        }

        /// <summary>
        /// NOTE: temperatures should be the same for every frequency, and be distinct.
        /// </summary>
        public IEnumerable<string> Temperatures => _dic.First().Value.Select(pair => pair.ColumnName);

        public IEnumerable<string> Frequencies => _dic.Keys;

        /// <summary>
        /// Key is freq
        /// </summary>
        private readonly Dictionary<string, List<(string ColumnName, string Value)>> _dic = new();
    }
}
