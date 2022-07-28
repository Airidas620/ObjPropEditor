using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.ConfModels.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackMechanismGroupOneEnum
    {
        GroupOneTypeOne = 0,
        GroupOneTypeTwo = 1,
        GroupOneTypeThree = 2,
    }
}
