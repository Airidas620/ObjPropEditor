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

namespace JSONConfFileEditor.ViewModel
{
    public class JSONConfigurationEditor
    {

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


        public JSONConfigurationEditor(Object myCustomConfigurationClass)
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
            int propDesIndex = 0;

            if (allAvailableProperties != null && allAvailableProperties.Count != 0)
                PropertyDescriptionBuilder.SetObjectValuesWithPropertyDescription(MyCustomConfigurationClass, allAvailableProperties, ref propDesIndex);

            return MyCustomConfigurationClass; 
        }

    }
}
