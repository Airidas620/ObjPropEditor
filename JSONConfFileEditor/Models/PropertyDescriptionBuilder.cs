using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
using System;
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
    public class PropertyDescriptionBuilder
    {

        public string buttonText { get; set; } = "hii";


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
            //Type type = src.GetType();

            recursiveDepth++;

            //var fields = type.GetFields().ToList();     

            var props = type.GetProperties().ToList();

            foreach (var prop in props)
            {


                if (prop.PropertyType.IsEnum)
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(prop.PropertyType) });
                    continue;
                }

                if (CheckIfPropertyIsNumeric(prop))
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.String });
                    continue;
                }


                if (prop.PropertyType == typeof(bool))
                {
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Bool });
                    continue;
                }

                //If proporety was unresolved for some reason, it might go to recursion for the wrong reasons
                //null array

                //Console.WriteLine(prop.PropertyType.GetInterfaces().);


                /*foreach (var item in prop.PropertyType.GetInterfaces())
                {
                    Console.WriteLine(item);
                }*/


                {
                    /*
                     * add list method turi buti klases patirkinimas (kartu ir ar list, kas bus jei objektas(adini i description vertes?)) ir paskui setinama generalList type

                     1. padaryti su string
                     parasymas add ir delete
                     issaugojimas i man obj
                     string list string liste

                     2. padaryti su kitais tipais
                     3. Objektas

                     */

                }

                if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)) &&
                    prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))

                {

                    currentDescription.Add(new PropertyDescription()
                    {
                        PropertyName = "List0",
                        NestDepth = depth,
                        PropertyType = prop.PropertyType,
                        ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                        GeneralProperty = PossibleTypes.List
                    });
                    TryResolveListAndAddToCollection(prop.PropertyType.GenericTypeArguments.First(), currentDescription.Last());
                    continue;
                }


                //if (prop.GetValue(src) != null)
                {
                    var increasedDepth = depth + 40;
                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
                    currentDescription.Add(new PropertyDescription() { GeneralProperty = PossibleTypes.Class });

                    //Console.WriteLine(prop.PropertyType.IsClass);

                    TryResolvePropertyAndAddToCollection(prop.PropertyType, currentDescription, increasedDepth);

                    currentDescription.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
                }
                /*else
                {
                    currentDescription.Add(new PropertyDescription() { GeneralProperty = PossibleTypes.Null });

                }*/


            }

        }

        private void TryResolveListAndAddToCollection(Type listType, PropertyDescription listPropDes)
        {
            //Console.WriteLine(listType);
            if (listType == typeof(string))
            {
                Console.WriteLine(listPropDes);
                listPropDes.ListProperty = PossibleTypes.String;
                listPropDes.StringList = new ObservableCollection<string>();
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = "String", NestDepth = listPropDes.NestDepth, PropertyType = listPropDes.PropertyType, GeneralProperty = PossibleTypes.String });
                return;
            }


            if (listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                listType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))

            {
                Console.WriteLine("asf");

                listPropDes.ListProperty = PossibleTypes.List;
                listPropDes.ListPropertyDescriptions.Add(new PropertyDescription()
                {
                    ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                    PropertyName = "LIst",
                    NestDepth = listPropDes.NestDepth,
                    PropertyType = listPropDes.PropertyType,
                    GeneralProperty = PossibleTypes.List
                }
                );

                /*listPropDes.ListPropertyDescriptions.Add(new PropertyDescription()
                {
                    PropertyName = "List0",
                    NestDepth = listPropDes.NestDepth,
                    ListPropertyDescriptions = new ObservableCollection<PropertyDescription>(),
                    GeneralProperty = PossibleTypes.List
                });*/
                TryResolveListAndAddToCollection(listType.GenericTypeArguments.First(), listPropDes.ListPropertyDescriptions.Last());
            }

            listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view

            TryResolvePropertyAndAddToCollection(listType, listPropDes.ListPropertyDescriptions);

            listPropDes.ListPropertyDescriptions.Add(new PropertyDescription() { PropertyName = listType.Name, GeneralProperty = PossibleTypes.ObjectLine }); //Just for property separation in the view
        }


        public static bool CheckIfPropertyIsNumeric(PropertyInfo prop)
        {

            var type = prop.PropertyType;

            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
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
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //return IsNumericType(Nullable.GetUnderlyingType(type)); This is very advanced check for numeric property
                    }
                    return false;
            }
            return false;
        }

        public class PropertyDescription : INotifyPropertyChanged
        {
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
                    if (ListPropertyDescriptions.First().ValueAsString != null)
                    {
                        StringList.Add(ListPropertyDescriptions.First().ValueAsString);

                    }
                }

                if (ListProperty == PossibleTypes.List)
                {


                    /*if (ListPropertyDescriptions.First().ValueAsString == null)
                    {
                        StringList.Add(ListPropertyDescriptions.First().ValueAsString);
                    }
                    else
                    {
                        StringList.Add(ListPropertyDescriptions.First().ValueAsString);
                    }*/
                }

            }

            private void RemoveElement<T>(IList<T> list, int index)
            {
                list.RemoveAt(index);
            }

            private void ExecuteRemoveFromListCommand(object obj)
            {

                if (ListProperty == PossibleTypes.String)
                {
                    RemoveElement<string>(StringList, selectedItem);
                }

            }

            public PropertyDescription()
            {
                AddToListCommand = new RelayCommand(ExecuteAddToListCommand);
                RemoveFromListCommand = new RelayCommand(ExecuteRemoveFromListCommand, canExecute => selectedItem >= 0);
            }


            public ObservableCollection<string> StringList { get; set; }

            public ObservableCollection<PropertyDescription> listPropertyDescriptions;

            public ObservableCollection<PropertyDescription> ListPropertyDescriptions
            {
                get { return (ObservableCollection<PropertyDescription>)listPropertyDescriptions.Where(prop => prop.GeneralProperty != PossibleTypes.Class); }
                //get { return new ObservableCollection<PropertyDescription>(listPropertyDescriptions.Where(prop => prop.GeneralProperty == null)); }
                set { listPropertyDescriptions = value; }
            }


            public List<double> DoubleList { get; set; } = new List<double>() { 1, 2, 3, 4, 6 };

            public List<Object> ObjectList { get; set; }

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

        }

      
    }
}
