using JSONConfFileEditor.ConfModels;
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
using System.Windows.Input;
using VisualPropertyEditor.ViewModel;
using JSONConfFileEditor.Abstractions.Classes;

namespace JSONConfFileEditor.ViewModel
{
    public class JSONControlViewModel : INotifyPropertyChanged
    {
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


        private PropertyEditor propertyEditorInstance;
        public PropertyEditor PropertyEditorInstance
        {
            get { return propertyEditorInstance; }
            set
            {
                if (value != propertyEditorInstance)
                {
                    propertyEditorInstance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RelayCommand SaveConfigurationCommand { private set; get; }


        /// <summary>
        /// Saves serialized config object to a file
        /// </summary>
        private void ExecuteSaveConfigurationCommand(object obj)
        {

            var myConfiguration = propertyEditorInstance.GetWrittenConfiguredClass();

            NonValidClassMessage = propertyEditorInstance.GetNonValidMessage();

            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JSONConf.txt"), JsonConvert.SerializeObject(myConfiguration, Formatting.Indented));
        }


        public JSONControlViewModel()
        {
            //var configurationFile = new CarbideSIModel();
            var configurationFile = new MyCustomConfigurationClass1();
            //var configurationFile = new MyCustomConfigurationClass2();
            //var configurationFile = new ValidationTest();


            PropertyEditorInstance = new PropertyEditor(configurationFile);

            //PropertyEditorInstance.(configurationFile);

            //propertyEditorInstance = new PropertyEditor(configurationFile);

            SaveConfigurationCommand = new RelayCommand(ExecuteSaveConfigurationCommand);
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
