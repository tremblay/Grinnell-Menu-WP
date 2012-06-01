using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;


namespace Glicious
{
    public partial class MainPage : PhoneApplicationPage
    {

        private string daysAvailableTXT;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.datePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(picker_ValueChanged);

            hideAllButtons();
            textBlock1.Text = "Loading menus, please wait.";
        }

        private void hideAllButtons()
        {
            outtakesButton.Visibility = Visibility.Collapsed;
            dinnerButton.Visibility = Visibility.Collapsed;
            lunchButton.Visibility = Visibility.Collapsed;
            bfastButton.Visibility = Visibility.Collapsed;
            textBlock1.Visibility = Visibility.Visible;
            
        }

        private void checkButtons(DateTime date)
        {
            textBlock1.Visibility = Visibility.Collapsed;
            if (date.DayOfWeek == DayOfWeek.Sunday)
                bfastButton.Visibility = Visibility.Collapsed;
            else
                bfastButton.Visibility = Visibility.Visible;
            if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                outtakesButton.Visibility = Visibility.Collapsed;
            else
                outtakesButton.Visibility = Visibility.Visible;
            dinnerButton.Visibility = Visibility.Visible;
            lunchButton.Visibility = Visibility.Visible;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var webClient = new WebClient();
            webClient.OpenReadAsync(new Uri("http://www.cs.grinnell.edu/~tremblay/menu/available_days_FAKE.php"));
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);

        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            using (var reader = new StreamReader(e.Result))
            {
                daysAvailableTXT = reader.ReadToEnd();
                DateTime newTime = (DateTime)datePicker.Value;
                if (daysAvailableTXT.Equals("-1"))
                {
                    datePicker.Value = DateTime.Today;
                    hideAllButtons();
                    textBlock1.Text = "No menus are currently available.";
                }
                else
                {
                    if (newTime.DayOfYear < DateTime.Today.DayOfYear)
                    {
                        newTime = DateTime.Today;
                        datePicker.Value = newTime;
                        checkButtons(newTime);
                    }
                    else if ((DateTime.Today.DayOfYear + Int32.Parse(daysAvailableTXT)) < newTime.DayOfYear)
                    {
                        hideAllButtons();
                        String s;
                        if (daysAvailableTXT.Equals("0"))
                            s = "No menus are available for the selected date.\nToday's menu is the only available menu.";
                        else if (daysAvailableTXT.Equals("0"))
                            s = "No menus are available for the selected date.\nThere is only 1 day after today available.";
                        else
                            s = System.String.Format("No menus are available for the selected date.\nThere are only {0} days after today available.", daysAvailableTXT);
                        textBlock1.Text = s;
                    }
                    else
                        checkButtons(newTime);
                }
            }
        }
 
        void picker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            hideAllButtons();
            textBlock1.Text = "";
        }


        private void buttonClick()
        {
            datePicker.ValueStringFormat = "{0:D}";
            (App.Current as App).datePick = datePicker;
            NavigationService.Navigate(new Uri("/VenuesPage.xaml", UriKind.Relative));
        }

        private void bfastButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).mealString = "Breakfast";
            buttonClick();
        }

        private void lunchButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).mealString = "Lunch";
            buttonClick();
        }

        private void dinnerButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).mealString = "Dinner";
            buttonClick();
        }

        private void outtakesButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).mealString = "Outtakes";
            buttonClick();
        }
    }
}