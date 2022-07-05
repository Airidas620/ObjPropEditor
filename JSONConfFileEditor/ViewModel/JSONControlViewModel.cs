using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
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

        MyCustomConfigurationClass myCustomConfigurationClass;

        public RelayCommand SaveConfigurationCommand { private set; get; }


        /// <summary>
        /// Saves serialized config object to file
        /// </summary>
        void SaveConfigToFile(MyCustomConfigurationClass myCustomConfigurationClass, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(jSONConfigurationEditor.MyCustomConfigurationClass, Formatting.Indented));
        }

        private void ExecuteSaveConfigurationCommand(object obj)
        {

            jSONConfigurationEditor.SetUpConfigurationClass();

            SaveConfigToFile(jSONConfigurationEditor.MyCustomConfigurationClass, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JSONConf.txt"));
        }

        public JSONControlViewModel()
        {
            myCustomConfigurationClass = new MyCustomConfigurationClass();

            jSONConfigurationEditor = new JSONConfigurationEditor(myCustomConfigurationClass);

            SaveConfigurationCommand = new RelayCommand(ExecuteSaveConfigurationCommand);
        }

        #region JSON object
        /// <summary>
        /// Class for JSON config file 
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
      
        public class MyCustomConfigurationClass
        {
           
            [JsonProperty]
            public bool IsFeedbackEnabled { get; set; }

            [JsonProperty]
            public bool IsFeedbackEnabled2 { get; set; }

            [JsonProperty]
            public bool IsFeedbackEnabled3 { get; set; }

            [JsonProperty]
            public bool IsFeedbackEnabled4 { get; set; }

            [JsonProperty]
            public bool IsFeedbackEnabled5 { get; set; }

            [JsonProperty]
            public double GetLastFeedbackValue { get; set; }

            [JsonProperty]
            public double GetLastFeedbackValue2 { get; set; }

            [JsonProperty]
            public string FeedbackTitle { get; set; }

            [JsonProperty]
            public string FeedbackTitle2 { get; set; }

            [JsonProperty]
            public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism { get; set; }

            [JsonProperty]
            public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism2 { get; set; }

            [JsonProperty]
            public FeedbackMechanismGroupTwoEnum TypeOfFeedbackMechanism3 { get; set; }

        }
        #endregion
    }
}
