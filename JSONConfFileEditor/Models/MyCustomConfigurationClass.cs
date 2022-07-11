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
        
        [JsonProperty]
        public InnerClass innerClass = new InnerClass();

        [JsonProperty]
        public bool IsFeedbackEnabled { get; set; }

        [JsonProperty]
        public bool IsFeedbackEnabled2 { get; set; }

        [JsonProperty]
        public bool IsFeedbackEnabled3 { get; set; }

        [JsonProperty]
        public bool IsFeedbackEnabled4 { get; set; }

        [JsonProperty]
        public bool IsFeedbackEnabled5 { get; set; }

        [JsonProperty]
        public double GetLastFeedbackValue { get; set; }

        [JsonProperty]
        public double GetLastFeedbackValue2 { get; set; }

        [JsonProperty]
        public string FeedbackTitle { get; set; }

        [JsonProperty]
        public string FeedbackTitle2 { get; set; }

        [JsonProperty]
        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism { get; set; }

        [JsonProperty]
        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism2 { get; set; }

        [JsonProperty]
        public FeedbackMechanismGroupTwoEnum TypeOfFeedbackMechanism3 { get; set; }

        public class InnerClass
        {

            [JsonProperty]
            public string Name { get; set; } = "name";

            [JsonProperty]
            public bool BoolValue { get; set; } = false;
        }

    }
}
