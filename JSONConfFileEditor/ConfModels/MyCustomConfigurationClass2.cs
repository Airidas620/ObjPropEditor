﻿using JSONConfFileEditor.Abstractions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.ConfModels
{
    public class MyCustomConfigurationClass2
    {

        //public ObservableCollection<string> AllAvailableProperties { get; set; } = new ObservableCollection<string> { "hi", "x" };

        //public List<List<List<string>>> test2 { get; set; } = new List<List<List<string>>>();

        //public List<InnerClass1> listobj { get; set; } = new List<InnerClass1>();

        //public bool IsFeedbackEnabled2 { get; set; }

        //public List<InnerClass1> innerClass1 { get; set; }
        //public List<bool> listBool { get; set; } = new List<bool>();

        //public bool[] IsFeedbackEnabled { get; set; }

        /* public List<bool> listBool { get; set; } = new List<bool>();

         public List<double> listDouble { get; set; } = new List<double>();

         public List<Int32> lisInt32 { get; set; } = new List<Int32>();

         public List<FeedbackMechanismGroupOneEnum> listEnum { get; set; } = new List<FeedbackMechanismGroupOneEnum>();


         public List<string> listString { get; set; } = new List<string>();
         public List<string> listString2 { get; set; } = new List<string>();*/

        //public List<List<string>> test4 { get; set; } = new List<List<string>>();

        //public List<FeedbackMechanismGroupOneEnum> listEnum { get; set; } = new List<FeedbackMechanismGroupOneEnum>();

        //public List<List<string>> test4 { get; set; } = new List<List<string>>();

        //public List<InnerClass1> listobj { get; set; } = new List<InnerClass1>();

        /*public List<string> listString { get; set; } = new List<string>();

        public List<bool> listBool { get; set; } = new List<bool>();

        public List<double> listDouble { get; set; } = new List<double>();

        public List<Int32> lisInt32 { get; set; } = new List<Int32>();

        public List<FeedbackMechanismGroupOneEnum> listEnum { get; set; } = new List<FeedbackMechanismGroupOneEnum>();

        public List<InnerClass1> listobj{ get; set; } = new List<InnerClass1>();




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

        public InnerClass1 innerClass1 { get; set; } //= new InnerClass1();*/


        public class InnerClass1
        {

            public string FeedbackTitle1x { get; set; }

            public InnerClass2 innerClass2 { get; set; }// = new InnerClass2(); 



            public bool IsFeedbackEnabled1x { get; set; }

           
        }

        public class InnerClass2
        {

            public List<InnerClass3> InnerClass3 { get; set; }


            public string FeedbackTitle2 { get; set; }

            public InnerClass3 innerClass3 { get; set; }

            public bool IsFeedbackEnabled2x { get; set; }
        }

        public class InnerClass3
        {

            public string FeedbackTitle3 { get; set; }

            //public List<bool> stringArray { get; set; } = new List<bool>();

            public bool IsFeedbackEnabled3x { get; set; }
        }
    }

}
