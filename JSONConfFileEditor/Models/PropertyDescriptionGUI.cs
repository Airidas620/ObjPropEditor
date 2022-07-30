using JSONConfFileEditor.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
    public partial class PropertyDescriptionBuilder
    {


        /// <summary>
        /// Writes values to Object src from GUI values
        /// </summary>
        /// <param name="src">Object to write values to</param>
        /// <param name="propertyDescriptions">List that holds Object src property descriptions</param>
        /// <param name="propDesIndex">Index for tracking propertyDescriptions current member</param>
        public static void SetObjectValuesWithPropertyDescription(Object src, ObservableCollection<PropertyDescription> propertyDescriptions)
        {
            var props = src.GetType().GetProperties().ToList();

            int currentIndex = 0;

            PropertyDescription propertyDescription;

            foreach (var prop in props)
            {

                propertyDescription = propertyDescriptions.ElementAt(currentIndex);
                currentIndex++;

                //Enum
                if (propertyDescription.GeneralProperty == PossibleTypes.Enum)
                {
                    prop.SetValue(src, propertyDescription.ValueAsEnum);
                    continue;
                }

                //String
                if (propertyDescription.GeneralProperty == PossibleTypes.String)
                {
                    prop.SetValue(src, propertyDescription.ValueAsString);
                    continue;
                }

                //Bool
                if (propertyDescription.GeneralProperty == PossibleTypes.Bool)
                {
                    prop.SetValue(src, propertyDescription.ValueAsBool);
                    continue;
                }

                //Numeric
                if (propertyDescription.GeneralProperty == PossibleTypes.Numeric)
                {
                    prop.SetValue(src, SetNumericProp(prop.PropertyType, propertyDescription.ValueAsDouble.ToString()));
                    continue;
                }

                //List which has its own Type
                if (propertyDescription.GeneralProperty == PossibleTypes.List)
                {

                    propertyDescription.SaveGUIListDataToList();


                    if (propertyDescription.ListProperty == PossibleTypes.String || propertyDescription.ListProperty == PossibleTypes.Bool || propertyDescription.ListProperty == PossibleTypes.Numeric 
                        || propertyDescription.ListProperty == PossibleTypes.Enum || propertyDescription.ListProperty == PossibleTypes.Class )
                    {
                        //Todo check why saving by reference is ok

                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                        if (propertyDescription.ListProperty == PossibleTypes.Numeric && prop.PropertyType.GetGenericArguments().First() != typeof(double))//TODO check if not double
                        {
                            for (int i = 0; i < values.Length; i++)
                            {
                                //Change from double to required type
                                values.SetValue(SetNumericProp(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList[i].ToString()), i);
                            }
                        }

                        else
                        {
                            for (int i = 0; i < values.Length; i++)
                            {
                                values.SetValue(propertyDescription.ObjectList[i], i);
                            }
                        }

                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));

                        continue;
                    }

                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Class)
                {

                    if (prop.GetValue(src) == null)
                    {
                        //Console.WriteLine(prop.PropertyType);
                        prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                    }
                    SetObjectValuesWithPropertyDescription(prop.GetValue(src), propertyDescription.InnerPropertyDescriptions); //propertyDescription.InnerPropertyDescriptions
                }

            }

            /// <summary>
            /// Writes numeric values to src or returns them
            /// </summary>
            Object SetNumericProp(Type type, string valueAsDoubleString)
            {

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                        Byte byteNumber;
                        if (Byte.TryParse(valueAsDoubleString, out byteNumber))
                        {
                            return byteNumber;
                        }
                        break;

                    case TypeCode.Decimal:
                        Decimal decimalNumber;
                        if (Decimal.TryParse(valueAsDoubleString, out decimalNumber))
                        {
                            return decimalNumber;
                        }
                        break;

                    case TypeCode.Double:
                        Double doubleNumber;
                        if (Double.TryParse(valueAsDoubleString, out doubleNumber))
                        {
                            return doubleNumber;
                        }
                        break;

                    case TypeCode.Int16:
                        Int16 int16Number;
                        if (Int16.TryParse(valueAsDoubleString, out int16Number))
                        {
                            return int16Number;
                        }
                        break;

                    case TypeCode.Int32:
                        Int32 int32Number;
                        if (Int32.TryParse(valueAsDoubleString, out int32Number))
                        {
                            return int32Number;
                        }
                        break;

                    case TypeCode.Int64:
                        Int64 int64Number;
                        if (Int64.TryParse(valueAsDoubleString, out int64Number))
                        {
                            return int64Number;
                        }
                        break;

                    case TypeCode.SByte:
                        sbyte sbyteNumber;
                        if (sbyte.TryParse(valueAsDoubleString, out sbyteNumber))
                        {
                            return sbyteNumber;
                        }
                        break;

                    case TypeCode.Single:
                        Single SingleNumber;
                        if (Single.TryParse(valueAsDoubleString, out SingleNumber))
                        {
                            return SingleNumber;
                        }
                        break;

                    case TypeCode.UInt16:
                        UInt16 uInt16Number;
                        if (UInt16.TryParse(valueAsDoubleString, out uInt16Number))
                        {
                            return uInt16Number;
                        }
                        break;

                    case TypeCode.UInt32:
                        UInt32 uInt32Number;
                        if (UInt32.TryParse(valueAsDoubleString, out uInt32Number))
                        {
                            return uInt32Number;
                        }
                        break;

                    case TypeCode.UInt64:
                        UInt64 uInt64Number;
                        if (UInt64.TryParse(valueAsDoubleString, out uInt64Number))
                        {
                            return uInt64Number;
                        }
                        break;
                }
                return null;
            }

        }
    }

}
