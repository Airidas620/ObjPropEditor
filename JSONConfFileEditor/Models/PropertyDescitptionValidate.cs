using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSONConfFileEditor.Abstractions.Enums;

namespace JSONConfFileEditor.Models
{
    public partial class PropertyDescriptionBuilder
    {

        public ValidationState ValidationState { get; set; }

        public string NonValidClassMessage { get; set; }

        public bool ValidateClass(Type classType, int depth = 0)
        {
            try
            {
                GetTypePropertyDescriptions(classType);
            }
            catch (Exception)
            {
                NonValidClassMessage = "Unknown builder error";
                ValidationState = ValidationState.Nonvalid;
                return false;

            }

            return ValidateClassProperties(classType);

        }

        private bool ValidateClassProperties(Type classType, int depth = 0)
        {
            var fields = classType.GetFields().ToList();

            if (fields.Count != 0)
            {
                NonValidClassMessage = "Fields are not supported";
                ValidationState = ValidationState.Nonvalid;
                return false;
            }

            var props = classType.GetProperties().ToList();

            foreach (var prop in props)
            {

                //Enum
                if (prop.PropertyType == typeof(char))
                {
                    NonValidClassMessage = "Char type is not supported";
                    ValidationState = ValidationState.Nonvalid;
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

                NonValidClassMessage = "Unresolved property type (" + prop.Name + ")";
                ValidationState = ValidationState.Nonvalid;
                return false;
            }



            ValidationState = ValidationState.Valid;
            return true;
        }

        private bool ValidateList(Type listType, int depth)
        {

            if (listType == typeof(char))
            {
                NonValidClassMessage = "Char type is not supported";
                ValidationState = ValidationState.Nonvalid;
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
                    ValidationState = ValidationState.Nonvalid;
                    return false;
                }

            //Class
            if (listType.IsClass)
            {
                return ValidateClass(listType, depth);
            }
            NonValidClassMessage = "Unresolved List property type";
            ValidationState = ValidationState.Nonvalid;
            return false;
        }
    }
}
