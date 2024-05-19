using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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


namespace RAA_WPF
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Challenge01Form : Window
    {
        public Challenge01Form()
        {
            this.InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\";
            openFile.Filter = "Excel files|*.xls;*xlsx;*xlsm";

            if (openFile.ShowDialog() == true)
            {
                tbxFile.Text = openFile.FileName;
            }
            else
            {
                tbxFile.Text = "";
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(tbxFile.Text))
            {
                MessageBox.Show("Please select a file", "Error");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public string GetFilePath()
        {
            return tbxFile.Text;
        }

        public bool IsFloorPlanChecked()
        {
            if (cbFloor.IsChecked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCeilingPlanChecked()
        {
            if (cbCeiling.IsChecked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetUnitType()
        {
            if (rbImperial.IsChecked == true)
            {
                return rbImperial.Content.ToString();
            }
            else if (rbMetric.IsChecked == true)
            {
                return rbMetric.Content.ToString();
            }

            return null;
        }
    }
}
