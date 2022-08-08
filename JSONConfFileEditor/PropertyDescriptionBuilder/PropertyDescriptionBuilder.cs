using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JSONConfFileEditor.Models
{
    public partial class PropertyDescriptionBuilder
    {

        public ObservableCollection<PropertyDescription> AllAvailableProperties { get; set; }

        public PropertyDescriptionBuilder(Object customConfigurationClass)
        {
            //AllAvailableProperties = new ObservableCollection<PropertyDescription>();

            AllAvailableProperties = new ObservableCollection<PropertyDescription>(GetTypePropertyDescriptions(customConfigurationClass.GetType()));
        }

        /// <summary>
        /// Resolves Type properties and adds their descriptions to ObservableCollection<PropertyDescription> array
        /// </summary>
        /// <param name="type">Type for which properties will be resolved</param>
        /// <paramref name="currentDescription"> holds descriptions for type properties</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times this function was called recursively>
        public ObservableCollection<PropertyDescription> GetTypePropertyDescriptions(Type type, int depth = 0)
        {
            var availableProperties = new ObservableCollection<PropertyDescription>();

            var props = type.GetProperties().ToList();

            foreach (var prop in props)
            {
                //Enum
                if (prop.PropertyType.IsEnum)
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(prop.PropertyType) });
                    continue;
                }

                //Numeric
                if (CheckIfPropertyIsNumeric(prop.PropertyType))
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                    continue;
                }

                //String
                if (prop.PropertyType == typeof(string))
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.String });
                    continue;
                }

                //Bool
                if (prop.PropertyType == typeof(bool))
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Bool });
                    continue;
                }

                //List
                if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    var listProp = new PropertyDescription()
                    {
                        PropertyName = prop.Name,
                        NestDepth = depth,
                        ObjectList = new List<Object>(),
                        PropertyType = prop.PropertyType,
                        ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                        DescriptionList = new ObservableCollection<string>(),
                        GeneralProperty = PossibleTypes.List
                    };
                    //Function to resolve List<T> Type T properties
                    TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), listProp, depth);
                    availableProperties.Add(listProp);

                    continue;
                }
                //Class
                if (prop.PropertyType.IsClass)                {
                    var increasedDepth = depth + 1;

                    var reccursiveProperty = new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.Class };
                    availableProperties.Add(reccursiveProperty);

                    var resolvedProps = GetTypePropertyDescriptions(prop.PropertyType, increasedDepth);
                    resolvedProps.ToList().ForEach(resolvedProp => availableProperties.Add(resolvedProp));
                }
            }

            return availableProperties;
        }

        /// <summary>
        /// Resolves List<T> Type t properties and adds their descriptions to ObservableCollection<PropertyDescription> List
        /// </summary>
        /// <param name="listType">Type for which properties will be resolved</param>
        /// <paramref name="listPropDes">Property descriptions will be stored in List parents PropertyDescription.listPropertyDescriptions property</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times this and TryResolvePropertyAndAddToCollection functions were called recursively>
        private void TryResolveListAndAddToCollection(Type listType,ref PropertyDescription listPropDes, int depth)
        {
            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listPropDes.PropertyName, NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.String });
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Bool });
                return;
            }

            //Class
            if (listType.IsClass)
            {
                var increasedDepth = depth + 40;

                listPropDes.ListProperty = PossibleTypes.Class;
                listPropDes.ListObjectType = listType;
                listPropDes.ListObjectType = listType;
                
                listPropDes.ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(GetTypePropertyDescriptions(listType, increasedDepth));               
            }

        }



        public class PropertyDescription
        {
            #region ListProperties

            /// <summary>
            /// <ObservableCollection<PropertyDescription> list to hold List<T> property properties
            /// </summary>
            private ObservableCollection<PropertyDescription> listPropertyDescriptions;

            public ObservableCollection<PropertyDescription> ListPropertyDescriptions
            {
                get { return listPropertyDescriptions; }
                set { 
                    if(listPropertyDescriptions != value)
                        {
                            listPropertyDescriptions = value;
                        }
                    }
            }

            /// <summary>
            /// (Not used)
            /// </summary>
            public ObservableCollection<PropertyDescription> ListPropertyDescriptionsGraph
            {
                get { return new ObservableCollection<PropertyDescription>(listPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.Class)); }
                set { listPropertyDescriptions = value; }
            }


            /// <summary>
            /// Property to describe added list item for WPF ListBox
            /// </summary>
            public ObservableCollection<string> DescriptionList { get; set; }

            /// <summary>
            /// List to hold List<string,bool,double,enum,object> values 
            /// </summary>
            public List<Object> ObjectList { get; set; }

            /// <summary>
            /// Objects List<T> list Type T resolved at runtime
            /// </summary>
            public Type ListObjectType { get; set; }
            #endregion

            #region ObjectProperties

            //Properties used for single Type T properties

            /// <summary>
            /// Type of Property
            /// </summary>
            public Type PropertyType { get; set; }

            /// <summary>
            /// Name of property
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Holds numeric values if property is double
            /// </summary>
            public double ValueAsDouble { get; set; }

            /// <summary>
            /// Holds string values if property is atring
            /// </summary>
            public string ValueAsString { get; set; } = "";

            /// <summary>
            /// Holds bool values if property is bool
            /// </summary>
            public bool ValueAsBool { get; set; }

            /// <summary>
            /// Holds enum values if property is enum
            /// </summary>
            public Enum ValueAsEnum { get; set; }

            /// <summary>
            /// All available enum property enum values
            /// </summary>
            public Array AvailableEnumValues { get; set; }

            /// <summary>
            /// Property Type as PossibleTypes enum value
            /// </summary>
            public PossibleTypes GeneralProperty { get; set; }

            /// <summary>
            /// List property Type as PossibleTypes enum value
            /// </summary>
            public PossibleTypes ListProperty { get; set; }

            /// <summary>
            /// Proportional to how many times resolve functions were called recursively
            /// </summary>
            public int NestDepth { get; set; }

            #endregion

            /// <summary>
            /// Command for adding values to List<>
            /// </summary>
            public RelayCommand AddToListCommand { set; get; }

            /// <summary>
            /// Command for removing values from List<>
            /// </summary>
            public RelayCommand RemoveFromListCommand { set; get; }

            private int selectedItem = -1;

            public int SelectedItem
            {
                get { return selectedItem; }
                set
                {
                    if (value != selectedItem)
                    {
                        selectedItem = value;
                    }
                }
            }


            private void ExecuteAddToListCommand(object obj)
            {

                if (ListProperty == PossibleTypes.String)
                {

                    //Since single property list only have one property description
                    //Last() can be replaced with First() or [0]
                    ObjectList.Add(listPropertyDescriptions.Last().ValueAsString);

                    if (listPropertyDescriptions.Last().ValueAsString != "")
                        DescriptionList.Add(ObjectList.Last().ToString());

                    else
                        DescriptionList.Add("\"\"");
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    ObjectList.Add(listPropertyDescriptions.Last().ValueAsBool);
                    DescriptionList.Add(ObjectList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    ObjectList.Add(listPropertyDescriptions.Last().ValueAsDouble);
                    DescriptionList.Add(ObjectList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    if(listPropertyDescriptions.Last().ValueAsEnum != null)
                    {
                        ObjectList.Add(listPropertyDescriptions.Last().ValueAsEnum);
                        DescriptionList.Add(ObjectList.Last().ToString());
                    }
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    //Create object of List<T> Type T 
                    Object instance = Activator.CreateInstance(ListObjectType);

                    int propDesIndex = 0;

                    //Assing values to Object from GUI 
                    SetObjectValuesWithPropertyDescription(ref instance, ListPropertyDescriptions, ref propDesIndex);

                    //Add to List
                    ObjectList.Add(instance);

                    DescriptionList.Add(JsonConvert.SerializeObject(instance, Formatting.Indented));


                }

            }


            /// <summary>
            /// Removes value from array
            /// </summary>
            private void ExecuteRemoveFromListCommand(object obj)
            {
                ObjectList.RemoveAt(selectedItem);
                DescriptionList.RemoveAt(selectedItem);
            }

            public PropertyDescription()
            {
                AddToListCommand = new RelayCommand(ExecuteAddToListCommand);
                RemoveFromListCommand = new RelayCommand(ExecuteRemoveFromListCommand, canExecute => selectedItem >= 0);
            }
        }

        
        private bool CheckIfPropertyIsNumeric(Type propType)
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
