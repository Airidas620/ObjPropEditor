using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Abstractions.Enums
{
    public enum PossibleTypes
    {
		Numeric,
		String,
		Bool,
		Enum,
		Class,
		List,
		ObjectLine,
		ListLine,
		Null,
		Unknown
	}
}
