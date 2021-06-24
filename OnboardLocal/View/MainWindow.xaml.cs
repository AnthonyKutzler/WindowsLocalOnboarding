using System;
using System.Collections;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OnboardLocal.Controller;
using OnboardLocal.Model;

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
        
        public int DrugScreen = 0;
        public MainWindow()
        {
            InitializeComponent();
            AccountNumber.Text = Properties.Settings.Default.AccountId;
            OrderCode.Text = Properties.Settings.Default.OrderCode;
            LocationCode.Text = Properties.Settings.Default.CollectionSite;
            BadgeId.Text = Properties.Settings.Default.BadgeId;
            StationCode.Text = Properties.Settings.Default.StationCode;
            ResultsGrid.ItemsSource = _provider.GetPeople();

            
        }
        
        
        private void Run_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                new OnboardingService().RunApplication(this);
            }
            catch (LoginException ex)
            {
                MessageBox.Show(ex.Message);
            }


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

            SettingsSaved.Visibility = Visibility.Visible;

        }

        private void ImportData(object sender, RoutedEventArgs e)
        {
            
        }

        private void ExportData(object sender, RoutedEventArgs e)
        {
        }

        private void ResultsGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Do you want to Create this new entry", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _provider.InsertPerson(_person);
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
        }
        
    }

    
}