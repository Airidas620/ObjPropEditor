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
        /// Resolves Type properties and returns their descriptions with ObservableCollection<PropertyDescription>
        /// </summary>
        /// <param name="type"> Type for which properties will be resolved</param>
        /// <param name="src"> Configuration class object </param>
        /// <paramref name="writeValue">If to write values from configuration object to GUI</paramref>/>
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

            var props = type.GetProperties().ToList();


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

                    IEnumerable listItems = null;

                    if (src != null)
                    {
                        listItems = (IEnumerable)prop.GetValue(src);
                    }

                    TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), propertyDescription, listItems, depth);                   

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
        /// <param name="listType">List Type for which properties will be resolved</param>
        /// <param name="listPropDes">List property description</param>
        /// <param name="listSrc">Holds configuration class List Values</param>
        /// <param name="depth"> Propotional to how many times this and TryResolvePropertyAndAddToCollection functions were called recursively</param>
        /// <param name="maxDepth">Max depth allowed </param>
        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes, IEnumerable listItems = null, int depth = 0, int maxDepth = 20)
        {
            
            if (depth > maxDepth)
            {
                throw new Exception("Too many inner Object/Lists");
            }


            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Enum", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });

                SaveListItems(listPropDes, listItems);
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.Numeric });

                SaveListItems(listPropDes, listItems);
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.String });

                SaveListItems(listPropDes, listItems);
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.Bool });

                SaveListItems(listPropDes, listItems);
                return;
            }

            //List
            if (listType.IsGenericType)
            {
                if (listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    listPropDes.ListProperty = PossibleTypes.List;
                    listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "List", NestDepth = listPropDes.NestDepth, PropertyType = listType, GeneralProperty = PossibleTypes.List });
                    listPropDes.InnerPropertyDescriptions.Last().InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                    listPropDes.InnerPropertyDescriptions.Last().ListItems = new ObservableCollection<PropertyDescription>();
                    listPropDes.InnerPropertyDescriptions.Last().GeneralProperty = PossibleTypes.List;


                    TryResolveListAndAddToCollection(listType.GenericTypeArguments.First(), listPropDes.InnerPropertyDescriptions.Last(), null, depth);

                    SaveListItems(listPropDes, listItems);
                    return;
                }
            }

            //Class
            if (listType.IsClass)
            {

                listPropDes.ListProperty = PossibleTypes.Class;
                listPropDes.ListObjectType = listType;

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

                SaveListItems(listPropDes, listItems);
            }

        }

        /// <summary>
        /// Saves original configuration class List items to ListItems
        /// </summary>
        /// <param name="listPropDes">Property description of List<T> property</param>
        /// <param name="listItems">List items in List<T> property</param>
        void SaveListItems(PropertyDescription listPropDes, IEnumerable listItems)
        {
            if (listItems != null)
            {
                foreach (var item in listItems)
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

                        case PossibleTypes.List:

                            SaveListOfAList(listPropDes.ListItems.Last(), item as IEnumerable);

                            break;

                        case PossibleTypes.Class:
                            listPropDes.ListItems.Last().InnerPropertyDescriptions = GetTypePropertyDescriptions(item.GetType(), item, true);
                            break;
                    }

                    //Update ListItem indexes
                    listPropDes.ListItems.Last().parentListItems = listPropDes.ListItems;
                    listPropDes.ListItems.Last().ListItemIndex = listPropDes.ListItems.Count - 1;
                }
            }
        }

        void SaveListOfAList(PropertyDescription listProp, IEnumerable listItems)
        {
            if (listProp.ListProperty == PossibleTypes.List)
            {
                foreach (var item in listItems)
                {
                    listProp.ListItems.Add(
                        (PropertyDescription)listProp.InnerPropertyDescriptions.First().Clone()
                    );

                    listProp.ListItems.Last().parentListItems = listProp.ListItems;
                    listProp.ListItems.Last().ListItemIndex = listProp.ListItems.Count - 1;

                    SaveListOfAList(listProp.ListItems.Last(), item as IEnumerable);
                }
            }
            else
            {
                SaveListItems(listProp,listItems);
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
                        return CheckIfPropertyIsNumeric(Nullable.GetUnderlyingType(propType)); //This is very advanced check for numeric property
                    }
                    return false;
            }
            return false;
        }

    }

}
