using Autodesk.Revit.UI;
using Microsoft.Win32;
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


namespace RAA_WPF
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        public MyForm()
        {
            this.InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\";
            openFile.Filter = "csv files (*csv)|*.csv";

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
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public string GetTextBoxValue()
        {
            return tbxFile.Text;
        }

        public bool GetCheckBoxValue()
        {
            if (chbCheck1.IsChecked == true)
                return true;
            else
                return false;
        }

        public string GetGroup1()
        {
            if (rb1.IsChecked == true)
                return rb1.Content.ToString();
            else if(rb2.IsChecked == true)
                return rb2.Content.ToString();
            else
                return rb3.Content.ToString();
        }
    }
}
