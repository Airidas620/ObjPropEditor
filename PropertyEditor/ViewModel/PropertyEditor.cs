using VisualPropertyEditor.Models;
using System;
using System.Collections.ObjectModel;
using static VisualPropertyEditor.Models.PropertyDescriptionBuilder;
using VisualPropertyEditor.Abstractions.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VisualPropertyEditor.ViewModel
{
    public class PropertyEditor : INotifyPropertyChanged
    {

        private Object ConfigurationClass { get; set; }

        private ObservableCollection<PropertyDescription> allAvailableProperties = new ObservableCollection<PropertyDescription>() { new PropertyDescription() { GeneralProperty = PossibleTypes.String, ValueAsString = "sfdsdfs"} }; 

        public ObservableCollection<PropertyDescription> AllAvailableProperties
        {

            get { return allAvailableProperties; }
            private set
            {
                if (allAvailableProperties != value)
                {
                    allAvailableProperties = value;
                    NotifyPropertyChanged();
                }
            }
        }


        PropertyDescriptionBuilder propertyDescriptionBuilder;

        public string GetNonValidMessage()
        {
            return propertyDescriptionBuilder.NonValidClassMessage;
        }

        public bool ValidateConfiguratioClass()
        {
            return propertyDescriptionBuilder.ValidateClass(ConfigurationClass.GetType());
        }

        public bool IsConfigurationFileValid { get; private set; }



        public  PropertyEditor(Object configurationClass)
        {
            ConfigurationClass = configurationClass;

            propertyDescriptionBuilder = new PropertyDescriptionBuilder(ConfigurationClass);

            IsConfigurationFileValid = ValidateConfiguratioClass();

            if (propertyDescriptionBuilder.BuildProperties())
            {
                AllAvailableProperties = propertyDescriptionBuilder.AllAvailableProperties;
            }
        }


        public Object GetWrittenConfiguredClass()
        {

            if (propertyDescriptionBuilder.ValidateValues(allAvailableProperties))
            {
                if (allAvailableProperties != null && allAvailableProperties.Count != 0)
                    PropertyDescriptionBuilder.SetObjectValuesWithPropertyDescription(ConfigurationClass, allAvailableProperties);
            }

            return ConfigurationClass;

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
