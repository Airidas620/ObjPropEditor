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
    public class JSONEditorViewModel
    {


        public Object MyCustomConfigurationClass { get; private set; }

        private ObservableCollection<PropertyDescription> allAvailableProperties;

        public ObservableCollection<PropertyDescription> AllAvailableProperties {

            get { return allAvailableProperties; }
            private set { allAvailableProperties = value; } 
        }


        public delegate void EventFocusAction();
        public event EventFocusAction FocusEvent;

        public void FinalizeBinding()
        {
            FocusEvent.Invoke();
        }


        PropertyDescriptionBuilder propertyDescriptionBuilder;

        public ValidationState GetValidationState()
        {          
            return propertyDescriptionBuilder.ValidationState;
        }

        public string GetNonValidMessage()
        {
            return propertyDescriptionBuilder.NonValidClassMessage;
        }

        public void ValidateConfiguratioClass()
        {
            propertyDescriptionBuilder.ValidateClass(MyCustomConfigurationClass.GetType());
        }


        public JSONEditorViewModel(Object myCustomConfigurationClass)
        {

            MyCustomConfigurationClass = myCustomConfigurationClass;

            propertyDescriptionBuilder = new PropertyDescriptionBuilder(MyCustomConfigurationClass);

            if (propertyDescriptionBuilder.BuildProperties())
            {
                AllAvailableProperties = propertyDescriptionBuilder.AllAvailableProperties;
            }

        }


        public Object GetConfiguredClass()
        {

            if (allAvailableProperties != null && allAvailableProperties.Count != 0)
                PropertyDescriptionBuilder.SetObjectValuesWithPropertyDescription(MyCustomConfigurationClass, allAvailableProperties);

            return MyCustomConfigurationClass; 
        }

    }
}
