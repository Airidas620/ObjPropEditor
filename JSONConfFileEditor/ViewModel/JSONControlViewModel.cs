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

        MyCustomConfigurationClass2 myCustomConfigurationClass;

        public RelayCommand SaveConfigurationCommand { private set; get; }


        /// <summary>
        /// Saves serialized config object to file
        /// </summary>
        void SaveConfigToFile(Object myCustomConfigurationClass, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(jSONConfigurationEditor.MyCustomConfigurationClass, Formatting.Indented));
        }

        private void ExecuteSaveConfigurationCommand(object obj)
        {

            jSONConfigurationEditor.SetUpConfigurationClass2();

            SaveConfigToFile(jSONConfigurationEditor.MyCustomConfigurationClass, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JSONConf.txt"));
        }

        public JSONControlViewModel()
        {
            myCustomConfigurationClass = new MyCustomConfigurationClass2();

            jSONConfigurationEditor = new JSONConfigurationEditor(myCustomConfigurationClass);

            MyCustomConfigurationClass2 myCustomConfigurationClass2 = new MyCustomConfigurationClass2();

            //Console.Write(JsonConvert.SerializeObject(myCustomConfigurationClass2, Formatting.Indented));

            SaveConfigurationCommand = new RelayCommand(ExecuteSaveConfigurationCommand);
        }

       
    }
}
