using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using OnboardLocal.Controller;

namespace OnboardLocal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
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
                filePath.Text = new ExcelService("").CreateTemplate();
            }
            //clone read only .xlsx file
            throw new System.NotImplementedException();
        }
        
        private void Run_OnClick(object sender, RoutedEventArgs e)
        {
            if (ExcelInstalled())
            {
                
            }
            //run application with input settings : amzEmail, amzPassword, questUsername, questPassword
            //nest in try catch to display errors
            
        }

        private bool ExcelInstalled()
        {
            Excel.Application excelApplication = new Microsoft.Office.Interop.Excel.Application();
            if (excelApplication == null)
            {
                MessageBox.Show("Excel is not properly installed!");
                return false;
            }

            return true;
        }
        
    }
}