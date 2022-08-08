using JSONConfFileEditor.Abstractions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
    public class ApplicationConfiguration
    {

        //public ObservableCollection<string> AllAvailableProperties { get; set; } = new ObservableCollection<string> { "hi", "x" };

        //public List<List<List<string>>> test2 { get; set; } = new List<List<List<string>>>();
        public string DisplayTitle { get; set; }
        public Car SelectedCar { get; set; }
        public List<Car> AvailableCars { get; set; } = new List<Car>();

  


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


        public class Car
        {
            public EngineConfiguration Engine { get; set; }// = new InnerClass2(); 


            public string Model { get; set; }


            public bool IsAllWheelDrive { get; set; }

           
        }

        public class EngineConfiguration
        {

            public PowerSteeringConfig PowerSteering { get; set; }

            public string EngineTitle { get; set; }

            public bool IsTurboCharged { get; set; }
        }

        public class PowerSteeringConfig
        {

            public string PowerSteeringType { get; set; }

            public List<string> AllAffectedDevices { get; set; } = new List<string>();

            public bool IsEnabled { get; set; }
        }
    }

}
