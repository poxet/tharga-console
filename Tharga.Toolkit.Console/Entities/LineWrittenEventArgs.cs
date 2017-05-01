using System;

namespace Tharga.Toolkit.Console.Entities
{
    internal class LineWrittenEventArgs : EventArgs
    {
        public LineWrittenEventArgs(string value, OutputLevel level)
        {
            Value = value;
            Level = level;
        }

        public string Value { get; }
        public OutputLevel Level { get; }
    }
}