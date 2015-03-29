using System.IO;
using System.Xml.Serialization;

namespace Utility
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        public Configuration()
        {
            ProducerConfiguration = new ConfigurationParameters();
            ConsumerConfiguration = new ConfigurationParameters();
        }
        public  static Configuration CreateConfiguration()
        {
            var serializer = new XmlSerializer(typeof(Configuration));
            var reader = new StreamReader("ConfigFile.xml");
            var configuration = serializer.Deserialize(reader);
            reader.Close();
            return (Configuration)configuration;
        }

        [XmlElement("ProducerConfiguration")]
        public ConfigurationParameters ProducerConfiguration { get; set; }
        [XmlElement("ConsumerConfiguration")]
        public ConfigurationParameters ConsumerConfiguration { get; set; }
        
    }
}
