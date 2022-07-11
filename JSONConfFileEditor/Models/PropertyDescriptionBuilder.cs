using JSONConfFileEditor.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
    public class PropertyDescriptionBuilder
    {

		public ObservableCollection<PropertyDescription> AllAvailableProperties { get; set; }


		public PropertyDescriptionBuilder(Object customConfigurationClass)
        {
			AllAvailableProperties = new ObservableCollection<PropertyDescription>();

			TryResolvePropertyAndAddToCollection(customConfigurationClass.GetType());

		}

		public void TryResolvePropertyAndAddToCollection(Type type)
		{
			var fields = type.GetFields().ToList();

			fields.ForEach(field => {
				TryResolvePropertyAndAddToCollection(field.FieldType);
			});


			var props = type.GetProperties().ToList();

			foreach (var prop in props)
            {
				if (prop.PropertyType.IsEnum)
				{
					var propDescription = new PropertyDescription() { PropertyName = prop.Name, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Enum };
					propDescription.AvailableEnumValues = Enum.GetValues(prop.PropertyType);

					AllAvailableProperties.Add(propDescription);
					continue;
				}

				if (CheckIfPropertyIsNumeric(prop))
				{
					AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Numeric});
					continue;
				}

				if (prop.PropertyType == typeof(string))
				{
					AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.String});
					continue;
				}


				if (prop.PropertyType == typeof(bool))
				{
					AllAvailableProperties.Add(new PropertyDescription() { PropertyName = prop.Name, PropertyType = prop.PropertyType, GeneralProperty = PossibleTypes.Bool});

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
			public System.Type PropertyType { get; set; }

			public string PropertyName { get; set; }

			public double ValueAsDouble { get; set; }

			public string ValueAsString { get; set; }

			public bool ValueAsBool { get; set; }

			public Enum ValueAsEnum { get; set; }

			public Array AvailableEnumValues { get; set; }

			public PossibleTypes GeneralProperty { get; set; }
		}

	}
}
