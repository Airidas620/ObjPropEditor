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


        public List<int> Test { get; private set; } = new List<int>(){1,2,3,4};

        public Dictionary<int,string> Test2 { get; private set; } = new Dictionary<int, string>() { {1,"a"}, { 2, "a" }, { 3, "a" }, {4, "a" }, };

        public delegate void EventFocusAction();
        public event EventFocusAction FocusEvent;

        public void FinalizeBinding()
        {
            FocusEvent.Invoke();
        }

        private ObservableCollection<PropertyDescription> allAvailableProperties;

        public ObservableCollection<PropertyDescription> AllAvailableProperties {

            get { return allAvailableProperties; }
            private set { allAvailableProperties = value; } 
        }


        public int AllAvailablePropertiesLength { get ; private set; }

        public Object MyCustomConfigurationClass { get; private set; }


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
                AllAvailablePropertiesLength = AllAvailableProperties.Count;
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
