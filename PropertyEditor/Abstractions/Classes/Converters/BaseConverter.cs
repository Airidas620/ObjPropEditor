using System;

namespace VisualPropertyEditor.Abstractions.Classes.Converters
{
    internal class BaseConverter : System.Windows.Markup.MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
