using JSONConfFileEditor.Abstractions.Classes;
using JSONConfFileEditor.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

            TryResolvePropertyAndAddToCollection(customConfigurationClass.GetType(), customConfigurationClass);

        }

        int recursiveDepth = 0;
        int maxDepth = 100;

        public void TryResolvePropertyAndAddToCollection(Type type, Object src, int depth = 0)
        {


            recursiveDepth++;

            //var fields = type.GetFields().ToList();     

            var props = type.GetProperties().ToList();

            foreach (var prop in props)
            {


                if (prop.PropertyType.IsEnum)
                {
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Enum, AvailableEnumValues = Enum.GetValues(prop.PropertyType) });
                    continue;
                }

                if (CheckIfPropertyIsNumeric(prop))
                {
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Numeric });
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                {
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.String });
                    continue;
                }


                if (prop.PropertyType == typeof(bool))
                {
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Bool });
                    continue;
                }

                //If proporety was unresolved for some reason, it might go to recursion for the wrong reasons
                //null array

                //Console.WriteLine(prop.PropertyType.GetInterfaces().);


                /*foreach (var item in prop.PropertyType.GetInterfaces())
                {
                    Console.WriteLine(item);
                }*/

                if (prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
                    prop.PropertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.List
                        ,StringList = new ObservableCollection<string>() {"dfsdf"}, ListPropertyDescriptions = new ObservableCollection<ListPropertyDescription>()});
                    AllAvailableProperties.Last().ListPropertyDescriptions.Add(new ListPropertyDescription() { PropertyName = "String", GeneralProperty = PossibleTypes.String });
                    AllAvailableProperties.Last().ListPropertyDescriptions.Add(new ListPropertyDescription() { PropertyName = "String", GeneralProperty = PossibleTypes.String });
                    Console.WriteLine("oo");
                    continue;
                }


                if(prop.GetValue(src) != null)
                {
                    var increasedDepth = depth + 40;
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.FieldLine }); //Just for property separation in the view
                    AllAvailableProperties.Add(new PropertyDescription() { GeneralProperty = PossibleTypes.Class });
                    TryResolvePropertyAndAddToCollection(prop.PropertyType, prop.GetValue(src), increasedDepth);
                    AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, NestDepth = depth, GeneralProperty = PossibleTypes.FieldLine }); //Just for property separation in the view
                }
                else
                {
                    AllAvailableProperties.Add(new PropertyDescription() { GeneralProperty = PossibleTypes.Null});

                }


            }

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

        public class PropertyDescription
        {
            public RelayCommand AddToListCommand { set; get; }

            private void ExecuteAddToListCommand(object obj)
            {
                Console.WriteLine("hi");
                StringList.Add("tt");
                //Console.WriteLine(obj.ToString());
            }

            public PropertyDescription()
            {
                AddToListCommand = new RelayCommand(ExecuteAddToListCommand);
            }


            public ObservableCollection<string> StringList { get; set; }

            public ObservableCollection<ListPropertyDescription> ListPropertyDescriptions { get; set; }

            public List<double> DoubleList { get; set; } = new List<double>() { 1,2,3,4,6};

            public List<Object> Ok;


            public void Add()
            {
                Console.WriteLine("hi");
                //testing.Add(1);
            }


            public Type PropertyType { get; set; }

            public string PropertyName { get; set; }

            public double ValueAsDouble { get; set; }

            public string ValueAsString { get; set; }

            public bool ValueAsBool { get; set; }

            public Enum ValueAsEnum { get; set; }

            public Array AvailableEnumValues { get; set; }

            public PossibleTypes GeneralProperty { get; set; }
            public int NestDepth { get; set; }

        }

        public class ListPropertyDescription
        {
            public string PropertyName { get; set; }

            public double ValueAsDouble { get; set; }

            public string ValueAsString { get; set; }

            public bool ValueAsBool { get; set; }

            public Enum ValueAsEnum { get; set; }

            public PossibleTypes GeneralProperty { get; set; }
        }

    }
}
