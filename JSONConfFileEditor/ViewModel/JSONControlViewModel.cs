using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
using JSONConfFileEditor.ConfModels;
using JSONConfFileEditor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JSONConfFileEditor.ViewModel
{
    public class JSONControlViewModel : INotifyPropertyChanged
    {
        private ValidationState validationState;
        public ValidationState ValidationState
        {
            get { return validationState; }
            set
            {
                if (value != validationState)
                {
                    validationState = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string nonValidClassMessage;
        public string NonValidClassMessage
        {
            get { return nonValidClassMessage; }
            set
            {
                if (value != nonValidClassMessage)
                {
                    nonValidClassMessage = value;
                    NotifyPropertyChanged();
                }
            }
        }



        private JSONConfigurationEditor jSONConfigurationEditor;
        public JSONConfigurationEditor JSONConfigurationEditor
        {
            get { return jSONConfigurationEditor; }
        }

        public RelayCommand SaveConfigurationCommand { private set; get; }

        public RelayCommand ValidateJSONClassCommand { private set; get; }

        /// <summary>
        /// Saves serialized config object to a file
        /// </summary>
        private void ExecuteSaveConfigurationCommand(object obj)
        {
            var myConfiguration = jSONConfigurationEditor.GetConfiguredClass();
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JSONConf.txt"), JsonConvert.SerializeObject(myConfiguration, Formatting.Indented));
        }

        /// <summary>
        /// Saves serialized config object to a file
        /// </summary>
        private void ExecuteValidateJSONClassCommand(object obj)
        {
            //var myConfiguration = jSONConfigurationEditor.GetConfiguredClass();
            jSONConfigurationEditor.ValidateConfiguratioClass();

            ValidationState = jSONConfigurationEditor.GetValidationState();
            NonValidClassMessage = jSONConfigurationEditor.GetNonValidMessage();
        }

        public JSONControlViewModel()
        {
            //var configurationFile = new CarbideSIModel();
            var configurationFile = new MyCustomConfigurationClass2();
            //var configurationFile = new ValidationTest();

            jSONConfigurationEditor = new JSONConfigurationEditor(configurationFile);

            SaveConfigurationCommand = new RelayCommand(ExecuteSaveConfigurationCommand);
            ValidateJSONClassCommand = new RelayCommand(ExecuteValidateJSONClassCommand);
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
