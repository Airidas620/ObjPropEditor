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

        private ObservableCollection<PropertyDescription> availableProperties;
        public ObservableCollection<PropertyDescription> AvailableProperties 
        {
            get { return availableProperties; }            
            private set { availableProperties = value; }
        }        
        public Object ConfigurableClass { get; private set; }

        PropertyDescriptionBuilder propertyDescriptionBuilder;

        public JSONConfigurationEditor(Object configurableClass)
        {
            ConfigurableClass = configurableClass;
            propertyDescriptionBuilder = new PropertyDescriptionBuilder(ConfigurableClass);
            AvailableProperties = propertyDescriptionBuilder.AllAvailableProperties;   
        }


        public Object GetConfiguredClass()
        {         
            var parsedVersion = Version.Parse("1.0.0");

            int propDesIndex = 0;
            var config = ConfigurableClass;

            if (availableProperties.Count != 0) {                
                PropertyDescriptionBuilder.SetObjectValuesWithPropertyDescription(ref config,availableProperties, ref propDesIndex);
            }

            return config;
        }

    }
}
