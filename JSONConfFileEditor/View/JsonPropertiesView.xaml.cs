using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using JSONConfFileEditor.ViewModel;

namespace JSONConfFileEditor.View
{
    /// <summary>
    /// Interaction logic for JsonPropertiesView.xaml
    /// </summary>
    public partial class JsonPropertiesView : UserControl
    {


        public JsonPropertiesView()
        {
            InitializeComponent();

            //this.DataContext = MainWindow.JSONControlViewModel.JSONConfigurationEditor;

            //Try Catch to avoid design time error
            try
            {
                this.DataContext = MainWindow.JSONControlViewModel.JSONConfigurationEditor;

                /*var viewModel  = (PropertyEditor)this.DataContext;
                viewModel.FocusEvent += () => ScrollViewer.Focus();*/

            }
            catch (Exception)
            {

            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.eEmM-]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
