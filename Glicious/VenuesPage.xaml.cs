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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

namespace Glicious
{
    public partial class VenuesPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appsettings = IsolatedStorageSettings.ApplicationSettings;
        public VenuesPage()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();
            ApplicationBarIconButton settings = new ApplicationBarIconButton();
            settings.IconUri = new Uri("/Images/settings.png", UriKind.Relative);
            settings.Text = "Settings";
            ApplicationBar.Buttons.Add(settings);
            settings.Click += new EventHandler(settings_Click);

            if ((appsettings.Contains("meal")) && (appsettings.Contains("date")))
            {
                meal.Text = appsettings["meal"].ToString();
                date.Text = appsettings["date"].ToString();
            }
            else
            {
                meal.Visibility = Visibility.Collapsed;
                date.Visibility = Visibility.Collapsed;
            }

            //venues.DataContext = 
        }
        void settings_Click(object sender, EventArgs e)
        {
             NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}