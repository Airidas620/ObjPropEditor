using JSONConfFileEditor.Abstractions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
    /// <summary>
    /// Class for JSON config file 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MyCustomConfigurationClass
    {
        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism5 { get; set; }

        public InnerClass innerClass = new InnerClass();
        
        public bool IsFeedbackEnabled { get; set; }

        
        public bool IsFeedbackEnabled2 { get; set; }

        
        public bool IsFeedbackEnabled3 { get; set; }

        
        public bool IsFeedbackEnabled4 { get; set; }

        
        public bool IsFeedbackEnabled5 { get; set; }

        
        public double GetLastFeedbackValue { get; set; }

        
        public double GetLastFeedbackValue2 { get; set; }

        
        public string FeedbackTitle { get; set; }

        
        public string FeedbackTitle2 { get; set; }

        
        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism { get; set; }

        
        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism2 { get; set; }

        
        public FeedbackMechanismGroupTwoEnum TypeOfFeedbackMechanism3 { get; set; }

        public class InnerClass
        {

            
            public string Name { get; set; } = "name";

            
            public bool BoolValue { get; set; } = false;
        }

    }
}
