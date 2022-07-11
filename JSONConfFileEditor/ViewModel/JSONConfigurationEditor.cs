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

namespace JSONConfFileEditor.ViewModel
{
    public class JSONConfigurationEditor : INotifyPropertyChanged
    {

        public ObservableCollection<PropertyDescription> AllAvailableProperties { get; private set; }

        public int AllAvailablePropertiesLength { get; private set; }

        public Object MyCustomConfigurationClass { get; private set; }

        public JSONConfigurationEditor(Object myCustomConfigurationClass)
        {

            MyCustomConfigurationClass = myCustomConfigurationClass;

            AllAvailableProperties = new PropertyDescriptionBuilder(MyCustomConfigurationClass).AllAvailableProperties;
            AllAvailablePropertiesLength = AllAvailableProperties.Count;

        }


        public void SetUpConfigurationClass2()
        {
            int propDesIndex = 0;

            setConfigInstanceMemebers(MyCustomConfigurationClass.GetType(), MyCustomConfigurationClass, ref propDesIndex);

            Console.WriteLine("n " + propDesIndex);

        }

        private void setConfigInstanceMemebers(Type type, Object src, ref  int propDesIndex)
        {
            
            var fields = type.GetFields().ToList();

            foreach (var field in fields)
            {
                setConfigInstanceMemebers(field.FieldType, field.GetValue(src), ref propDesIndex);
            }

            var props = type.GetProperties().ToList();

            foreach (var prop in props)
            {

                if (prop.PropertyType.IsEnum)
                {
                    prop.SetValue(src, AllAvailableProperties.ElementAt(propDesIndex).ValueAsEnum);
                    continue;
                }

                if (CheckIfPropertyIsNumeric(prop))
                {
                    prop.SetValue(src, AllAvailableProperties.ElementAt(propDesIndex).ValueAsDouble);
                }

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(src, AllAvailableProperties.ElementAt(propDesIndex).ValueAsString);//AllAvailableProperties.ElementAt(propDesIndex).ValueAsString
                }


                if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(src, AllAvailableProperties.ElementAt(propDesIndex).ValueAsBool);
                }

                propDesIndex++;

            }
        }

        #region commented text

        //SaveConfiguration()
        //{
        //    var props = curentConfigClass.GetProperties();

        //    props.ForEach(prop =>
        //    {
        //        if (AllAvailableProps.Contains(prop.Name)) ;

        //        //What is property type
        //        //Get that property value
        //        //If successful 
        //        //currentConfigClass.thatproperty = resolvedValue;

        //    } );

        //    // serialize current class and save to file;
        //}

        //  public Dictionary<string,T> AllAvailableProps { get; set; }

        #endregion

        #region notification
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
