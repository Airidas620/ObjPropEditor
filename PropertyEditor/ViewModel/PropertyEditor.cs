using VisualPropertyEditor.Models;
using System;
using System.Collections.ObjectModel;
using static VisualPropertyEditor.Models.PropertyDescriptionBuilder;
using VisualPropertyEditor.Abstractions.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VisualPropertyEditor.Abstractions;
using VisualPropertyEditor.Abstractions.Classes;
using System.Diagnostics;

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

        public bool IsConfigurationClassValid { get; private set; }

        public string ClassDescription { get; private set; } = "";
        public bool HasDescription { get; }

        public  PropertyEditor(Object configurationClass)
        {
            ConfigurationClass = configurationClass;


            var descriptionAttributes = ConfigurationClass.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false);
            
            if (descriptionAttributes != null && descriptionAttributes.Length > 0)
            {
                ClassDescription = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                HasDescription = true;
            }

            propertyDescriptionBuilder = new PropertyDescriptionBuilder(ConfigurationClass);

            IsConfigurationClassValid = propertyDescriptionBuilder.ValidateClass(ConfigurationClass.GetType());

            AllAvailableProperties = propertyDescriptionBuilder.BuildProperties();            
        }


        public Object GetWrittenConfiguredClass()
        {
            if (allAvailableProperties != null && allAvailableProperties.Count != 0)
            {
                PropertyDescriptionHelper.SetObjectValuesWithPropertyDescription(ConfigurationClass, allAvailableProperties);
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
