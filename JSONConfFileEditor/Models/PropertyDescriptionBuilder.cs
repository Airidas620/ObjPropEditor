using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
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
            AllAvailableProperties = new ObservableCollection<PropertyDescription>();

            TryResolvePropertyAndAddToCollection(customConfigurationClass.GetType(), AllAvailableProperties);

        }

        /// <summary>
        /// Resolves Type properties and adds their descriptions to ObservableCollection<PropertyDescription> array
        /// </summary>
        /// <param name="type">Type for which properties will be resolved</param>
        /// <paramref name="currentDescription"> holds descriptions for type properties</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times this function was called recursively>
        public void TryResolvePropertyAndAddToCollection(Type type, ObservableCollection<PropertyDescription> currentDescription, int depth = 0)
        {
            var props = type.GetProperties().ToList();

            foreach (var prop in props)
            {
                //Enum
                if (prop.PropertyType.IsEnum)
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(prop.PropertyType) });
                    continue;
                }

                //Numeric
                if (CheckIfPropertyIsNumeric(prop.PropertyType))
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                    continue;
                }

                //String
                if (prop.PropertyType == typeof(string))
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.String });
                    continue;
                }

                //Bool
                if (prop.PropertyType == typeof(bool))
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Bool });
                    continue;
                }


                //List
                if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))

                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ListLine});
                    currentDescription.Add(new PropertyDescription() { PropertyName = "List", NestDepth = depth, PropertyType = prop.PropertyType, listPropertyDescriptions = new ObservableCollection<PropertyDescription>(), DescriptionList = new ObservableCollection<string>(), GeneralProperty = PossibleTypes.List});

                    //Function to resolve List<T> Type T properties
                    TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), currentDescription.Last(), depth);

                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ListLine });
                    continue;
                }

                //Class
                if (prop.PropertyType.IsClass)
                {
                    var increasedDepth = depth + 40;

                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.Class });

                    //Since prop is class object it can be resolved and be linearly added to same ObservableCollection<PropertyDescription> array
                    TryResolvePropertyAndAddToCollection(prop.PropertyType, currentDescription, increasedDepth);

                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
                }

            }

        }

        /// <summary>
        /// Resolves List<T> Type t properties and adds their descriptions to ObservableCollection<PropertyDescription> array
        /// </summary>
        /// <param name="listType">Type for which properties will be resolved</param>
        /// <paramref name="listPropDes">Property descriptions will be stored in List parents PropertyDescription.listPropertyDescriptions property</paramref>/> 
        /// <paramref name="depth"/ Propotional to how many times TryResolvePropertyAndAddToCollection() was called recursively>
        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes, int depth)
        {

            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.EnumList = new List<Enum>();
                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listPropDes.PropertyName, NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.DoubleList = new List<double>();
                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.StringList = new List<string>();
                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.String });
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.BoolList = new List<bool>();
                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Bool });
                return;
            }

            //List TODO
            if (listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))

            {

                listPropDes.ListProperty = PossibleTypes.List;

                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ListLine });
                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "List", NestDepth = depth, PropertyType = listType, listPropertyDescriptions = new ObservableCollection<PropertyDescription>(), DescriptionList = new ObservableCollection<string>(), GeneralProperty = PossibleTypes.List });

                TryResolveListAndAddToCollection(listType.GenericTypeArguments.First(), listPropDes.listPropertyDescriptions.Last(), depth);

                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ListLine });


                return;
            }


            //Class
            if (listType.IsClass)
            {
                var increasedDepth = depth + 40;

                listPropDes.ListProperty = PossibleTypes.Class;

                listPropDes.ObjectList = new List<object>();
                listPropDes.ListObjectType = listType;

                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view

                //Since List<T> holds class properties they need to be resolved
                TryResolvePropertyAndAddToCollection(listType, listPropDes.listPropertyDescriptions, increasedDepth);

                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
            }

        }



        public class PropertyDescription
        {
            #region ListProperties

            /// <summary>
            /// <ObservableCollection<PropertyDescription> list to hold List<T> property properties
            /// </summary>
            public ObservableCollection<PropertyDescription> listPropertyDescriptions { get; set; }

            public ObservableCollection<PropertyDescription> ListPropertyDescriptions
            {
                get { return new ObservableCollection<PropertyDescription>(listPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.Class)); }
                set { listPropertyDescriptions = value; }
            }

            /// <summary>
            /// Property to describe added list item for WPF ListBox
            /// </summary>
            public ObservableCollection<string> DescriptionList { get; set; }

            /// <summary>
            /// List to hold List<string> values
            /// </summary>
            public List<string> StringList { get; set; }

            /// <summary>
            /// List to hold List<double> values
            /// </summary>
            public List<double> DoubleList { get; set; }

            /// <summary>
            /// List to hold List<bool> values
            /// </summary>
            public List<bool> BoolList { get; set; }

            /// <summary>
            /// List to hold List<enum> values
            /// </summary>
            public List<Enum> EnumList { get; set; }

            /// <summary>
            /// List to hold List<object> values
            /// </summary>
            public List<Object> ObjectList { get; set; }

            /// <summary>
            /// Objects List<T> list Type T resolved at runtime
            /// </summary>
            public Type ListObjectType { get; set; }
            #endregion

            #region ObjectProperties
            //Properties used for single Type T properties

            public Type PropertyType { get; set; }

            public string PropertyName { get; set; }

            //Properties to hold values for different properties Type T types
            public double ValueAsDouble { get; set; }

            public string ValueAsString { get; set; } = "";

            public bool ValueAsBool { get; set; }

            public Enum ValueAsEnum { get; set; }

            public Array AvailableEnumValues { get; set; }

            public PossibleTypes GeneralProperty { get; set; }

            public PossibleTypes ListProperty { get; set; }

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
                    if (ListPropertyDescriptions.Last().ValueAsString != null)
                    {
                        //Since single property list only have one property description
                        //Last() can be replaced with First() or [0]
                        StringList.Add(ListPropertyDescriptions.Last().ValueAsString);
                        DescriptionList.Add(StringList.Last());
                    }
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    DoubleList.Add(ListPropertyDescriptions.Last().ValueAsDouble);
                    DescriptionList.Add(DoubleList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    BoolList.Add(ListPropertyDescriptions.Last().ValueAsBool);
                    DescriptionList.Add(BoolList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    EnumList.Add(ListPropertyDescriptions.Last().ValueAsEnum);
                    DescriptionList.Add(EnumList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    //Create object of List<T> Type T 
                    Object instance = Activator.CreateInstance(ListObjectType);

                    int propDesIndex = 0;

                    //Assing values to Object from GUI 
                    SetObjectValuesWithPropertyDescription(instance, listPropertyDescriptions, ref propDesIndex);

                    //Add to List
                    ObjectList.Add(instance);

                    //TODO replace with serialize
                    DescriptionList.Add(instance.ToString());


                }

                //TODO
                if (ListProperty == PossibleTypes.List)
                {
                    //list<Lsit<Object>>
                    //Patikrinama ar ilgis nelygus nuliui
                }

            }

            /// <summary>
            /// Removes value from array
            /// </summary>
            /// <param name="ilist">List implements IList interface which has RemoveAt</param>
            /// <param name="index">Value index to remove</param>
            private void RemoveElement(IList ilist, int index)
            {
                ilist.RemoveAt(index);
                DescriptionList.RemoveAt(index);
            }

            private void ExecuteRemoveFromListCommand(object obj)
            {

                if (ListProperty == PossibleTypes.String)
                {
                    RemoveElement(StringList, selectedItem);
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    RemoveElement(DoubleList, selectedItem);
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    RemoveElement(BoolList, selectedItem);
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    RemoveElement(EnumList, selectedItem);
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    RemoveElement(ObjectList, selectedItem);
                }

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
