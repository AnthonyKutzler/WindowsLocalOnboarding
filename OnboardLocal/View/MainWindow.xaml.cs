using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using OnboardLocal.Controller;
using OnboardLocal.Model;
using OpenQA.Selenium;

namespace OnboardLocal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    
    
    
    public partial class MainWindow
    {

        private readonly PeopleProvider _provider = new PeopleProvider();
        private Person _person;
        private bool _newPerson = false;
        public int DrugScreen = 0;
        public MainWindow()
        {
            InitializeComponent();

            ChromeDriverPath.Text = Properties.Settings.Default.ChromedriverPath ?? "";
            AccountNumber.Text = Properties.Settings.Default.AccountId ?? "";
            OrderCode.Text = Properties.Settings.Default.OrderCode ?? "";
            LocationCode.Text = Properties.Settings.Default.CollectionSite ?? "";
            BadgeId.Text = Properties.Settings.Default.BadgeId ?? "";
            StationCode.Text = Properties.Settings.Default.StationCode ?? "";
            AmzEmail.Text = Properties.Settings.Default.AmzEmail ?? "";
            QuestUsername.Text = Properties.Settings.Default.QuestUsername ?? "";
            AmzPassword.Password = Properties.Settings.Default.AmzPassword ?? "";
            QuestPassword.Password = Properties.Settings.Default.QuestPassword ?? "";

            SetTableData();
        }
        
        
        private async void Run_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AmzEmail = AmzEmail.Text;
            Properties.Settings.Default.QuestUsername = QuestUsername.Text;
            Properties.Settings.Default.AmzPassword = AmzPassword.Password;
            Properties.Settings.Default.QuestPassword = QuestPassword.Password;
            Properties.Settings.Default.Save();
            try
            {
                await System.Threading.Tasks.Task.Run(() => RunOnboarding(AmzPassword.Password, QuestPassword.Password, DrugScreen));
            }
            catch (LoginException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (WebDriverException ex2)
            {
                Console.Write(ex2.Message);
                MessageBox.Show("Chromedriver closed unexpectedly!");
            }
            SetTableData();

        }

        private async System.Threading.Tasks.Task RunOnboarding(string amzPass, string questPass, int drugScreen)
        {
            new OnboardingService().RunApplication(amzPass, questPass, drugScreen);
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

        private void QuestSave_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AccountId = AccountNumber.Text;
            Properties.Settings.Default.OrderCode = OrderCode.Text;
            Properties.Settings.Default.CollectionSite = LocationCode.Text;
            Properties.Settings.Default.BadgeId = BadgeId.Text;
            Properties.Settings.Default.StationCode = StationCode.Text;
            Properties.Settings.Default.Save();
            SettingsSaved.Visibility = Visibility.Visible;

        }

        

        private void ResultsGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Do you want to Update this new entry", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _provider.UpdatePerson(_person);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }
        
        
        private void ResultsGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                var first = ResultsGrid.Columns[0].GetCellContent(e.Row);
                if (first.GetType() == typeof(TextBox))
                {
                    _person.FirstName = ((TextBox) first).Text;
                }

                var last = ResultsGrid.Columns[0].GetCellContent(e.Row);
                if (last.GetType() == typeof(TextBox))
                {
                    _person.Lastname = ((TextBox) last).Text;
                }

                var phone = ResultsGrid.Columns[0].GetCellContent(e.Row);
                if (phone.GetType() == typeof(TextBox))
                {
                    _person.Phone = ((TextBox) phone).Text;
                }

                var email = ResultsGrid.Columns[0].GetCellContent(e.Row);
                if (email.GetType() == typeof(TextBox))
                {
                    _person.Email = ((TextBox) email).Text;
                }

                var background = ResultsGrid.Columns[0].GetCellContent(e.Row);
                if (background.GetType() == typeof(TextBox))
                {
                    _person.Background = ((TextBox) background).Text;
                }

                var drug = ResultsGrid.Columns[0].GetCellContent(e.Row);
                if (drug.GetType() == typeof(TextBox))
                {
                    _person.Drug = ((TextBox) drug).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void ResultsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _person = ResultsGrid.SelectedItem as Person;
            _newPerson = _person == null;
        }

        private void ChooseChromedriver(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ChromedriverPath = Path.GetDirectoryName(ChooseFile(new OpenFileDialog {Filter = "Executable (*.exe)|*.exe"}));
            Properties.Settings.Default.Save();
            ChromeDriverPath.Text = Properties.Settings.Default.ChromedriverPath;
        }

        private void CheckChromedriver(object sender, RoutedEventArgs e)
        {
            try
            {
                OnboardingService.CheckChromeDriver(ChromeDriverPath.Text);
                DriverWorks.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static string ChooseFile(FileDialog dialog)
        {
            return dialog.ShowDialog() == true ? dialog.FileName : "";
        }
        
        private void ImportData(object sender, RoutedEventArgs e)
        {
            try
            {
                new CsvService<Person>().ImportPeople(ChooseFile(new OpenFileDialog {Filter = "CSV (*.csv)|*.csv|All Files (*.*)|*.*"}));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
            SetTableData();
        }

        private void ExportData(object sender, RoutedEventArgs e)
        {
            try
            {
                new CsvService<Person>().ExportPeople(
                    ChooseFile(new SaveFileDialog
                        {Filter = "Comma-separated values (*.csv)|*.csv|All Files (*.*)|*.*"}),
                    new PeopleProvider().GetPeople());
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong. Error104!");
            }
        }

        private void NewOnboard_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                new OnboardingService().NewOnboard(this);
            }
            catch(NoSuchElementException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Insert_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO: DATA Verification
            new PeopleProvider().InsertPerson(new Person(First.Text, Last.Text, Phone.Text, Email.Text, "", ""));
            SetTableData();
        }

        private void SetTableData()
        {
            try
            {
                var people = new PeopleService().GetPeopleSorted().ToList();
                if (people.Count > 0)
                    ResultsGrid.ItemsSource = people;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void HandleDrugRadio(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleProviderRadio(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    
}