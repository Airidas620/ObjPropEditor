using JSONConfFileEditor.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JSONConfFileEditor.ViewModel;

namespace JSONConfFileEditor.ViewModel
{
    public class JSONConfigurationEditor : INotifyPropertyChanged
    {

        #region Lists for UI
        //#########################################//
        private List<BoolItem> boolCollection = new List<BoolItem>();
        /// <summary>
        /// Holds JSONControlViewModel.MyCustomConfigurationClass bool properties
        /// </summary>
        public List<BoolItem> BoolCollection
        {
            get { return boolCollection; }
            set
            {
                if (value != boolCollection)
                {
                    boolCollection = value;
                    //BoolCollectionLength = BoolCollection.Count;
                    NotifyPropertyChanged();

                }
            }
        }

        /// <summary>
        /// Holds The length of boolCollection list length
        /// </summary>
        public int BoolCollectionLength
        {
            get { return (int)Math.Round(BoolCollection.Count / 2.0, MidpointRounding.AwayFromZero); }

        }

        //#########################################//
        List<DoubleItem> doubleCollection = new List<DoubleItem>();
        /// <summary>
        /// Holds JSONControlViewModel.MyCustomConfigurationClass double properties
        /// </summary>
        public List<DoubleItem> DoubleCollection
        {
            get { return doubleCollection; }
            set
            {
                if (value != doubleCollection)
                {
                    doubleCollection = value;
                    DoubleCollectionLength = doubleCollection.Count;
                    NotifyPropertyChanged();
                }
            }
        }

        private int doubleCollectionLength;
        /// <summary>
        /// Holds The length of doubleCollectionLength list length
        /// </summary>
        public int DoubleCollectionLength
        {
            get { return doubleCollectionLength; }
            set
            {
                if (value != doubleCollectionLength)
                {
                    doubleCollectionLength = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //#########################################//
        List<StringItem> stringCollection = new List<StringItem>() { };
        /// <summary>
        /// Holds JSONControlViewModel.MyCustomConfigurationClass string properties
        /// </summary>
        public List<StringItem> StringCollection
        {
            get { return stringCollection; }
            set
            {
                if (value != stringCollection)
                {
                    stringCollection = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int stringCollectionLength;
        /// <summary>
        /// Holds The length of doubleCollectionLength list length
        /// </summary>
        public int StringCollectionLength
        {
            get { return stringCollectionLength; }
            set
            {
                if (value != stringCollectionLength)
                {
                    stringCollectionLength = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //#########################################//
        List<EnumItem> enumCollection = new List<EnumItem>() { };
        /// <summary>
        /// Holds JSONControlViewModel.MyCustomConfigurationClass enum properties
        /// </summary>
        public List<EnumItem> EnumCollection
        {
            get { return enumCollection; }
            set
            {
                if (value != enumCollection)
                {
                    enumCollection = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int enumCollectionLength;
        /// <summary>
        /// Holds The length of doubleCollectionLength list length
        /// </summary>
        public int EnumCollectionLength
        {
            get { return enumCollectionLength; }
            set
            {
                if (value != enumCollectionLength)
                {
                    enumCollectionLength = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public bool ChangeCurrentJsonClass(object newClass)
        {
            return true;
        }

        List<properties> AllAvailableProps;

        foreach(AllAvailableProperties){
            tryToResolveUiElement{
            if (propType.IsText) myView.Add(TextBox);

            }
            }

      //  public T MyCustomConfigurationClass { get; private set; }

        public JSONConfigurationEditor(T myCustomConfigurationClass)
        {

            MyCustomConfigurationClass = new JSONControlViewModel.MyCustomConfigurationClass();

            /// <summary>
            /// Adds UI elements
            /// </summary>
            foreach (var proprety in myCustomConfigurationClass.GetType().GetProperties())
            {
                switch (proprety.PropertyType.Name)
                {
                    case "Boolean":
                        BoolCollection.Add(new BoolItem(proprety.Name));
                        break;
                    case "Double":
                    case "Int":
                    case "Float":
                    case "Decimal":
                        DoubleCollection.Add(new DoubleItem(proprety.Name));
                        break;
                    case "String":
                        StringCollection.Add(new StringItem(proprety.Name));
                        break;
                    default:
                        if (proprety.PropertyType.IsEnum)
                        {
                            EnumCollection.Add(new EnumItem(proprety.Name, Enum.GetValues(proprety.PropertyType)));
                        }
                    break;
                }
            }
        }

        //SaveConfiguration()
        //{
        //    var props = curentConfigClass.GetProperties();

        //    props.ForEach(prop =>
        //    {
        //        if (AllAvailableProps.Contains(prop.Name)) ;

        //        //What is property type
        //        //Get that property value
        //        //If successful 
        //        //currentConfigClass.thatproperty = resolvedValue;

        //    } );

        //    // serialize current class and save to file;
        //}

      //  public Dictionary<string,T> AllAvailableProps { get; set; }

        #region different object value classes
        /// <summary>
        /// Class for boolean values
        /// </summary>
        public class BoolItem
        {

            public string Title { get; set; }

            public bool IsChecked { get; set; } = false;

            public BoolItem(string title)
            {
                Title = title;
            }

        }

        /// <summary>
        /// Class for double values
        /// </summary>
        public class DoubleItem
        {
            public string Title { get; set; }

            public double DoubleValue { get; set; }

            public DoubleItem(string title)
            {
                Title = title;
            }

        }

        /// <summary>
        /// Class for string values
        /// </summary>
        public class StringItem
        {
            public string Title { get; set; }

            public String StringValue { get; set; }
            public StringItem(string title)
            {
                Title = title;
            }

        }

        /// <summary>
        /// Class for enum values
        /// </summary>
        public class EnumItem
        {
            public string Title { get; set; }

            public Array ComboBoxValues { get; set; }

            public Enum ComboBoxValue { get; set; }
            public EnumItem(string title, Array enumValues)
            {
                ComboBoxValues = enumValues;
                Title = title;
            }

        }
        #endregion

        /// <summary>
        /// Assigns MyCustomConfigurationClass objects property values
        /// </summary>
        public void SetUpConfigurationClass()
        {

            int booleanIndex =  0, doubleIndex = 0, stringIndex = 0, enumIndex = 0;

            System.Reflection.PropertyInfo[] props =  MyCustomConfigurationClass.GetType().GetProperties();

              
            
            for (int i = 0; i < props.Length; i++)
            {
                switch (props[i].PropertyType.Name)
                {
                    case "Boolean":
                        props[i].SetValue(MyCustomConfigurationClass, BoolCollection[booleanIndex].IsChecked);
                        booleanIndex += 1;
                        break;

                    case "Double":
                        props[i].SetValue(MyCustomConfigurationClass, DoubleCollection[doubleIndex].DoubleValue);
                        doubleIndex += 1;
                        break;

                    case "String":
                        props[i].SetValue(MyCustomConfigurationClass, StringCollection[stringIndex].StringValue);
                        stringIndex += 1;
                        break;

                    default:
                        if (props[i].PropertyType.IsEnum)
                        {
                            props[i].SetValue(MyCustomConfigurationClass, EnumCollection[enumIndex].ComboBoxValue);
                            enumIndex += 1;
                        }
                        break;
                }
            }
           
        }


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
