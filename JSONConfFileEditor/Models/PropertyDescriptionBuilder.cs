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

            //Collection that will be returned
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
                        PropertyType = prop.PropertyType,
                        InnerPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                        InnerPropertyDescriptionList = new ObservableCollection<PropertyDescription>(),
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

                    var innerCollection = reccursiveProperty.InnerPropertyDescriptions;

                    foreach (var item in resolvedProps)
                    {
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

            //List TODO
            /*if (listPropDes.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
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
            }*/

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
                    innerCollection.Add(item);
                }
            }

        }



        public class PropertyDescription : INotifyPropertyChanged
        {
            
            public PropertyDescription CurrentProperty { get { return this; } private set { } }

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
            /// // pakeisiti i ObservableCollection<PropertyDescription>
            private ObservableCollection<PropertyDescription> innerPropertyDescriptionList;

            public ObservableCollection<PropertyDescription> InnerPropertyDescriptionList
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

            public  ObservableCollection<PropertyDescription> parentInnerPropertyDescriptionList;

            int ListIndex;

            /// <summary>
            /// List for holding List<{string,bool,double,enum,object}> values 
            /// </summary>
            public List<Object> ObjectList { get; set; }

            /// <summary>
            /// Objects List<T> list Type T resolved at runtime
            /// </summary>
            public Type ListObjectType { get; set; }

            #endregion


            #region ObjectProperties

            //Properties used for single properties

            /// <summary>
            /// Property Type
            /// </summary>
            public Type PropertyType { get; set; }

            /// <summary>
            /// Property name
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
            /// Command for removing values from List<>
            /// </summary>
            public RelayCommand RemoveFromListCommand { set; get; }

            /// <summary>
            /// Command for dublicating List<> item
            /// </summary>
            public RelayCommand DublicateListItemCommand { set; get; }


            private void ExecuteDublicateListItemCommand(Object obj)
            {
                var listItem = parentInnerPropertyDescriptionList.ElementAt(ListIndex);

                parentInnerPropertyDescriptionList.Add(
                
                    (PropertyDescription)listItem.Clone(true)
                );

                parentInnerPropertyDescriptionList.Last().parentInnerPropertyDescriptionList = parentInnerPropertyDescriptionList;
                parentInnerPropertyDescriptionList.Last().ListIndex = parentInnerPropertyDescriptionList.Count - 1;
            }

            private void ExecuteAddToListCommand(object obj)
            {

                //visada bus vienas, jei string... vienas ir object vienas jo vertes issaugojamos inner.inner
                //var clonedList = new List<PropertyDescription>() { (PropertyDescription)innerPropertyDescriptions.First().Clone() };

                InnerPropertyDescriptionList.Add(
                    (PropertyDescription)innerPropertyDescriptions.First().Clone()
                );
                Console.Write(InnerPropertyDescriptionList.Count);

                InnerPropertyDescriptionList.Last().parentInnerPropertyDescriptionList = InnerPropertyDescriptionList;
                InnerPropertyDescriptionList.Last().ListIndex = InnerPropertyDescriptionList.Count - 1;
            }

            private void ExecuteRemoveFromListCommand(object obj)
            {
                parentInnerPropertyDescriptionList.RemoveAt(ListIndex);

                for (int i = ListIndex; i < parentInnerPropertyDescriptionList.Count; i++)
                {
                    parentInnerPropertyDescriptionList[i].ListIndex = i;
                }
            }

            public PropertyDescription()
            {
                AddToListCommand = new RelayCommand(ExecuteAddToListCommand);
                RemoveFromListCommand = new RelayCommand(ExecuteRemoveFromListCommand);
                DublicateListItemCommand = new RelayCommand(ExecuteDublicateListItemCommand);
            }

            public void SaveGUIListDataToList()
            {
                ObjectList = new List<object>();

                if (ListProperty == PossibleTypes.String)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.ValueAsString);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.ValueAsBool);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.ValueAsDouble);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        ObjectList.Add(propertyList.ValueAsEnum);
                    }
                    return;
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    foreach (var propertyList in innerPropertyDescriptionList)
                    {
                        Object instance = Activator.CreateInstance(ListObjectType);

                        //Assing values to Object from GUI 
                        SetObjectValuesWithPropertyDescription(instance, propertyList.innerPropertyDescriptions);

                        //Add to List
                        ObjectList.Add(instance);
                    }
                }
            }

            public object Clone(bool CloneGuiValues = false)
            {
                var propDescriptionCopy = new PropertyDescription() { GeneralProperty = this.GeneralProperty, PropertyName = this.PropertyName, PropertyType = this.PropertyType, NestDepth = this.NestDepth };

                if (CloneGuiValues)
                {
                    propDescriptionCopy.ListProperty = this.ListProperty;
                    propDescriptionCopy.ListObjectType = this.ListObjectType;
                    propDescriptionCopy.ValueAsBool = this.ValueAsBool;
                    propDescriptionCopy.ValueAsString = this.ValueAsString;
                    propDescriptionCopy.ValueAsEnum = this.ValueAsEnum;
                    propDescriptionCopy.ValueAsDouble = this.ValueAsDouble;

                    propDescriptionCopy.AvailableEnumValues = this.AvailableEnumValues;


                    //propDescriptionCopy.parentInnerPropertyDescriptionList = this.parentInnerPropertyDescriptionList;


                    if (this.innerPropertyDescriptionList != null)
                    {
                        propDescriptionCopy.innerPropertyDescriptionList = new ObservableCollection<PropertyDescription>();
                        //foreach (var item in this.innerPropertyDescriptionList)
                        //{
                        var copied = new ObservableCollection<PropertyDescription>();
                            foreach (var item in this.innerPropertyDescriptionList)
                            {
                                propDescriptionCopy.innerPropertyDescriptionList.Add((PropertyDescription)item.Clone(true));

                                propDescriptionCopy.InnerPropertyDescriptionList.Last().parentInnerPropertyDescriptionList = propDescriptionCopy.InnerPropertyDescriptionList;

                                propDescriptionCopy.InnerPropertyDescriptionList.Last().ListIndex = propDescriptionCopy.InnerPropertyDescriptionList.Count - 1;
                            }
                            //propDescriptionCopy.innerPropertyDescriptionList.Add(copied);
                            
                        //}
                    }

                    if (this.innerPropertyDescriptions != null)
                    {
                        propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                        foreach (var item in this.innerPropertyDescriptions)
                        {
                            propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)item.Clone(true));
                            //propDescriptionCopy.InnerPropertyDescriptionList.Last().parentInnerPropertyDescriptionList = InnerPropertyDescriptionList;
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
                    //jei list sukuri nauja ObservableCollection<PropertyDescription>(), nes ten bus reference type, naujas dvigubas masyvas, nes ten bus vertes
                    propDescriptionCopy.innerPropertyDescriptions = new ObservableCollection<PropertyDescription>();
                    propDescriptionCopy.innerPropertyDescriptionList = new ObservableCollection<PropertyDescription>();
                    propDescriptionCopy.ListProperty = this.ListProperty;
                    propDescriptionCopy.ListObjectType = this.ListObjectType;

                    propDescriptionCopy.innerPropertyDescriptions.Add((PropertyDescription)this.innerPropertyDescriptions.First().Clone());
                }

                if (this.GeneralProperty == PossibleTypes.Class)
                {
                    //jei klase nukupijuojamos vidines prop nes ten viskas yra
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
