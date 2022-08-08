﻿using JSONConfFileEditor.Abstractions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
    public class MyCustomConfigurationClass2
    {
        public InnerClass1 innerClass1 { get; set; } = new InnerClass1();

        public bool IsFeedbackEnabled { get; set; }


        public bool IsFeedbackEnabled2 { get; set; }


        public bool IsFeedbackEnabled3 { get; set; }


        public bool IsFeedbackEnabled4 { get; set; }


        public bool IsFeedbackEnabled5 { get; set; }


        public uint GetLastFeedbackValue { get; set; }


        public uint GetLastFeedbackValue2 { get; set; }


        public string FeedbackTitle { get; set; }


        public string FeedbackTitle2 { get; set; }


        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism { get; set; }


        public FeedbackMechanismGroupOneEnum TypeOfFeedbackMechanism2 { get; set; }


        public FeedbackMechanismGroupTwoEnum TypeOfFeedbackMechanism3 { get; set; }



        public class InnerClass1
        {
            public InnerClass2 innerClass2 { get; set; }= new InnerClass2();


            public string FeedbackTitle1x { get; set; }


            public bool IsFeedbackEnabled1x { get; set; }

           
        }

        public class InnerClass2
        {

            public string FeedbackTitle2 { get; set; }


            public bool IsFeedbackEnabled2x { get; set; }
        }
    }

}
