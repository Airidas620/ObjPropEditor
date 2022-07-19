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
                    currentDescription.Add(new PropertyDescription() { PropertyName = "List", NestDepth = depth, PropertyType = prop.PropertyType, ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(), DescriptionList = new ObservableCollection<string>(), GeneralProperty = PossibleTypes.List});

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
        /// <paramref name="depth"/ Propotional to how many times this and TryResolvePropertyAndAddToCollection functions were called recursively>
        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes, int depth)
        {

            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.EnumList = new List<Object>();
                listPropDes.ObjectList = new List<Object>();
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listPropDes.PropertyName, NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });
                return;
            }

            //Numeric
            if (CheckIfPropertyIsNumeric(listType))
            {
                listPropDes.ListProperty = PossibleTypes.Numeric;
                listPropDes.DoubleList = new List<Object>();
                listPropDes.ObjectList = new List<Object>();
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Numeric", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                return;
            }

            //String
            if (listType == typeof(string))
            {
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.StringList = new List<string>();
                listPropDes.ObjectList = new List<Object>();
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.String });
                return;
            }

            //Bool
            if (listType == typeof(bool))
            {
                listPropDes.ListProperty = PossibleTypes.Bool;
                listPropDes.BoolList = new List<Object>();
                listPropDes.ObjectList = new List<Object>();
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Bool", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Bool });
                return;
            }

            //List TODO
            if (listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))

            {

                var increasedDepth = depth + 40;

                listPropDes.ListProperty = PossibleTypes.List;
                listPropDes.ObjectList = new List<Object>();


                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.ListLine });
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "List", NestDepth = increasedDepth, PropertyType = listType, ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(), DescriptionList = new ObservableCollection<string>(), GeneralProperty = PossibleTypes.List });

                TryResolveListAndAddToCollection(listType.GenericTypeArguments.First(), listPropDes.ListPropertyDescriptions.Last(), increasedDepth);

                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = increasedDepth, GeneralProperty = PossibleTypes.ListLine });


                return;
            }


            //Class
            if (listType.IsClass)
            {
                var increasedDepth = depth + 40;

                listPropDes.ListProperty = PossibleTypes.Class;

                listPropDes.ObjectList = new List<object>();
                listPropDes.ListObjectType = listType;

                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view

                //Since List<T> holds class properties they need to be resolved
                TryResolvePropertyAndAddToCollection(listType, listPropDes.ListPropertyDescriptions, increasedDepth);

                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
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

            public ObservableCollection<PropertyDescription> ListPropertyDescriptionsGraph
            {
                get { return new ObservableCollection<PropertyDescription>(listPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.Class)); }
                set { listPropertyDescriptions = value; }
            }

            public ObservableCollection<PropertyDescription> ListPropertyDescriptionsGUI
            {
                get { return new ObservableCollection<PropertyDescription>(listPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.ObjectLine && prop.GeneralProperty != PossibleTypes.ListLine)); }
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
            public List<Object> DoubleList { get; set; }

            /// <summary>
            /// List to hold List<bool> values
            /// </summary>
            public List<Object> BoolList { get; set; }

            /// <summary>
            /// List to hold List<enum> values
            /// </summary>
            public List<Object> EnumList { get; set; }

            /// <summary>
            /// List to hold List<object> values
            /// </summary>
            public List<Object> ObjectList { get; set; }

            //public List<Object> ListOfList { get; set; }

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
                //Console.WriteLine(ListProperty);

                if (ListProperty == PossibleTypes.String)
                {

                    //Since single property list only have one property description
                    //Last() can be replaced with First() or [0]

                    StringList.Add(listPropertyDescriptions.Last().ValueAsString);

                    if (listPropertyDescriptions.Last().ValueAsString != "")
                        DescriptionList.Add(StringList.Last().ToString());

                    else
                        DescriptionList.Add("\"\"");
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    ObjectList.Add(listPropertyDescriptions.Last().ValueAsDouble);
                    DescriptionList.Add(ObjectList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Bool)
                {
                    ObjectList.Add(listPropertyDescriptions.Last().ValueAsBool);
                    DescriptionList.Add(ObjectList.Last().ToString());
                }

                if (ListProperty == PossibleTypes.Enum)
                {
                    if(listPropertyDescriptions.Last().ValueAsEnum != null)
                    {
                        EnumList.Add(listPropertyDescriptions.Last().ValueAsEnum);
                        DescriptionList.Add(EnumList.Last().ToString());
                    }
                }

                if (ListProperty == PossibleTypes.Class)
                {
                    //Create object of List<T> Type T 
                    Object instance = Activator.CreateInstance(ListObjectType);

                    int propDesIndex = 0;

                    //Assing values to Object from GUI 
                    SetObjectValuesWithPropertyDescription(instance, ListPropertyDescriptionsGUI, ref propDesIndex);

                    //Add to List
                    ObjectList.Add(instance);

                    //TODO replace with serialize
                    DescriptionList.Add(JsonConvert.SerializeObject(instance, Formatting.Indented));


                }

                //TODO
                if (ListProperty == PossibleTypes.List)
                {
                    
                    PropertyDescription listDescritpion = null;

                    for (int i = 0; i < listPropertyDescriptions.Count(); i++)
                    {
                        if (listPropertyDescriptions[i].GeneralProperty == PossibleTypes.List)
                        {
                            listDescritpion = listPropertyDescriptions[i];
                        }
                    }

                    if (listDescritpion.ListProperty == PossibleTypes.String)
                    {
                        ObjectList.Add(new List<string>(listDescritpion.StringList));//stores reference masyvai laiko masyvus(nuoradas) 
                        
                        DescriptionList.Add(JsonConvert.SerializeObject(listDescritpion.StringList, Formatting.Indented));
                        Console.WriteLine(ObjectList.Count());

                        //DescriptionList.Add(ListOfList.ToString());
                    }

                    if (listDescritpion.ListProperty == PossibleTypes.List)
                    {
                        /*Console.WriteLine(listDescritpion.ListOfList[0]);
                        Console.WriteLine(listDescritpion.ListOfList.GetType());
                        Console.WriteLine(listDescritpion.ListOfList[0].GetType());*/
                        ObjectList.Add(new List<Object>(listDescritpion.ObjectList));

                        DescriptionList.Add(JsonConvert.SerializeObject(listDescritpion.ObjectList, Formatting.Indented));
                        Console.WriteLine(ObjectList.Count());

                        //DescriptionList.Add(ListOfList.ToString());
                    }

                    if (listDescritpion.ListProperty == PossibleTypes.Numeric)
                    {
                       
                    }

                    if (listDescritpion.ListProperty == PossibleTypes.Bool)
                    {
                       
                    }

                    if (listDescritpion.ListProperty == PossibleTypes.Enum)
                    {
                       
                    }



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
