﻿using System;
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
    public partial class SettingsPage : PhoneApplicationPage
    {
        IsolatedStorageSettings appsettings = IsolatedStorageSettings.ApplicationSettings;
        public SettingsPage()
        {
            InitializeComponent();
            ApplicationBar = new ApplicationBar();
            ApplicationBarIconButton save = new ApplicationBarIconButton();
            save.IconUri = new Uri("/Images/save.png", UriKind.Relative);
            save.Text = "Save";
            ApplicationBar.Buttons.Add(save);
            save.Click += new EventHandler(save_Click);
            ApplicationBarIconButton cancel = new ApplicationBarIconButton();
            cancel.IconUri = new Uri("/Images/close.png", UriKind.Relative);
            cancel.Text = "Cancel";
            ApplicationBar.Buttons.Add(cancel);
            cancel.Click += new EventHandler(cancel_Click);

            ovolactoBox.IsChecked = (App.Current as App).ovoFilter;
            veganBox.IsChecked = (App.Current as App).veganFilter;
        }

        void save_Click(object sender, EventArgs e)
        {
            if (appsettings.Contains("ovolacto"))
                appsettings.Remove("ovolacto"); 
            if (appsettings.Contains("vegan"))
                appsettings.Remove("vegan");
            appsettings.Add("ovolacto", ovolactoBox.IsChecked);
            appsettings.Add("vegan", veganBox.IsChecked);
            (App.Current as App).ovoFilter = (bool)ovolactoBox.IsChecked;
            (App.Current as App).veganFilter = (bool)veganBox.IsChecked;
            appsettings.Save();
            NavigationService.GoBack();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}