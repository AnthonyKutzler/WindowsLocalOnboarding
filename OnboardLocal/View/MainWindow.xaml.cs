using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OnboardLocal.Controller;

namespace OnboardLocal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        
        public int DrugScreen = 0;
        public MainWindow()
        {
            InitializeComponent();
            
            //Load Preferences
        }

        private void ChooseFile_OnClick(object sender, RoutedEventArgs e)
        {
            //Open file chooser dialog
            throw new System.NotImplementedException();
        }
        
        private void CreateFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (ExcelInstalled())
            {
                FilePath.Text = new ExcelService("").CreateTemplate();
            }
            //clone read only .xlsx file
            throw new System.NotImplementedException();
        }
        
        private void Run_OnClick(object sender, RoutedEventArgs e)
        {
            if (ExcelInstalled())
            {
                //handle login exception
                new OnboardingService().RunApplication(this);
                
            }
            else
            {
                //create Dialog of excel not installed
            }
            
            
        }

        private bool ExcelInstalled()
        {
            var excelApplication = new Microsoft.Office.Interop.Excel.Application();
            if (excelApplication != null) return true;
            MessageBox.Show("Excel is not properly installed!");
            return false;

        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton button)) return;
            var name = button.Name;
            switch (name)
            {
                case "BgPass":
                    DrugScreen = 0;
                    break;
                case "BgPend":
                    DrugScreen = 1;
                    break;
                case "PreBg":
                    DrugScreen = 2;
                    break;
            }
        }
        

        
    }

    
}