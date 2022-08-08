using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Abstractions.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackMechanismGroupTwoEnum
    {
        GroupTwoTypeOne = 0,
        GroupTwoTypeTwo = 1,
        GroupTwoTypeThree = 2,
        GroupTwoTypeFour = 3,
    }
}
