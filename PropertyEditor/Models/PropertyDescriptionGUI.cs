using VisualPropertyEditor.Abstractions.Classes;
using VisualPropertyEditor.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VisualPropertyEditor.Models
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
                    prop.SetValue(src, NumericParser.StringToNumericTypeValue(prop.PropertyType, propertyDescription.NumericValueAsString));
                    continue;
                }

                //List
                if (propertyDescription.GeneralProperty == PossibleTypes.List)
                {

                    propertyDescription.SaveGUIListDataToList();


                    if (propertyDescription.ListProperty == PossibleTypes.String || propertyDescription.ListProperty == PossibleTypes.Bool || propertyDescription.ListProperty == PossibleTypes.Numeric
                        || propertyDescription.ListProperty == PossibleTypes.Enum || propertyDescription.ListProperty == PossibleTypes.Class)
                    {

                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());

                        if (propertyDescription.ListProperty == PossibleTypes.Numeric)
                        {
                            for (int i = 0; i < values.Length; i++)
                            {
                                //Change from double to required type
                                values.SetValue(NumericParser.StringToNumericTypeValue(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList[i].ToString()), i);
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
                        prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                    }

                    SetObjectValuesWithPropertyDescription(prop.GetValue(src), propertyDescription.InnerPropertyDescriptions);
                }

            }


        }
    }

}
