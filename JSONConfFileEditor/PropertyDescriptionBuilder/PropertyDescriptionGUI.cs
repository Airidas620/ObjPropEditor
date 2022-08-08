﻿using JSONConfFileEditor.Abstractions.Enums;
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
        public static void SetObjectValuesWithPropertyDescription(ref Object src, ObservableCollection<PropertyDescription> propertyDescriptions, ref int propDesIndex)
        {
            var props = src.GetType().GetProperties().ToList();

            PropertyDescription propertyDescription;

            foreach (var prop in props)
            {
                //Skip ObjectLines and ListLines
                while (propertyDescriptions.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ObjectLine ||
                    propertyDescriptions.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ListLine)
                {
                    propDesIndex++;
                }


                propertyDescription = propertyDescriptions.ElementAt(propDesIndex);

                propDesIndex++;

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
                    prop.SetValue(src, InitializeNumericObjectFromType(prop.PropertyType, propertyDescription.ValueAsDouble.ToString()));
                    continue;
                }

                //List which has its own Type
                if (propertyDescription.GeneralProperty == PossibleTypes.List)
                {

                    if (propertyDescription.ListProperty == PossibleTypes.String || propertyDescription.ListProperty == PossibleTypes.Bool || propertyDescription.ListProperty == PossibleTypes.Numeric 
                        || propertyDescription.ListProperty == PossibleTypes.Enum || propertyDescription.ListProperty == PossibleTypes.Class )
                    {

                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                        if (propertyDescription.ListProperty == PossibleTypes.Numeric)
                        {
                            for (int i = 0; i < values.Length; i++)
                            {
                                //Change from double to required type
                                values.SetValue(InitializeNumericObjectFromType(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList[i].ToString()), i);
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

                        //Such prop value write is not good because it saves list by reference
                        //prop.SetValue(src, propertyDescription.ObjectList);
                        continue;
                    }

                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Class)
                {

                    if (prop.GetValue(src) == null)
                    {
                        prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                    }
                    var reccursiveObject = prop.GetValue(src);

                    SetObjectValuesWithPropertyDescription(ref reccursiveObject, propertyDescriptions, ref propDesIndex);
                }

            }          

        }


        /// <summary>
        /// Writes numeric values to src or returns them
        /// </summary>
        private static Object InitializeNumericObjectFromType(Type type, string valueAsDoubleString)
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
