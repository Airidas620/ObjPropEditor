using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
using JSONConfFileEditor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JSONConfFileEditor.ViewModel
{
    public class JSONControlViewModel
    {

        private JSONConfigurationEditor jSONConfigurationEditor;
        public JSONConfigurationEditor JSONConfigurationEditor
        {
            get { return jSONConfigurationEditor; }
        }
       

        public RelayCommand SaveConfigurationCommand { private set; get; }


        /// <summary>
        /// Saves serialized config object to file
        /// </summary>
        void SaveConfigToFile(Object serializableClass, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(serializableClass, Formatting.Indented));
        }

        private void ExecuteSaveConfigurationCommand(object obj)
        {
            var myConfiguration = jSONConfigurationEditor.GetConfiguredClass();
            SaveConfigToFile(myConfiguration, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JSONConf.txt"));
        }

        public JSONControlViewModel()
        {
            //var configurationFile = new CarbideSIModel();
            var configurationFile = new MyCustomConfigurationClass2();

            jSONConfigurationEditor = new JSONConfigurationEditor(configurationFile);         

            SaveConfigurationCommand = new RelayCommand(ExecuteSaveConfigurationCommand);
        }

       
    }
}
