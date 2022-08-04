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
        }


        public bool BuildProperties()
        {
            try
            {
                AllAvailableProperties = GetTypePropertyDescriptions(MyCustonConfigurationClass.GetType(), true, MyCustonConfigurationClass);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Resolves Type properties and adds returns their descriptions with ObservableCollection<PropertyDescription>
        /// </summary>
        /// <param name="type"> Type for which properties will be resolved</param>
        /// <param name="depth"> Proportional to how many times this function was called recursively>
        /// <param name="maxDepth"> Max depth allowed </param>
        public ObservableCollection<PropertyDescription> GetTypePropertyDescriptions(Type type, bool writeValue, Object src = null, int depth = 0, int maxDepth = 20)
        {

            if (depth > maxDepth)
            {
                throw new Exception("Too many inner Object/Lists");
            }

            //Collection that will be returned
            var availableProperties = new ObservableCollection<PropertyDescription>();

            if (src != null)
            {
                type = src.GetType();
            }

            var props = type.GetProperties().ToList();



            foreach (var prop in props)
            {

                //Enum
                if (prop.PropertyType.IsEnum)
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(prop.PropertyType) });

                    if (writeValue && src != null)
                    {
                        availableProperties.Last().ValueAsEnum = prop.GetValue(src);
                    }
                    continue;
                }

                //Numeric
                if (CheckIfPropertyIsNumeric(prop.PropertyType))
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Numeric });

                    if (writeValue && src != null)
                    {
                        availableProperties.Last().NumericValueAsString = prop.GetValue(src).ToString();
                    }
                    continue;
                }

                //String
                if (prop.PropertyType == typeof(string))
                {
                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.String });

                    if (writeValue && src != null)
                    {
                        if(prop.GetValue(src)!= null)
                        {
                            availableProperties.Last().ValueAsString = prop.GetValue(src).ToString();
                        }
                    }
                    continue;
                }

                //Bool
                if (prop.PropertyType == typeof(bool))
                {

                    availableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Bool });

                    if (writeValue && src != null)
                    {
                        availableProperties.Last().ValueAsBool = (bool)prop.GetValue(src);
                    }
                    continue;
                }

                //List
                if (prop.PropertyType.IsGenericType)
                {
                    if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                        prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                    {

                        var listProp = new PropertyDescription()
                        {
                            PropertyName = prop.Name,
                            NestDepth = depth,
                            PropertyType = prop.PropertyType,
                            InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                            ListItems = new ObservableCollection<PropertyDescription>(),
                            GeneralProperty = PossibleTypes.List
                        };

                        IEnumerable iErnurable = null;

                        if (src != null)
                        {
                            iErnurable = (IEnumerable)prop.GetValue(src);
                        }

                        //Function to resolve List<T> Type T properties
                        TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), listProp, iErnurable, depth);

                        availableProperties.Add(listProp);

                        continue;
                    }
                }

                //Class
                if (prop.PropertyType.IsClass)
                {

                    int increasedDepth = depth + 1;

                    var reccursiveProperty = new PropertyDescription() { PropertyType = prop.PropertyType, PropertyName = prop.Name, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.Class, InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>() };
                    availableProperties.Add(reccursiveProperty);

                    if (writeValue && prop.GetValue(src) == null)
                    {
                        prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                    }

                    var resolvedProps = writeValue ? GetTypePropertyDescriptions(prop.PropertyType, writeValue, prop.GetValue(src), increasedDepth, maxDepth) :
                        GetTypePropertyDescriptions(prop.PropertyType, writeValue, increasedDepth, maxDepth);

                    var innerCollection = reccursiveProperty.InnerPropertyDescriptions;

                    foreach (var item in resolvedProps)
                    {
                        innerCollection.Add(item);

                    }
                }

            }

            return availableProperties;
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
            if (depth > maxDepth)
            {
                throw new Exception("Too many inner Object/Lists");
            }


            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listPropDes.PropertyName, NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });

                SaveToList();
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Numeric });

                SaveToList();
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.String });

                SaveToList();
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.InnerPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Bool });

                SaveToList();
                return;
            }

            //List TODO

            //Class
            if (listType.IsClass)
            {

                listPropDes.ListProperty = PossibleTypes.Class;
                listPropDes.ListObjectType = listType;

                var increasedDepth = depth + 1;

                var reccursiveProperty = new PropertyDescription() { PropertyType = listPropDes.PropertyType, PropertyName = listPropDes.PropertyName, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.Class, InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>() };
                listPropDes.InnerPropertyDescriptions.Add(reccursiveProperty);

                //Since List<T> holds class properties they need to be resolved
                var resolvedProps = GetTypePropertyDescriptions(listType, false, null, depth);

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
                                listPropDes.ListItems.Last().InnerPropertyDescriptions = GetTypePropertyDescriptions(item.GetType(), true, item, depth);
                                break;
                        }

                        listPropDes.ListItems.Last().parentListItems = listPropDes.ListItems;
                        listPropDes.ListItems.Last().ListItemIndex = listPropDes.ListItems.Count - 1;
                    }
                }
            }

        }



        public class PropertyDescription : INotifyPropertyChanged
        {


            #region ObjectProperties

            //Properties used for describing single properties

            /// <summary>
            /// Property Type
            /// </summary>
            public Type PropertyType { get; set; }

            /// <summary>
            /// Property name
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Holds double value if property is numeric
            /// </summary>
            //public double ValueAsDouble { get; set; }

            /// <summary>
            /// Holds string value if property is string
            /// </summary>
            public string ValueAsString { get; set; } = "";


            private string numericValueAsString = "";
            /// <summary>
            /// Holds string value if property is string
            /// </summary>
            public string NumericValueAsString
            {
                get { return numericValueAsString; }
                set
                {
                    if (numericValueAsString != value)
                    {
                        try
                        {
                            if (value.Contains("e") | value.Contains("E") | value.Contains("m") | value.Contains("M"))
                            {
                                NumericParser.StringToNumericTypeValue(PropertyType, value).ToString();
                                numericValueAsString = value;
                            }
                            else
                            {
                                numericValueAsString = NumericParser.StringToNumericTypeValue(PropertyType, value).ToString();
                            }
                            IsInputValueValid = true;
                        }
                        catch (Exception)
                        {
                            numericValueAsString = "0";
                            IsInputValueValid = false;
                        }
                    }
                }
            }


            private bool isInputValueValid = true;

            /// <summary>
            /// Is the value written in GUI correctly
            /// </summary>
            public bool IsInputValueValid
            {
                get { return isInputValueValid; }
                set
                {
                    if (isInputValueValid != value)
                    {
                        isInputValueValid = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            /// <summary>
            /// Holds bool value if property is bool
            /// </summary>
            public bool ValueAsBool { get; set; }

            /// <summary>
            /// Holds enum value if property is enum
            /// </summary>
            public object ValueAsEnum { get; set; } //TODO check for saving with reference error

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

            #region ListProperties

            /// <summary>
            /// For Object properties it holds that objects properties and 
            /// for List properties it holds reference for creating new list entries.
            /// </summary>
            private ObservableCollection<PropertyDescription> innerPropertyDescriptions;

            public ObservableCollection<PropertyDescription> InnerPropertyDescriptions
            {
                get { return innerPropertyDescriptions; }
                set
                {
                    if (innerPropertyDescriptions != value)
                    {
                        innerPropertyDescriptions = value;
                    }
                }
            }

            /// <summary>
            /// Holds List items
            /// </summary>
            private ObservableCollection<PropertyDescription> listItems;

            public ObservableCollection<PropertyDescription> ListItems
            {
                get { return listItems; }
                set
                {
                    if (listItems != value)
                    {
                        listItems = value;
                    }
                }
            }

            /// <summary>
            /// Reference to items ListItems collection
            /// </summary>
            public ObservableCollection<PropertyDescription> parentListItems;

            /// <summary>
            /// Items index inside ListItems collection
            /// </summary>
            public int ListItemIndex;

            /// <summary>
            /// List for holding List<{string,bool,double,enum,object}> values 
            /// </summary>
            public List<Object> ObjectList { get; set; }

            /// <summary>
            /// List<T(Object)> Type T resolved at runtime
            /// </summary>
            public Type ListObjectType { get; set; }

            #endregion

            /// <summary>
            /// Command for adding items to List<>
            /// </summary>
            public RelayCommand AddToListCommand { set; get; }


            /// <summary>
            /// Command for removing items from List<>
            /// </summary>
            public RelayCommand RemoveFromListCommand { set; get; }

            /// <summary>
            /// Command for duplicating List<> item
            /// </summary>
            public RelayCommand DuplicateListItemCommand { set; get; }


            private void ExecuteDuplicateListItemCommand(Object obj)
            {
                var listItem = parentListItems.ElementAt(ListItemIndex);

                parentListItems.Add(

                    (PropertyDescription)listItem.Clone(true)
                );

                parentListItems.Last().parentListItems = parentListItems;
                parentListItems.Last().ListItemIndex = parentListItems.Count - 1;
            }

            private void ExecuteAddToListCommand(object obj)
            {


                ListItems.Add(
                    (PropertyDescription)innerPropertyDescriptions.First().Clone()
                );

                ListItems.Last().parentListItems = ListItems;
                ListItems.Last().ListItemIndex = ListItems.Count - 1;
            }

            private void ExecuteRemoveFromListCommand(object obj)
            {
                parentListItems.RemoveAt(ListItemIndex);

                for (int i = ListItemIndex; i < parentListItems.Count; i++)
                {
                    parentListItems[i].ListItemIndex = i;
                }
            }

            public PropertyDescription()
            {
                AddToListCommand = new RelayCommand(ExecuteAddToListCommand);
                RemoveFromListCommand = new RelayCommand(ExecuteRemoveFromListCommand);
                DuplicateListItemCommand = new RelayCommand(ExecuteDuplicateListItemCommand);
            }


            /// <summary>
            /// Saves GUI items data to single list
            /// </summary>
            public void SaveGUIListDataToList()
            {
                ObjectList = new List<object>();

                if (ListProperty == PossibleTypes.String)
                {
                    foreach (var propertyList in listItems)
                    {
                        ObjectList.Add(propertyList.ValueAsString);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    foreach (var propertyList in listItems)
                    {
                        ObjectList.Add(propertyList.ValueAsBool);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    foreach (var propertyList in listItems)
                    {
                        ObjectList.Add(propertyList.NumericValueAsString);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    foreach (var propertyList in listItems)
                    {
                        ObjectList.Add(propertyList.ValueAsEnum);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    foreach (var propertyList in listItems)
                    {
                        Object instance = Activator.CreateInstance(ListObjectType);

                        //Assing values to Object from GUI 
                        SetObjectValuesWithPropertyDescription(instance, propertyList.innerPropertyDescriptions);

                        //Add to List
                        ObjectList.Add(instance);
                    }
                }
            }

            /// <summary>
            /// Clones current object
            /// <paramref name="CloneGuiValues"/> Copies current object values
            /// </summary>
            /// <returns></returns>
            public object Clone(bool CloneGuiValues = false)
            {
                var propDescriptionCopy = new PropertyDescription() { GeneralProperty = this.GeneralProperty, PropertyName = this.PropertyName, PropertyType = this.PropertyType, NestDepth = this.NestDepth };

                if (CloneGuiValues)
                {
                    propDescriptionCopy.ListProperty = this.ListProperty;
                    propDescriptionCopy.ListObjectType = this.ListObjectType;
                    propDescriptionCopy.ValueAsBool = this.ValueAsBool;
                    propDescriptionCopy.ValueAsString = this.ValueAsString;
                    propDescriptionCopy.NumericValueAsString = this.NumericValueAsString;
                    propDescriptionCopy.ValueAsEnum = this.ValueAsEnum;

                    propDescriptionCopy.AvailableEnumValues = this.AvailableEnumValues;


                    if (this.listItems != null)
                    {
                        propDescriptionCopy.listItems = new ObservableCollection<PropertyDescription>();

                        foreach (var item in this.listItems)
                        {
                            propDescriptionCopy.listItems.Add((PropertyDescription)item.Clone(true));

                            propDescriptionCopy.ListItems.Last().parentListItems = propDescriptionCopy.ListItems;

                            propDescriptionCopy.ListItems.Last().ListItemIndex = propDescriptionCopy.ListItems.Count - 1;
                        }

                    }

                    if (this.innerPropertyDescriptions != null)
                    {
                        propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                        foreach (var item in this.innerPropertyDescriptions)
                        {
                            propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)item.Clone(true));
                        }
                    }
                    return propDescriptionCopy;
                }


                if (this.GeneralProperty == PossibleTypes.Enum)
                {
                    propDescriptionCopy.AvailableEnumValues = this.AvailableEnumValues;
                }

                if (this.GeneralProperty == PossibleTypes.List)
                {

                    propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                    propDescriptionCopy.listItems = new ObservableCollection<PropertyDescription>();

                    propDescriptionCopy.ListProperty = this.ListProperty;
                    propDescriptionCopy.ListObjectType = this.ListObjectType;

                    propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)this.innerPropertyDescriptions.First().Clone());
                }

                if (this.GeneralProperty == PossibleTypes.Class)
                {
                    propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();

                    foreach (var item in this.innerPropertyDescriptions)
                    {
                        propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)item.Clone());
                    }
                }

                return propDescriptionCopy;
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
