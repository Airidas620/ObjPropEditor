using VisualPropertyEditor.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisualPropertyEditor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PropertyEditorControl : UserControl
    {
        public PropertyEditorControl()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.eEmM-]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
