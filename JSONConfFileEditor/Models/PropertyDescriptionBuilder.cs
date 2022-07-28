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

        Object MyCustonConfigurationClass;

        public PropertyDescriptionBuilder(Object customConfigurationClass)
        {
            MyCustonConfigurationClass = customConfigurationClass;
            //GetTypePropertyDescriptions(customConfigurationClass.GetType());

        }


        public bool BuildProperties()
        {


            try
            {
                AllAvailableProperties = GetTypePropertyDescriptions(MyCustonConfigurationClass.GetType());
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Resolves Type properties and adds their descriptions to ObservableCollection<PropertyDescription> array
        /// </summary>
        /// <param name="classType">Type for which properties will be resolved</param>
        /// <paramref name="currentDescription"> holds descriptions for type properties</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times this function was called recursively>
        public ObservableCollection<PropertyDescription> GetTypePropertyDescriptions(Type type, int depth = 0, int maxDepth = 20)
        {
            
            if(depth > maxDepth)
            {
                throw new Exception("Too many inner Object/Lists");
            }

            var availableProperties = new ObservableCollection<PropertyDescription>();
            //var currentPropertyCollection = availableProperties;

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
                        PropertyType = prop.PropertyType,
                        InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                        InnerPropertyDescriptionList = new ObservableCollection<ObservableCollection<PropertyDescription>>(),
                        DescriptionList = new ObservableCollection<string>(),
                        GeneralProperty = PossibleTypes.List
                    };
                    //Function to resolve List<T> Type T properties
                    TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), listProp, depth);
                    availableProperties.Add(listProp);

                    continue;
                }


                //Class
                if (prop.PropertyType.IsClass)
                {
                    int increasedDepth = depth + 1;

                    var reccursiveProperty = new PropertyDescription() {PropertyType = prop.PropertyType, PropertyName = prop.Name, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.Class, InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>()};
                    availableProperties.Add(reccursiveProperty);

                    var resolvedProps = GetTypePropertyDescriptions(prop.PropertyType, increasedDepth, maxDepth);

                    var innerCollection = reccursiveProperty.InnerPropertyDescriptions; // pakeist i recursive

                    foreach (var item in resolvedProps)
                    {
                        //availableProperties.Add(item);
                        innerCollection.Add(item);

                    }
                    //resolvedProps.ToList().ForEach(resolvedProp => availableProperties.Add(resolvedProp));
                }
            }

            return availableProperties;
        }


        /// <summary>
        /// Resolves List<T> Type t properties and adds their descriptions to ObservableCollection<PropertyDescription> List
        /// </summary>
        /// <param name="listType">Type for which properties will be resolved</param>
        /// <paramref name="listPropDes">Property descriptions will be stored in List parents PropertyDescription.innerPropertyDescriptions property</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times this and TryResolvePropertyAndAddToCollection functions were called recursively>
        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes, int depth = 0, int maxDepth = 20)
        {
            if (depth > maxDepth)
            {
                throw new Exception("Too many inner Object/Lists");
            }

            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listPropDes.PropertyName, NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.String });
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Bool });
                return;
            }

            //List
            if (listPropDes.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                listPropDes.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                Console.WriteLine("hi");

                var listProp = new PropertyDescription()
                {
                    PropertyName = listPropDes.PropertyName,
                    NestDepth = depth,
                    PropertyType = listPropDes.PropertyType,
                    InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                    InnerPropertyDescriptionList = new ObservableCollection<ObservableCollection<PropertyDescription>>(),
                    DescriptionList = new ObservableCollection<string>(),
                    GeneralProperty = PossibleTypes.List
                };
                //Function to resolve List<T> Type T properties
                TryResolveListAndAddToCollection(listProp.PropertyType.GenericTypeArguments.First(), listProp, depth);
                listPropDes.InnerPropertyDescriptions.Add(listProp);
                return;
            }

            //Class
            if (listType.IsClass)
            {
                listPropDes.ListProperty = PossibleTypes.Class;
                listPropDes.ListObjectType = listType;

                var increasedDepth = depth + 1;

                var reccursiveProperty = new PropertyDescription() { PropertyType = listPropDes.PropertyType, PropertyName = listPropDes.PropertyName, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.Class, InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>() };
                listPropDes.InnerPropertyDescriptions.Add(reccursiveProperty);

                //Since List<T> holds class properties they need to be resolved
                var resolvedProps = GetTypePropertyDescriptions(listType, depth);

                var innerCollection = reccursiveProperty.InnerPropertyDescriptions; // pakeist i recursive

                foreach (var item in resolvedProps)
                {
                    //listPropDes.InnerPropertyDescriptions.Add(item);
                    innerCollection.Add(item);

                }

                //(Not used)listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
            }

        }



        public class PropertyDescription : INotifyPropertyChanged, ICloneable
        {
            public object Clone()
            {

                //Console.WriteLine(this.PropertyName);

                var propDescriptionCopy = new PropertyDescription() {GeneralProperty = this.GeneralProperty, PropertyName = this.PropertyName, PropertyType = this.PropertyType, NestDepth = this.NestDepth
                    };

                if(this.GeneralProperty == PossibleTypes.Enum)
                {
                    propDescriptionCopy.AvailableEnumValues = this.AvailableEnumValues;
                }

                if(this.GeneralProperty == PossibleTypes.List)
                {
                    propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                    propDescriptionCopy.InnerPropertyDescriptionList = new ObservableCollection<ObservableCollection<PropertyDescription>>();
                    propDescriptionCopy.ListProperty = this.ListProperty;
                    propDescriptionCopy.ListObjectType = this.ListObjectType;

                    propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)this.innerPropertyDescriptions.First().Clone());
                    //Console.WriteLine("hi");
                }

                if (this.GeneralProperty == PossibleTypes.Class)
                {
                    propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();

                    foreach(var item in this.innerPropertyDescriptions)
                    {
                        propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)item.Clone());
                    }
                }
                return propDescriptionCopy;

                //return null;
            }

            private void ExecuteAddToListCommand2(object obj)
            {
                //innerPropertyDescriptions


                //Console.WriteLine(innerPropertyDescriptions.Count());
                innerPropertyDescriptions.Select(objEntity => objEntity.PropertyName);

                var clonedList = innerPropertyDescriptions.Select(objEntity => (PropertyDescription)objEntity.Clone()).ToList();

                InnerPropertyDescriptionList.Add(new ObservableCollection<PropertyDescription>(clonedList));
                //innerPropertyDescriptionList.Add(new ObservableCollection<PropertyDescription>(innerPropertyDescriptions));
                //innerPropertyDescriptions.
            }


            #region ListProperties

            /// <summary>
            /// ObservableCollection<PropertyDescription> list which for Object properties it holds that objects properties and for List properties
            /// it holds reference for creating new list entries (cref)
            /// </summary>
            private ObservableCollection<PropertyDescription> innerPropertyDescriptions;

            public ObservableCollection<PropertyDescription> InnerPropertyDescriptions
            {
                get { return innerPropertyDescriptions; }
                set { 
                    if(innerPropertyDescriptions != value)
                        {
                            innerPropertyDescriptions = value;
                        }
                    }
            }

            /// <summary>
            /// 
            /// </summary>
            private ObservableCollection<ObservableCollection<PropertyDescription>> innerPropertyDescriptionList;

            public ObservableCollection<ObservableCollection<PropertyDescription>> InnerPropertyDescriptionList
            {
                get { return innerPropertyDescriptionList; }
                set
                {
                    if (innerPropertyDescriptionList != value)
                    {
                        innerPropertyDescriptionList = value;
                    }
                }
            }


            /// <summary>
            /// (Not used)
            /// </summary>
            public ObservableCollection<PropertyDescription> ListPropertyDescriptionsGraph
            {
                get { return new ObservableCollection<PropertyDescription>(innerPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.Class)); }
                set { innerPropertyDescriptions = value; }
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
            /// Holds string values if property is string
            /// </summary>
            public string ValueAsString { get; set; } = "";

            /// <summary>
            /// Holds bool values if property is bool
            /// </summary>
            public bool ValueAsBool { get; set; }

            /// <summary>
            /// Holds enum values if property is enum
            /// </summary>
            public Enum ValueAsEnum { get; set; } //TODO check for saving with reference error

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
            /// Command for adding values to List<>
            /// </summary>
            public RelayCommand AddToListCommand2 { set; get; }

            /// <summary>
            /// Command for removing values from List<>
            /// </summary>
            public RelayCommand RemoveFromListCommand { set; get; }


            /// <summary>
            /// Command for entering Edit List menu
            /// </summary>
            public RelayCommand EditListMenuCommand { set; get; }

            /// <summary>
            /// Command for canceling lists edit
            /// </summary>
            public RelayCommand EditListCancelCommand { set; get; }

            /// <summary>
            /// Command for editing to List<> values
            /// </summary>
            public RelayCommand EditListCommand { set; get; }

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

            private bool isEditing;

            public bool IsEditing
            {
                get { return isEditing; }
                set
                {
                    if (value != isEditing)
                    {
                        isEditing = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private List<int> numberOfObjects = new List<int> { 1,3,2};

            public List<int> NumberOfObjects
            {
                get { return numberOfObjects; }
                set
                {
                    if (numberOfObjects != value)
                    {
                        numberOfObjects = value;
                    }
                }
            }

         

            public void SaveGUIListDataToList()
            {
                Console.WriteLine("hi");
                ObjectList = new List<object>();
                if (ListProperty == PossibleTypes.String)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.First().ValueAsString);
                    }
                    //return;
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    Console.WriteLine("dd0");
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.First().ValueAsBool);
                        Console.WriteLine("dd");
                    }
                    //return;
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.First().ValueAsDouble);
                    }
                    //return;
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.First().ValueAsEnum);
                    }
                    //return;
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        //Console.WriteLine(ListObjectType);

                        Object instance = Activator.CreateInstance(ListObjectType);

                        int propDesIndex = 0;

                        //Assing values to Object from GUI 
                        SetObjectValuesWithPropertyDescription(instance, propertyList.First().innerPropertyDescriptions, ref propDesIndex);

                        //Add to List
                        ObjectList.Add(instance);

                    }
                }
                Console.WriteLine(ObjectList.Count());
            }

            private void ExecuteAddToListCommand(object obj)
            {

                if (ListProperty == PossibleTypes.String)
                {

                    //Since single property list only have one property description
                    //Last() can be replaced with First() or [0]
                    ObjectList.Add(innerPropertyDescriptions.Last().ValueAsString);

                    if (innerPropertyDescriptions.Last().ValueAsString != "")
                        DescriptionList.Add(ObjectList.Last().ToString());

                    else
                        DescriptionList.Add("\"\"");
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    ObjectList.Add(innerPropertyDescriptions.Last().ValueAsBool);
                    DescriptionList.Add(ObjectList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    ObjectList.Add(innerPropertyDescriptions.Last().ValueAsDouble);
                    DescriptionList.Add(ObjectList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    if(innerPropertyDescriptions.Last().ValueAsEnum != null)
                    {
                        ObjectList.Add(innerPropertyDescriptions.Last().ValueAsEnum);
                        DescriptionList.Add(ObjectList.Last().ToString());
                    }
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    //Create object of List<T> Type T 
                    Object instance = Activator.CreateInstance(ListObjectType);

                    int propDesIndex = 0;

                    //Assing values to Object from GUI 
                    SetObjectValuesWithPropertyDescription(instance, InnerPropertyDescriptions, ref propDesIndex);

                    //Add to List
                    ObjectList.Add(instance);

                    DescriptionList.Add(JsonConvert.SerializeObject(instance, Formatting.Indented));


                }

            }


            private void ExecuteRemoveFromListCommand(object obj)
            {
                ObjectList.RemoveAt(selectedItem);
                DescriptionList.RemoveAt(selectedItem);
            }

            private void ExecuteEditListMenuCommand(object obj)
            {
                int index = selectedItem;
                IsEditing = true;

            }

            private void ExecuteEditListCancelCommand(object obj)
            {
                int index = selectedItem;
                IsEditing = false;

            }

            private void ExecuteEditListCommand(object obj)
            {
                int index = selectedItem;

            }

            public PropertyDescription()
            {
                AddToListCommand = new RelayCommand(ExecuteAddToListCommand, canExecute => !IsEditing);
                AddToListCommand2 = new RelayCommand(ExecuteAddToListCommand2);
                RemoveFromListCommand = new RelayCommand(ExecuteRemoveFromListCommand, canExecute => selectedItem >= 0 && !IsEditing);
                EditListMenuCommand = new RelayCommand(ExecuteEditListMenuCommand, canExecute => !IsEditing);
                EditListCancelCommand = new RelayCommand(ExecuteEditListCancelCommand);
                EditListCommand = new RelayCommand(ExecuteEditListCommand);
            }

           
            #region INotifyPropertyChanged implementation
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
