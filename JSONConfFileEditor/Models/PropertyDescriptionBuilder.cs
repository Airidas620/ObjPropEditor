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

        int recursiveDepth = 0;
        int maxDepth = 100;

        public void TryResolvePropertyAndAddToCollection(Type type, ObservableCollection<PropertyDescription> currentDescription, int depth = 0)
        {

            recursiveDepth++;

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
                    currentDescription.Add(new PropertyDescription(){PropertyName = "List", NestDepth = depth, PropertyType = prop.PropertyType, listPropertyDescriptions = new ObservableCollection<PropertyDescription>(), DescriptionList = new ObservableCollection<string>(), GeneralProperty = PossibleTypes.List});

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

                    TryResolvePropertyAndAddToCollection(prop.PropertyType, currentDescription, increasedDepth);

                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
                }

            }

        }

        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes, int depth)
        {
            //Enum
            if (listType.IsEnum)
            {
                listPropDes.ListProperty = PossibleTypes.Enum;
                listPropDes.EnumList = new List<Enum>();
                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "Enum", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(listType) });
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

            //List
            if (listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))

            {

                listPropDes.ListProperty = PossibleTypes.List;
                listPropDes.ObjectList = new List<object>();

                listPropDes.listPropertyDescriptions.Add(new PropertyDescription(){listPropertyDescriptions = new ObservableCollection<PropertyDescription>(), PropertyName = "List", NestDepth = listPropDes.NestDepth, 
                    PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.List });

                /*listPropDes.ListPropertyDescriptions.Add(new PropertyDescription()
                {
                    PropertyName = "List0",
                    NestDepth = listPropDes.NestDepth,
                    ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                    GeneralProperty = PossibleTypes.List
                });*/

                TryResolveListAndAddToCollection(listType.GenericTypeArguments.First(), listPropDes.listPropertyDescriptions.Last(), depth);

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

                TryResolvePropertyAndAddToCollection(listType, listPropDes.listPropertyDescriptions, increasedDepth);

                listPropDes.listPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view


            }

        }



        public class PropertyDescription
        {
            #region ListProperties
            public ObservableCollection<string> DescriptionList { get; set; }

            public List<string> StringList { get; set; }

            public List<double> DoubleList { get; set; }

            public List<bool> BoolList { get; set; }

            public List<Enum> EnumList { get; set; }

            public List<Object> ObjectList { get; set; }

            public Type ListObjectType { get; set; }
            #endregion

            #region ObjectProperties
            public Type PropertyType { get; set; }

            public string PropertyName { get; set; }

            public double ValueAsDouble { get; set; }

            public string ValueAsString { get; set; }

            public bool ValueAsBool { get; set; }

            public Enum ValueAsEnum { get; set; }

            public Array AvailableEnumValues { get; set; }

            public PossibleTypes GeneralProperty { get; set; }

            public PossibleTypes ListProperty { get; set; }

            public int NestDepth { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
            public RelayCommand AddToListCommand { set; get; }

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
                        StringList.Add(ListPropertyDescriptions.Last().ValueAsString);
                        DescriptionList.Add(StringList.Last());

                    }
                }

                if (ListProperty == PossibleTypes.Numeric)
                {
                    DoubleList.Add(ListPropertyDescriptions.First().ValueAsDouble);
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
                    Object instance = Activator.CreateInstance(ListObjectType);

                    int propDesIndex = 0;

                    SetObjectValuesWithPropertyDescription(instance, listPropertyDescriptions, ref propDesIndex);

                    ObjectList.Add(instance);
                    DescriptionList.Add(instance.ToString());


                }

                /*if (ListProperty == PossibleTypes.List)
                {

                }*/

            }

            private void RemoveElement(IList list, int index)
            {
                list.RemoveAt(index);
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



            public ObservableCollection<PropertyDescription> listPropertyDescriptions { get; set; }

            public ObservableCollection<PropertyDescription> ListPropertyDescriptions
            {
                get { return new ObservableCollection<PropertyDescription>(listPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.Class)); }
                set { listPropertyDescriptions = value; }
            }

           
        }

        public static void SetObjectValuesWithPropertyDescription(Object src, ObservableCollection<PropertyDescription> propertyDescriptions, ref int propDesIndex)
        {
            var props = src.GetType().GetProperties().ToList();

            PropertyDescription propertyDescription;



            foreach (var prop in props)
            {
                
                while (propertyDescriptions.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ObjectLine || 
                    propertyDescriptions.ElementAt(propDesIndex).GeneralProperty == PossibleTypes.ListLine)
                {
                    propDesIndex++;
                }
                propertyDescription = propertyDescriptions.ElementAt(propDesIndex);

                propDesIndex++;


                if (propertyDescription.GeneralProperty == PossibleTypes.Enum)
                {
                    prop.SetValue(src, propertyDescription.ValueAsEnum);
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.String)
                {
                    prop.SetValue(src, propertyDescription.ValueAsString);
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Bool)
                {

                    prop.SetValue(src, propertyDescription.ValueAsBool);
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Numeric)
                {

                    string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();
                    SetNumericProp(prop, prop.PropertyType, ValueAsDoubleString);

                    continue;
                }


                if (propertyDescription.GeneralProperty == PossibleTypes.List)
                {

                    if (propertyDescription.ListProperty == PossibleTypes.String)
                    {
                        prop.SetValue(src, propertyDescription.StringList);
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Numeric)
                    {
                        string ValueAsDoubleString = propertyDescription.ValueAsDouble.ToString();
                        Console.WriteLine(ValueAsDoubleString);

                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.DoubleList.Count());

                        for (int i = 0; i < values.Length; i++)
                        {
                            ValueAsDoubleString = propertyDescription.DoubleList[i].ToString();
                            values.SetValue(SetNumericProp(null, prop.PropertyType.GetGenericArguments().First(), ValueAsDoubleString, true), i);
                        }


                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));
                        //SetNumericProp(prop, ValueAsDoubleString);
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Bool)
                    {
                        prop.SetValue(src, propertyDescription.BoolList);
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Enum)
                    {

                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.EnumList.Count());

                        for (int i = 0; i < values.Length; i++)
                        {
                            values.SetValue(propertyDescription.EnumList[i], i);
                        }

                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));
                        continue;
                    }

                    if (propertyDescription.ListProperty == PossibleTypes.Class)
                    {
                        Array values = Array.CreateInstance(prop.PropertyType.GetGenericArguments().First(), propertyDescription.ObjectList.Count());
                        
                        for (int i = 0; i < values.Length; i++)
                        {
                            values.SetValue(propertyDescription.ObjectList[i], i);
                        }
                        
                        prop.SetValue(src, Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.PropertyType.GetGenericArguments().First()), new object[] { values }));


                    }
                    continue;
                }

                if (propertyDescription.GeneralProperty == PossibleTypes.Class)
                {


                    if (prop.GetValue(src) == null)
                    {
                        prop.SetValue(src, Activator.CreateInstance(prop.PropertyType));
                        Console.WriteLine("xd");
                    }
                    SetObjectValuesWithPropertyDescription(prop.GetValue(src), propertyDescriptions, ref propDesIndex);
                }

            }

            Object SetNumericProp(PropertyInfo prop,Type type ,string ValueAsDoubleString, bool setValue = false)
            {

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                        byte byteNumber;
                        if (Byte.TryParse(ValueAsDoubleString, out byteNumber))
                        {
                            prop.SetValue(src, byteNumber);
                        }
                        break;

                    case TypeCode.Decimal:
                        decimal decimalNumber;
                        if (Decimal.TryParse(ValueAsDoubleString, out decimalNumber))
                        {
                            prop.SetValue(src, decimalNumber);
                        }
                        break;

                    case TypeCode.Double:
                        double doubleNumber;
                        if (Double.TryParse(ValueAsDoubleString, out doubleNumber))
                        {
                            if (setValue)
                                return doubleNumber;
                            prop.SetValue(src, doubleNumber);
                        }
                        break;

                    case TypeCode.Int16:
                        Int16 int16Number;
                        if (Int16.TryParse(ValueAsDoubleString, out int16Number))
                        {
                            prop.SetValue(src, int16Number);
                        }
                        break;

                    case TypeCode.Int32:
                        Int32 int32Number;
                        if (Int32.TryParse(ValueAsDoubleString, out int32Number))
                        {
                            if (setValue)
                                return int32Number;
                            prop.SetValue(src, int32Number);
                        }
                        break;

                    case TypeCode.Int64:
                        Int64 int64Number;
                        if (Int64.TryParse(ValueAsDoubleString, out int64Number))
                        {
                            prop.SetValue(src, int64Number);
                        }
                        break;

                    case TypeCode.SByte:
                        sbyte sbyteNumber;
                        if (sbyte.TryParse(ValueAsDoubleString, out sbyteNumber))
                        {
                            prop.SetValue(src, sbyteNumber);
                        }
                        break;

                    case TypeCode.Single:
                        Single SingleNumber;
                        if (Single.TryParse(ValueAsDoubleString, out SingleNumber))
                        {
                            prop.SetValue(src, SingleNumber);
                        }
                        break;

                    case TypeCode.UInt16:
                        UInt16 uInt16Number;
                        if (UInt16.TryParse(ValueAsDoubleString, out uInt16Number))
                        {
                            prop.SetValue(src, uInt16Number);
                        }
                        break;

                    case TypeCode.UInt32:
                        UInt32 uInt32Number;
                        if (UInt32.TryParse(ValueAsDoubleString, out uInt32Number))
                        {
                            prop.SetValue(src, uInt32Number);
                        }
                        break;

                    case TypeCode.UInt64:
                        UInt64 uInt64Number;
                        if (UInt64.TryParse(ValueAsDoubleString, out uInt64Number))
                        {
                            prop.SetValue(src, uInt64Number);
                        }
                        break;
                }
                return null;
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
