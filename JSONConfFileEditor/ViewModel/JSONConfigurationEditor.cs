﻿using JSONConfFileEditor.Abstractions.Enums;
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

            PropertyDescription propertyDescription;

            foreach (var field in fields)
            {
                setConfigInstanceMemebers(field.FieldType, field.GetValue(src), ref propDesIndex);
            }

            var props = type.GetProperties().ToList();


            foreach (var prop in props)
            {

                while(AllAvailableProperties.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.FieldLine)
                {
                    propDesIndex++;
                }
                propertyDescription = AllAvailableProperties.ElementAt(propDesIndex);


                if (propertyDescription.GeneralProperty == PossibleTypes.Enum)
                {
                    prop.SetValue(src, propertyDescription.ValueAsEnum);
                    
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.String)
                {
                    prop.SetValue(src, propertyDescription.ValueAsString);
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Bool)
                {
                    prop.SetValue(src, propertyDescription.ValueAsBool);
                }


                if (propertyDescription.GeneralProperty == PossibleTypes.Numeric)
                {

                    string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();

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
