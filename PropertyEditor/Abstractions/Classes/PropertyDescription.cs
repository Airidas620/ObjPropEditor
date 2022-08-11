using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VisualPropertyEditor.Abstractions.Classes;
using VisualPropertyEditor.Abstractions.Enums;

namespace VisualPropertyEditor.Abstractions
{
    public class PropertyDescription : INotifyPropertyChanged
    {
        #region ObjectProperties

        //Properties used for describing single class properties

        /// <summary>
        /// Property Type
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Property name
        /// </summary>
        public string PropertyName { get; set; }

        public bool HasDescription => ! string.IsNullOrEmpty(Description);
        public string Description { get; set; } = "";

        /// <summary>
        /// Holds string value if property is a string
        /// </summary>
        public string ValueAsString { get; set; } = "";

        private string numericValueAsString = "0";
        /// <summary>
        /// Holds string value if property is a number
        /// </summary>
        public string NumericValueAsString
        {
            get { return numericValueAsString; }
            set
            {
                //Try parsing to validate written value
                try
                {
                    NumericParser.StringToNumericTypeValue(PropertyType, value).ToString();
                    
                    numericValueAsString = value;
                    IsInputValueValid = true;
                }
                catch (Exception)
                {
                    numericValueAsString = "0";
                    IsInputValueValid = false;
                }
        }
        }


        private bool isInputValueValid = true;

        /// <summary>
        /// Is the value in GUI is valid
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
        /// Holds bool value if property is a bool
        /// </summary>
        public bool ValueAsBool { get; set; }

        /// <summary>
        /// Holds enum value if property is a enum
        /// </summary>
        public object ValueAsEnum { get; set; } 

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
        public PropEditCommand AddToListCommand { set; get; }


        /// <summary>
        /// Command for removing items from List<>
        /// </summary>
        public PropEditCommand RemoveFromListCommand { set; get; }

        /// <summary>
        /// Command for duplicating List<> item
        /// </summary>
        public PropEditCommand DuplicateListItemCommand { set; get; }

        private void ExecuteAddToListCommand(object obj)
        {

            //First() because lists hold one generic type parameter
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

        private void ExecuteDuplicateListItemCommand(Object obj)
        {
            var listItem = parentListItems.ElementAt(ListItemIndex);

            parentListItems.Add(

                (PropertyDescription)listItem.Clone(true)
            );

            parentListItems.Last().parentListItems = parentListItems;
            parentListItems.Last().ListItemIndex = parentListItems.Count - 1;
        }

        public PropertyDescription()
        {
            AddToListCommand = new PropEditCommand(ExecuteAddToListCommand);
            RemoveFromListCommand = new PropEditCommand(ExecuteRemoveFromListCommand);
            DuplicateListItemCommand = new PropEditCommand(ExecuteDuplicateListItemCommand);
        }


        /// <summary>
        /// Saves GUI List items data to ObjectList 
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

            if (ListProperty == PossibleTypes.List)
            {
                Array values;

                //Used to avoid writing long lines. 
                var referenceToAListType = innerPropertyDescriptions.First().innerPropertyDescriptions.First().PropertyType;

                foreach (var propertyList in listItems)
                {

                    values = Array.CreateInstance(referenceToAListType, propertyList.listItems.Count());

                    propertyList.SaveGUIListDataToList();


                    if (innerPropertyDescriptions.First().ListProperty == PossibleTypes.Numeric)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            try
                            {
                                values.SetValue(NumericParser.StringToNumericTypeValue(referenceToAListType, 
                                    propertyList.ObjectList[i].ToString()), i);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                    else
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            values.SetValue(propertyList.ObjectList[i], i);
                        }
                    }


                    var multiDimenionalList = Activator.CreateInstance(typeof(List<>).MakeGenericType(referenceToAListType), new object[] { values });

                    ObjectList.Add(multiDimenionalList);

                }
                return;
            }

            if (ListProperty == PossibleTypes.Class)
            {

                foreach (var propertyList in listItems)
                {
                    Object listInstance = Activator.CreateInstance(ListObjectType);

                    //Assing values to Object from GUI 
                    PropertyDescriptionHelper.SetObjectValuesWithPropertyDescription(listInstance, propertyList.innerPropertyDescriptions);

                    //Add to List
                    ObjectList.Add(listInstance);
                }
            }

        }

        /// <summary>
        /// Clones PropertyDescription. Used for adding new items to list and cloning a list item
        /// <param name="CloneGuiValues"/> If to clone PropertyDescription with values from GUI
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
                propDescriptionCopy.numericValueAsString = this.NumericValueAsString;
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
}
