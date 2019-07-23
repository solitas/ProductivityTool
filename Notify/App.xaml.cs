using System;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        AppContextService _service;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // file read
            var contexts = ReadConfigFile();
            _service = new AppContextService(contexts);

            ConfigurationView window = new ConfigurationView(_service);
            window.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            SaveConfigFile();
        }
        private ExeAppContextCollection ReadConfigFile()
        {
            const string appConfigFile = "App.Configs";

            if (!File.Exists(appConfigFile))
            {
                var tmp = new ExeAppContextCollection();

                var xDoc = new XDocument();
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(ExeAppContextCollection));
                    using (var writer = new StreamWriter(appConfigFile))
                    {
                        ser.Serialize(writer, tmp);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("AppConfig file create failure: {0}", e.Message);
                }

                return tmp;
            }
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ExeAppContextCollection));
                using (var reader = new StreamReader(appConfigFile))
                {
                    return (ExeAppContextCollection)ser.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("AppConfig file load failure: {0}", e.Message);
                return null;
            }
        }
        private void SaveConfigFile()
        {
            const string appConfigFile = "App.Configs";

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ExeAppContextCollection));
                using (var writer = new StreamWriter(appConfigFile))
                {
                    ser.Serialize(writer, _service.Contexts);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("AppConfig file create failure: {0}", e.Message);
            }
        }
    }
}
