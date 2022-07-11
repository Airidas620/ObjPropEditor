using JSONConfFileEditor.Abstractions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
    [JsonObject()]
    public class MyCustomConfigurationClass2
    {
        [JsonProperty("InnerClass1")]
        public InnerClass1 innerClass1 = new InnerClass1();

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
        public uint GetLastFeedbackValue { get; set; }

        [JsonProperty]
        public uint GetLastFeedbackValue2 { get; set; }

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



        public class InnerClass1
        {

            [JsonProperty("InnerClass2")]
            public InnerClass2 innerClass2 = new InnerClass2();

            [JsonProperty]
            public string FeedbackTitle1x { get; set; }

            [JsonProperty]
            public bool IsFeedbackEnabled1x { get; set; }

            public class InnerClass2
            {
                [JsonProperty]
                public string FeedbackTitle2 { get; set; }

                [JsonProperty]
                public bool IsFeedbackEnabled2x { get; set; }
            }
        }
    }
}
