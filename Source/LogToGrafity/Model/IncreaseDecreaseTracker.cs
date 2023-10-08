namespace LogToGrafity
{
    public class IncreaseDecreaseTracker
    {
        public void AddValue(int value)
        {
            if (_currentValue is null) // first time: init
            {
                if (value >= 0)
                {
                    _increasing = true;
                    _nbIncrease = 1;
                }
                else
                {
                    _increasing = false;
                    _nbDecrease = 1;
                }
            }
            else if (value > _currentValue && !_increasing) // we were decreasing, we are now increasing
            {
                _increasing = true;
                _nbIncrease++;
            }
            else if (value < _currentValue &&_increasing) // we were increasing, we are now decreasing
            {
                _increasing = false;
                _nbDecrease++;
            }

            _currentValue = value;
        }

        public string GetName()
        {
            return $"{_currentValue}{(_increasing ? "H" : "C")}{(_increasing ? _nbIncrease : _nbDecrease)}";
        }

        private bool _increasing = true;
        private int? _currentValue = null;
        private int _nbIncrease = 0;
        private int _nbDecrease = 0;
    }
}
