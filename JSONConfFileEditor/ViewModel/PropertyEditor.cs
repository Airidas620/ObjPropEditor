using JSONConfFileEditor.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JSONConfFileEditor.ViewModel;
using JSONConfFileEditor.Models;
using System.Collections.ObjectModel;
using static JSONConfFileEditor.Models.PropertyDescriptionBuilder;
using JSONConfFileEditor.Abstractions.Classes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace JSONConfFileEditor.ViewModel
{
    public class PropertyEditor
    {


        private Object CustomConfigurationClass { get; set; }

        private ObservableCollection<PropertyDescription> allAvailableProperties;

        public ObservableCollection<PropertyDescription> AllAvailableProperties
        {

            get { return allAvailableProperties; }
            private set { allAvailableProperties = value; }
        }

        /// <summary>
        /// Event finalizing UI element binding to properties
        /// </summary>
        public delegate void EventFocusAction();
        public event EventFocusAction FocusEvent;

        public void FinalizeBinding()
        {
            // FocusEvent.Invoke();
        }

        PropertyDescriptionBuilder propertyDescriptionBuilder;

        public string GetNonValidMessage()
        {
            return propertyDescriptionBuilder.NonValidClassMessage;
        }

        public bool ValidateConfiguratioClass()
        {
            return propertyDescriptionBuilder.ValidateClass(CustomConfigurationClass.GetType());
        }

        public bool IsConfigurationFileValid { get; private set; }


        public PropertyEditor(Object customConfigurationClass)
        {

            CustomConfigurationClass = customConfigurationClass;

            propertyDescriptionBuilder = new PropertyDescriptionBuilder(CustomConfigurationClass);

            IsConfigurationFileValid = ValidateConfiguratioClass();

            if (propertyDescriptionBuilder.BuildProperties())
            {
                AllAvailableProperties = propertyDescriptionBuilder.AllAvailableProperties;
            }

        }


        public Object GetWrittenConfiguredClass()
        {

            FinalizeBinding();

            if (propertyDescriptionBuilder.ValidateValues(allAvailableProperties))
            {
                if (allAvailableProperties != null && allAvailableProperties.Count != 0)
                    PropertyDescriptionBuilder.SetObjectValuesWithPropertyDescription(CustomConfigurationClass, allAvailableProperties);
            }

            return CustomConfigurationClass;

        }

    }
}
