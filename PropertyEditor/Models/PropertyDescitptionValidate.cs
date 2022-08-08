using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisualPropertyEditor.Abstractions;

namespace VisualPropertyEditor.Models
{
    public partial class PropertyDescriptionBuilder
    {
        //This is trashy

        public string NonValidClassMessage { get; set; }

        public bool ValidateClass(Type classType)
        {
            return ValidateClassProperties(classType, 0);

        }

        public bool ValidateValues(ObservableCollection<PropertyDescription> propertyDescriptions)
        {
            if (propertyDescriptions != null)
            {
                foreach (PropertyDescription propertyDescription in propertyDescriptions)
                {
                    ValidateValues(propertyDescription.InnerPropertyDescriptions);
                    if (!propertyDescription.IsInputValueValid)
                    {
                        NonValidClassMessage = "Invalid values";
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidateClassProperties(Type classType, int depth = 0)
        {
            var fields = classType.GetFields().ToList();

            if (fields.Count != 0)
            {
                NonValidClassMessage = "Fields are not supported";
                return false;
            }

            var props = classType.GetProperties().ToList();

            foreach (var prop in props)
            {  
                //Enum
                if (prop.PropertyType == typeof(char))
                {
                    NonValidClassMessage = "Char type is not supported";
                    return false;
                }

                //Enum
                if (prop.PropertyType.IsEnum)
                {
                    continue;
                }

                //Numeric
                if (CheckIfPropertyIsNumeric(prop.PropertyType))
                {
                    continue;
                }

                //String
                if (prop.PropertyType == typeof(string))
                {
                    continue;
                }

                //Bool
                if (prop.PropertyType == typeof(bool))
                {
                    continue;
                }


                //List
                if (prop.PropertyType.IsGenericType)
                    if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                        prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))

                    {
                        //TODO validate if list 
                        if (ValidateList(prop.PropertyType.GenericTypeArguments.First(), depth))
                            continue;

                        return false;
                    }


                //Class
                if (prop.PropertyType.IsClass)
                {
                    depth += 1;
                    if (depth > 100)
                    {

                    }
                    ValidateClassProperties(prop.PropertyType, depth);
                    continue;
                }


                try
                {
                    ResolveProperty(prop, null, false, 0, 20);
                    continue;
                }
                catch (Exception ex)
                {
                    NonValidClassMessage = $"Unresolved property ({prop.Name}). Exception thrown: {ex.Message}";
                    return false;
                }

                NonValidClassMessage = "Unresolved property type (" + prop.Name + ")";
                return false;
            }

            return true;
        }

        private bool ValidateList(Type listType, int depth)
        {

            if (listType == typeof(char))
            {
                NonValidClassMessage = "Char type is not supported";
                return false;
            }

            //Enum
            if (listType.IsEnum)
            {
                return true;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                return true;
            }

            //String
            if (listType == typeof(string))
            {
                return true;
            }

            //Bool
            if (listType == typeof(bool))
            {
                return true;
            }


            if (listType.IsGenericType)
                if (listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                {

                    NonValidClassMessage = "Multidimensional Lists are not supported";
                    return false;
                }

            //Class
            if (listType.IsClass)
            {
                return ValidateClassProperties(listType, depth);
            }
            NonValidClassMessage = "Unresolved List property type";
            return false;
        }
    }
}
