using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.ConfModels
{
    public class LogConfigBase
    {
        [JsonIgnore]
        [Description("Leave this empty, so master root directory will be used")]
        public string RootDirectory { get; set; } = "";
        public int Log1DEveryMs { get; set; } = 0;
        public bool? IsEnabled { get; set; }
    }

    public class PeripheralsLogConfig : LogConfigBase { }


    public class MotorsPositionLogConfig : LogConfigBase
    {
        public bool LogSteps { get; set; } = true;      
        public bool LogUnits { get; set; } = true;
        public bool LogSuperUnits { get; set; } = true;

    }

    public class PowerFeedbacksLogsConfig : LogConfigBase { }
    public class AmbientLogsConfig : LogConfigBase { }
    public class PowerMetersLogsConfig : LogConfigBase
    {        
        public bool LogInvalidToo { get; set; } = false;
    }


    [Description("This configuration is responsible for logging. Topas4 will log values accordingly.")]
    public class GeneralLogsConfig
    {
        [Description("Hello testing")]
        public string Alias { get; set; } = "";

        public string RootDirectory { get; set; } = "";
        public int Log1DEveryMs { get; set; } = 5000;
        public bool IsEnabled { get; set; }

        [Obsolete(nameof(ADCAndTemperature) + " is being phased out by dedicated-purpose loggers")]
        public PeripheralsLogConfig ADCAndTemperature { get; set; } = new PeripheralsLogConfig();
        public MotorsPositionLogConfig MotorPositions { get; set; } = new MotorsPositionLogConfig();
        public PowerFeedbacksLogsConfig PowerFeedbacks { get; set; } = new PowerFeedbacksLogsConfig();
        public AmbientLogsConfig Ambient { get; set; } = new AmbientLogsConfig();
        public PowerMetersLogsConfig PowerMeters { get; set; } = new PowerMetersLogsConfig();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            SetChildrenValues();
        }

        public void SetChildrenValues()
        {
            var children = new List<LogConfigBase> { ADCAndTemperature, MotorPositions, PowerFeedbacks, Ambient, PowerMeters };
            foreach (var item in children)
            {
                if (string.IsNullOrWhiteSpace(RootDirectory) == false && string.IsNullOrWhiteSpace(item.RootDirectory)) item.RootDirectory = RootDirectory;
                if (Log1DEveryMs != 0 && item.Log1DEveryMs == 0) item.Log1DEveryMs = Log1DEveryMs;
                if (item.IsEnabled.HasValue == false) item.IsEnabled = IsEnabled;
            }
        }
    }
}
