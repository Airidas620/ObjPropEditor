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
        public static void SetObjectValuesWithPropertyDescription(Object src, ObservableCollection<PropertyDescription> propertyDescriptions, ref int propDesIndex)
        {
            var props = src.GetType().GetProperties().ToList();

            PropertyDescription propertyDescription;

            foreach (var prop in props)
            {
                //Skip ObjectLines and ListLines
                /*while (propertyDescriptions.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ObjectLine ||
                    propertyDescriptions.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ListLine)
                {
                    propDesIndex++;
                }*/


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

                    string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();
                    SetNumericProp(prop, prop.PropertyType, ValueAsDoubleString);

                    continue;
                }

                //List which has its own Type
                if (propertyDescription.GeneralProperty == PossibleTypes.List)
                {

                    if (propertyDescription.ListProperty == PossibleTypes.String)
                    {
                        prop.SetValue(src, propertyDescription.ObjectList.OfType<string>().ToList());
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Bool)
                    {
                        prop.SetValue(src, propertyDescription.ObjectList.OfType<bool>().ToList());
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Numeric)
                    {
                        string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();

                        //For Numeric Enum and Class types you need to convert from generic List(Double, Enum, Object) to src Type T list at runtime
                        //First array gets created of Type T. Then it's values are set with generic list
                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                        for (int i = 0; i < values.Length; i++)
                        {
                            ValueAsDoubleString = propertyDescription.ObjectList[i].ToString();
                            //Change from double to required type
                            values.SetValue(SetNumericProp(null, prop.PropertyType.GetGenericArguments().First(), ValueAsDoubleString, true), i);
                        }


                        //List<T> is created for src List with constructor List<T>(IEnumerable<T>)
                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Enum)
                    {

                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                        for (int i = 0; i < values.Length; i++)
                        {
                            values.SetValue(propertyDescription.ObjectList[i], i);
                        }

                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));
                        continue;
                    }

                    Array ArrayCreator(Type genericType, PropertyDescription propDescription)
                    {
                       
                        if(propDescription.ListProperty == PossibleTypes.List)
                        {
                            PropertyDescription listDescritpion = null;

                            for (int i = 0; i < propDescription.ListPropertyDescriptions.Count(); i++)
                            {
                                if (propDescription.ListPropertyDescriptions[i].GeneralProperty == PossibleTypes.List)
                                {
                                    listDescritpion = propDescription.ListPropertyDescriptions[i];
                                }
                            }

                            Console.WriteLine(genericType.GetGenericArguments().First());
                            

                            ArrayCreator(genericType.GetGenericArguments().First(), listDescritpion);
                        }

                        /*if (propertyDescription.ListProperty == PossibleTypes.String)
                        {
                            prop.SetValue(src, propertyDescription.ObjectList.OfType<string>().ToList());
                            continue;
                        }

                        if (propertyDescription.ListProperty == PossibleTypes.Bool)
                        {
                            prop.SetValue(src, propertyDescription.ObjectList.OfType<bool>().ToList());
                            continue;
                        }

                        if (propertyDescription.ListProperty == PossibleTypes.Numeric)
                        {
                            string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();

                            //For Numeric Enum and Class types you need to convert from generic List(Double, Enum, Object) to src Type T list at runtime
                            //First array gets created of Type T. Then it's values are set with generic list
                            Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                            for (int i = 0; i < values.Length; i++)
                            {
                                ValueAsDoubleString = propertyDescription.ObjectList[i].ToString();
                                //Change from double to required type
                                values.SetValue(SetNumericProp(null, prop.PropertyType.GetGenericArguments().First(), ValueAsDoubleString, true), i);
                            }


                            //List<T> is created for src List with constructor List<T>(IEnumerable<T>)
                            prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));
                            continue;
                        }

                        if (propertyDescription.ListProperty == PossibleTypes.Enum)
                        {

                            Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                            for (int i = 0; i < values.Length; i++)
                            {
                                values.SetValue(propertyDescription.ObjectList[i], i);
                            }

                            prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));
                            continue;
                        }*/
                        //Array values = Array.CreateInstance(genericType, list.Count());

                        /*
                        
                        if loop

                        for loop of length
                        array  = arraycreator
                        return array 


                        string...


                        return array
                         */

                        /*for (int i = 0; i < values.Length; i++)
                        {
                            Array InnerValues;
                            values.SetValue(propertyDescription.ListOfList[0], i);
                        }*/


                        /*
                         * 
                         * if list do array 
                         * jei ne toliau testi su string... ir paskui grazinti array, bet kur priskiriamas tas array su list
                        kveiciama funkcija kuris sukuria array ir pasukui jis grazinamas su return
                        for loop pereina per 2D dimensijas, kurios kviecia create array?


                         */

                        return null;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.List)
                    {


                        ArrayCreator(prop.PropertyType.GetGenericArguments().First(), propertyDescription);

                        //Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ListOfList.Count());

                        /*Console.WriteLine(prop.PropertyType.GetGenericArguments().First());
                        Console.WriteLine(propertyDescription.ListOfList[0].GetType());
                        Console.WriteLine(propertyDescription.ListOfList.Count());*/

                        /*for (int i = 0; i < values.Length; i++)
                        {
                            values.SetValue(propertyDescription.ListOfList[0], i);
                        }*/

                        //prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));


                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Class)
                    {
                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                        for (int i = 0; i < values.Length; i++)
                        {
                            values.SetValue(propertyDescription.ObjectList[i], i);
                        }

                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));

                    }
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Class)
                {

                    if (prop.GetValue(src) == null)
                    {
                        prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                    }
                    SetObjectValuesWithPropertyDescription(prop.GetValue(src), propertyDescriptions, ref propDesIndex);
                }

            }

            /// <summary>
            /// Writes numeric values to src or returns them
            /// </summary>
            Object SetNumericProp(PropertyInfo prop, Type type, string valueAsDoubleString, bool returnValue = false)
            {

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                        byte byteNumber;
                        if (Byte.TryParse(valueAsDoubleString, out byteNumber))
                        {
                            if (returnValue)
                                return byteNumber;
                            prop.SetValue(src, byteNumber);
                        }
                        break;

                    case TypeCode.Decimal:
                        decimal decimalNumber;
                        if (Decimal.TryParse(valueAsDoubleString, out decimalNumber))
                        {
                            if (returnValue)
                                return decimalNumber;
                            prop.SetValue(src, decimalNumber);
                        }
                        break;

                    case TypeCode.Double:
                        double doubleNumber;
                        if (Double.TryParse(valueAsDoubleString, out doubleNumber))
                        {
                            if (returnValue)
                                return doubleNumber;
                            prop.SetValue(src, doubleNumber);
                        }
                        break;

                    case TypeCode.Int16:
                        Int16 int16Number;
                        if (Int16.TryParse(valueAsDoubleString, out int16Number))
                        {
                            if (returnValue)
                                return int16Number;
                            prop.SetValue(src, int16Number);
                        }
                        break;

                    case TypeCode.Int32:
                        Int32 int32Number;
                        if (Int32.TryParse(valueAsDoubleString, out int32Number))
                        {
                            if (returnValue)
                                return int32Number;
                            prop.SetValue(src, int32Number);
                        }
                        break;

                    case TypeCode.Int64:
                        Int64 int64Number;
                        if (Int64.TryParse(valueAsDoubleString, out int64Number))
                        {
                            if (returnValue)
                                return int64Number;
                            prop.SetValue(src, int64Number);
                        }
                        break;

                    case TypeCode.SByte:
                        sbyte sbyteNumber;
                        if (sbyte.TryParse(valueAsDoubleString, out sbyteNumber))
                        {
                            if (returnValue)
                                return sbyteNumber;
                            prop.SetValue(src, sbyteNumber);
                        }
                        break;

                    case TypeCode.Single:
                        Single SingleNumber;
                        if (Single.TryParse(valueAsDoubleString, out SingleNumber))
                        {
                            if (returnValue)
                                return SingleNumber;
                            prop.SetValue(src, SingleNumber);
                        }
                        break;

                    case TypeCode.UInt16:
                        UInt16 uInt16Number;
                        if (UInt16.TryParse(valueAsDoubleString, out uInt16Number))
                        {
                            if (returnValue)
                                return uInt16Number;
                            prop.SetValue(src, uInt16Number);
                        }
                        break;

                    case TypeCode.UInt32:
                        UInt32 uInt32Number;
                        if (UInt32.TryParse(valueAsDoubleString, out uInt32Number))
                        {
                            if (returnValue)
                                return uInt32Number;
                            prop.SetValue(src, uInt32Number);
                        }
                        break;

                    case TypeCode.UInt64:
                        UInt64 uInt64Number;
                        if (UInt64.TryParse(valueAsDoubleString, out uInt64Number))
                        {
                            if (returnValue)
                                return uInt64Number;
                            prop.SetValue(src, uInt64Number);
                        }
                        break;
                }
                return null;
            }

        }
    }

}
