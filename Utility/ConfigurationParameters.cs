using System;
using System.Xml.Serialization;

namespace Utility
{
    public class ConfigurationParameters
    {
        public int RandomDelay { get { return new Random().Next(MinimumDelay, MaximumDelay); } }
        [XmlElement("NumberOfProcesses")]
        public int NumberOfProcesses { get; set; }
        [XmlElement("AliveTime")]
        public int AliveTime { get; set; }
        [XmlElement("MaximumDelay")]
        public int MaximumDelay { get; set; }
        [XmlElement("MinimumDelay")]
        public int MinimumDelay { get; set; }
    }
}
