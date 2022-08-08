using VisualPropertyEditor.Abstractions.Classes;
using VisualPropertyEditor.Abstractions.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using VisualPropertyEditor.Abstractions;

namespace VisualPropertyEditor.Models
{
    public partial class PropertyDescriptionBuilder
    {

        public ObservableCollection<PropertyDescription> AllAvailableProperties { get; set; }

        Object ConfigurationClass;

        public PropertyDescriptionBuilder(Object configurationClass)
        {
            ConfigurationClass = configurationClass;
        }


        public ObservableCollection<PropertyDescription> BuildProperties()
        {        
            AllAvailableProperties = GetTypePropertyDescriptions(ConfigurationClass.GetType(), ConfigurationClass, true);
            return AllAvailableProperties;
        }

        /// <summary>
        /// Resolves Type properties and adds returns their descriptions with ObservableCollection<PropertyDescription>
        /// </summary>
        /// <param name="type"> Type for which properties will be resolved</param>
        /// <param name="depth"> Proportional to how many times this function was called recursively>
        /// <param name="maxDepth"> Max depth allowed </param>
        public ObservableCollection<PropertyDescription> GetTypePropertyDescriptions(Type type, Object src, bool writeValue, int depth = 0, int maxDepth = 20)
        {           

            //Collection that will be returned
            var availableProperties = new ObservableCollection<PropertyDescription>();

            if (depth >= maxDepth)
            {
                return availableProperties;
            }

            /*if (src != null)
            {
                type = src.GetType();
            }*/

            var props = type.GetProperties().ToList();



            int index = 0;
            foreach (var prop in props)
            {
                try
                {
                    var resolvedProp = ResolveProperty(prop, src, writeValue, depth, maxDepth);
                    if (resolvedProp.GeneralProperty != PossibleTypes.Unknown)
                    {
                        availableProperties.Add(resolvedProp);
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unresolved property {prop.Name}. Exception: " + ex.Message);
                }
                index++;
            }

            return availableProperties;
        }

        private PropertyDescription ResolveProperty(System.Reflection.PropertyInfo prop, Object src, bool writeValue, int depth, int maxDepth)
        {        

            var propertyDescription = new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType};
            
            var attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false);
            
            if (attributes != null && attributes.Length > 0)
            {
                propertyDescription.Description = ((DescriptionAttribute)attributes[0]).Description;
            }

            if (prop.PropertyType.IsEnum)
            {               
                propertyDescription.GeneralProperty = PossibleTypes.Enum;
                propertyDescription.AvailableEnumValues = Enum.GetValues(prop.PropertyType);

                if (writeValue && src != null)
                {
                    propertyDescription.ValueAsEnum = prop.GetValue(src);
                }          
                
                return propertyDescription;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(prop.PropertyType))
            {
                propertyDescription.GeneralProperty = PossibleTypes.Numeric;               

                if (writeValue && src != null)
                {
                    propertyDescription.NumericValueAsString = prop.GetValue(src).ToString();
                }        

                return propertyDescription;
            }

            //String
            if (prop.PropertyType == typeof(string))
            {
                propertyDescription.GeneralProperty = PossibleTypes.String;               

                 if (writeValue && src != null)
                {
                    if (prop.GetValue(src) != null)
                    {
                        propertyDescription.ValueAsString = prop.GetValue(src).ToString();
                    }
                }            
                return propertyDescription;
            }

            //Bool
            if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                propertyDescription.GeneralProperty = PossibleTypes.Bool;               

                if (writeValue && src != null)
                {
                    if (prop.GetValue(src) != null)
                    {
                        propertyDescription.ValueAsBool = (bool)prop.GetValue(src);
                    }
                }
               
                return propertyDescription;
            }

            //List
            if (prop.PropertyType.IsGenericType)
            {
                if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    propertyDescription.InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                    propertyDescription.ListItems = new ObservableCollection<PropertyDescription>();
                    propertyDescription.GeneralProperty = PossibleTypes.List;              

                    IEnumerable iErnurable = null;

                    if (src != null)
                    {
                        iErnurable = (IEnumerable)prop.GetValue(src);
                    }

                    //Function to resolve List<T> Type T properties
                    TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), propertyDescription, iErnurable, depth);                   

                    return propertyDescription;
                }
            }

            //Class
            if (prop.PropertyType.IsClass)
            {
                int increasedDepth = depth + 1;

                propertyDescription.GeneralProperty = PossibleTypes.Class;
                propertyDescription.InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>();  

                if (writeValue && prop.GetValue(src) == null)
                {
                    prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                }

                var resolvedProps = writeValue ? GetTypePropertyDescriptions(prop.PropertyType, prop.GetValue(src), writeValue, increasedDepth, maxDepth) :
                    GetTypePropertyDescriptions(prop.PropertyType, null, writeValue, increasedDepth, maxDepth);


                foreach (var item in resolvedProps)
                {
                    propertyDescription.InnerPropertyDescriptions.Add(item);
                }

                
                return propertyDescription;
            }


            propertyDescription.GeneralProperty = PossibleTypes.Unknown;
            return propertyDescription;
        }


        /// <summary>
        /// Resolves List<T> Type T properties and adds their descriptions to ObservableCollection<PropertyDescription> List
        /// </summary>
        /// <param name="listType">list Type for which properties will be resolved</param>
        /// <paramref name="listPropDes">Property descriptions will be stored in List parents PropertyDescription.innerPropertyDescriptions property array</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times this and TryResolvePropertyAndAddToCollection functions were called recursively>
        /// <paramref name="maxDepth"/ Max depth allowed >
        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes, IEnumerable listSrc = null, int depth = 0, int maxDepth = 20, Object src = null)
        {

            //TODO change from property type Type to Get GenericAruments Get First()
            if (depth > maxDepth)
            {
                throw new Exception("Too many inner Object/Lists");
            }


            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listPropDes.PropertyName, NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });

                SaveToList();
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.Numeric });

                SaveToList();
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.String });

                SaveToList();
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.Bool });

                SaveToList();
                return;
            }

            //List TODO

            //Class
            if (listType.IsClass)
            {

                listPropDes.ListProperty = PossibleTypes.Class;
                listPropDes.ListObjectType = listType; // TODO replace with PropertyType

                var increasedDepth = depth + 1;

                var reccursiveProperty = new PropertyDescription() { PropertyType = listType, PropertyName = listPropDes.PropertyName, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.Class, InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>() };
                listPropDes.InnerPropertyDescriptions.Add(reccursiveProperty);

                //Since List<T> holds class properties they need to be resolved
                var resolvedProps = GetTypePropertyDescriptions(listType, null, false, depth);

                var innerCollection = reccursiveProperty.InnerPropertyDescriptions;

                foreach (var item in resolvedProps)
                {
                    innerCollection.Add(item);
                }

                SaveToList();
            }

            void SaveToList()
            {
                if (listSrc != null)
                {
                    foreach (var item in listSrc)
                    {
                        listPropDes.ListItems.Add(
                           (PropertyDescription)listPropDes.InnerPropertyDescriptions.First().Clone()
                        );

                        switch (listPropDes.ListProperty)
                        {
                            case PossibleTypes.String:
                                listPropDes.ListItems.Last().ValueAsString = item.ToString();
                                break;

                            case PossibleTypes.Numeric:
                                listPropDes.ListItems.Last().NumericValueAsString = item.ToString();
                                break;

                            case PossibleTypes.Bool:
                                listPropDes.ListItems.Last().ValueAsBool = (bool)item;
                                break;
                            case PossibleTypes.Enum:
                                listPropDes.ListItems.Last().ValueAsEnum = item;
                                break;
                            case PossibleTypes.Class:
                                listPropDes.ListItems.Last().InnerPropertyDescriptions = GetTypePropertyDescriptions(item.GetType(), item, true, depth);
                                break;
                        }

                        listPropDes.ListItems.Last().parentListItems = listPropDes.ListItems;
                        listPropDes.ListItems.Last().ListItemIndex = listPropDes.ListItems.Count - 1;
                    }
                }
            }

        }
   
        
        public bool CheckIfPropertyIsNumeric(Type propType)
        {

            if (propType == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(propType))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //return IsNumericType(Nullable.GetUnderlyingType(type)); This is very advanced check for numeric property
                    }
                    return false;
            }
            return false;
        }

    }

}
