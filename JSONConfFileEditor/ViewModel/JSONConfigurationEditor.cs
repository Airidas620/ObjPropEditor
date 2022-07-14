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

        public string buttonText { get; set; } = "hii";

        private ObservableCollection<PropertyDescription> allAvailableProperties;

        public ObservableCollection<PropertyDescription> AllAvailableProperties {
            get { return new ObservableCollection<PropertyDescription>(allAvailableProperties.Where(prop => prop.GeneralProperty != PossibleTypes.Class && prop.GeneralProperty != PossibleTypes.Null)); }
            private set { allAvailableProperties = value; } 
        }

        public int AllAvailablePropertiesLength { get ; private set; }

        public Object MyCustomConfigurationClass { get; private set; }


        public JSONConfigurationEditor(Object myCustomConfigurationClass)
        {

            MyCustomConfigurationClass = myCustomConfigurationClass;

            AllAvailableProperties = new PropertyDescriptionBuilder(MyCustomConfigurationClass).AllAvailableProperties;
            AllAvailablePropertiesLength = AllAvailableProperties.Count;

        }


        public Object GetConfiguredClass()
        {
            int propDesIndex = 0;

            if(allAvailableProperties.Count != 0)
                SetConfigInstanceMemebers(MyCustomConfigurationClass.GetType(), MyCustomConfigurationClass, ref propDesIndex);


            return MyCustomConfigurationClass; 
        }

        private void SetConfigInstanceMemebers(Type type, Object src, ref  int propDesIndex)
        {
            var fields = type.GetFields().ToList();

            PropertyDescription propertyDescription;

            /*foreach (var field in fields)
            {
                setConfigInstanceMemebers(field.FieldType, field.GetValue(src), ref propDesIndex);
            }*/

            var props = type.GetProperties().ToList();


            foreach (var prop in props)
            {

                while(allAvailableProperties.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ObjectLine)
                {
                    propDesIndex++;
                }
                propertyDescription = allAvailableProperties.ElementAt(propDesIndex);

                propDesIndex++;

                if (propertyDescription.GeneralProperty == PossibleTypes.Enum)
                {
                    prop.SetValue(src, propertyDescription.ValueAsEnum);
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.String)
                {
                    prop.SetValue(src, propertyDescription.ValueAsString);
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Bool)
                {

                    prop.SetValue(src, propertyDescription.ValueAsBool);
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Numeric)
                {

                    string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();
                    #region Numeric types
                    switch (Type.GetTypeCode(prop.PropertyType))
                    {
                        case TypeCode.Byte:
                            byte byteNumber;
                            if (Byte.TryParse(ValueAsDoubleString, out byteNumber))
                            {
                                prop.SetValue(src, byteNumber);
                            }
                            break;

                        case TypeCode.Decimal:
                            decimal decimalNumber;
                            if (Decimal.TryParse(ValueAsDoubleString, out decimalNumber))
                            {
                                prop.SetValue(src, decimalNumber);
                            }
                            break;

                        case TypeCode.Double:
                            double doubleNumber;
                            if (Double.TryParse(ValueAsDoubleString, out doubleNumber))
                            {
                                prop.SetValue(src, doubleNumber);
                            }
                            break;

                        case TypeCode.Int16:
                            Int16 int16Number;
                            if (Int16.TryParse(ValueAsDoubleString, out int16Number))
                            {
                                prop.SetValue(src, int16Number);
                            }
                            break;

                        case TypeCode.Int32:
                            Int32 int32Number;
                            if (Int32.TryParse(ValueAsDoubleString, out int32Number))
                            {
                                prop.SetValue(src, int32Number);
                            }
                            break;

                        case TypeCode.Int64:
                            Int64 int64Number;
                            if (Int64.TryParse(ValueAsDoubleString, out int64Number))
                            {
                                prop.SetValue(src, int64Number);
                            }
                            break;

                        case TypeCode.SByte:
                            sbyte sbyteNumber;
                            if (sbyte.TryParse(ValueAsDoubleString, out sbyteNumber))
                            {
                                prop.SetValue(src, sbyteNumber);
                            }
                            break;

                        case TypeCode.Single:
                            Single SingleNumber;
                            if (Single.TryParse(ValueAsDoubleString, out SingleNumber))
                            {
                                prop.SetValue(src, SingleNumber);
                            }
                            break;

                        case TypeCode.UInt16:
                            UInt16 uInt16Number;
                            if (UInt16.TryParse(ValueAsDoubleString, out uInt16Number))
                            {
                                prop.SetValue(src, uInt16Number);
                            }
                            break;

                        case TypeCode.UInt32:
                            UInt32 uInt32Number;
                            if (UInt32.TryParse(ValueAsDoubleString, out uInt32Number))
                            {
                                prop.SetValue(src, uInt32Number);
                            }
                            break;

                        case TypeCode.UInt64:
                            UInt64 uInt64Number;
                            if (UInt64.TryParse(ValueAsDoubleString, out uInt64Number))
                            {
                                prop.SetValue(src, uInt64Number);
                            }
                            break;
                    }
                    #endregion
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.List)
                {
                    if(propertyDescription.ListProperty == PossibleTypes.String)
                    {
                        //((List<string>)(prop.GetValue(src))).AddRange(propertyDescription.StringList);

                        prop.SetValue(src, propertyDescription.StringList.ToList());
                    }
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Class)
                {
                    SetConfigInstanceMemebers(prop.PropertyType, prop.GetValue(src), ref propDesIndex);
                }



            }
           
        }

    }
}
