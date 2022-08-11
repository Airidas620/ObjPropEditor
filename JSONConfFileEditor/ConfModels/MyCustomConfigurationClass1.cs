using JSONConfFileEditor.ConfModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.ConfModels
{
    public class MyCustomConfigurationClass1
    {

        public bool IsFeedbackEnabled { get; set; }

        public double GetLastFeedbackValue { get; set; }

        public string FeedbackTitle0 { get; set; }

        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism { get; set; }

        public InnerClass0 innerClass0 { get; set; }

        public InnerClass1 innerClass1 { get; set; }

        public List<bool> boolList { get; set; }
        
        public List<double> doubleList { get; set; }
        
        public List<string> stringList { get; set; }
        
        public List<FeedbackMechanismGroupOneEnum> EnumList { get; set; }

        public List<InnerClass0> objectList1{ get; set; }

        public List<InnerClass1> objectList2 { get; set; }

        public List<List<List<int>>> MultiDimensionalList { get; set; } = new List<List<List<int>>> { new List<List<int>> { new List<int> { 1, 2, 3, 5 }, new List<int> { 1, 2, 5 } }, new List<List<int>> { new List<int> { 1, 2, 3, 5 }, new List<int> { 1, 2, 5 } } };


        public class InnerClass0
        {
            public bool IsFeedbackEnabled { get; set; }

            public double GetLastFeedbackValue { get; set; }

            public string FeedbackTitle { get; set; }

            public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism { get; set; }

        }

        public class InnerClass1
        {
            public string FeedbackTitle { get; set; }

            public bool IsFeedbackEnabled { get; set; }

            public InnerClass2 innerClass2 { get; set; }


        }

        public class InnerClass2
        {
            public bool IsFeedbackEnabled { get; set; }

            public InnerClass3 innerClass3 { get; set; }

        }

        public class InnerClass3
        {
            public string FeedbackTitle3 { get; set; }

        }
    }
}
