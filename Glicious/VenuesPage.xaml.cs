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

namespace Glicious
{
    public partial class Page1 : PhoneApplicationPage
    {
        private IsolatedStorageSettings appsettings = IsolatedStorageSettings.ApplicationSettings;
        public Page1()
        {
            InitializeComponent();
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
    }
}