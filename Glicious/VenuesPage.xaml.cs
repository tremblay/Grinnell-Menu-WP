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
using System.IO;
using Newtonsoft.Json.Linq;

namespace Glicious
{
    public partial class VenuesPage : PhoneApplicationPage
    {
        private DatePicker dPicker;
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
                meal.Visibility = Visibility.Visible;
                date.Visibility = Visibility.Visible;
                meal.Text = appsettings["meal"].ToString();
                dPicker = (DatePicker) appsettings["date"];
                dPicker.ValueStringFormat = "{0:D}";
                date.Text = dPicker.ValueString;
            }
            else
            {
                meal.Visibility = Visibility.Collapsed;
                date.Visibility = Visibility.Collapsed;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime dTime = (DateTime)dPicker.Value;
            var webClient = new WebClient();
            String urlString = System.String.Format("http://www.cs.grinnell.edu/~tremblay/menu/{0}-{1}-{2}.json", dTime.Month, dTime.Day, dTime.Year);
           // System.Diagnostics.Debug.WriteLine(urlString);
            webClient.OpenReadAsync(new Uri(urlString));
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);

        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Menu newMenu = new Menu();
            using (var reader = new StreamReader(e.Result))
            {
                String json = reader.ReadToEnd();
                JObject o = JObject.Parse(json);
                newMenu.venues = new Menu.Venue[30];
                int i = 0;
                foreach (JToken venue in o[meal.Text.ToUpper()].Children())
                {
                    newMenu.venues[i] = new Menu.Venue();
                    newMenu.venues[i].dishes = new Menu.Venue.Dish[60];
                    String temp = venue.ToString().Substring(1, 40);
                    newMenu.venues[i].name = temp.Remove(temp.IndexOf("\""));
                    int j = 0;
                    foreach (JToken dish in venue.Children().Children())
                    {
                        newMenu.venues[i].dishes[j] = new Menu.Venue.Dish();
                        newMenu.venues[i].dishes[j].name = (String)dish["name"];
                        if (dish["ovolacto"].Equals("true"))
                            newMenu.venues[i].dishes[j].ovolacto = true;
                        else
                            newMenu.venues[i].dishes[j].ovolacto = false;
                        if (dish["vegan"].Equals("true"))
                            newMenu.venues[i].dishes[j].vegan = true;
                        else
                            newMenu.venues[i].dishes[j].vegan = false;
                        if (dish["passover"].Equals("true"))
                            newMenu.venues[i].dishes[j].passover = true;
                        else
                            newMenu.venues[i].dishes[j].passover = false;
                        if (dish["halal"].Equals("true"))
                            newMenu.venues[i].dishes[j].halal = true;
                        else
                            newMenu.venues[i].dishes[j].halal = false;

                        if (dish["nutrition"].Equals("NIL"))
                            newMenu.venues[i].dishes[j].hasNutrition = false;
                        else
                        {
                            newMenu.venues[i].dishes[j].hasNutrition = true;
                            newMenu.venues[i].dishes[j].nutrition = new float[20];
                            int k = 0;
                            foreach (JToken child in dish["nutrition"])
                                newMenu.venues[i].dishes[j].nutrition[k] = (float)child;
                        }
                    }
                }
            }
            //TODO - make dishes visible

           // venues.DataContext = newMenu.venues;
           // venues.ItemsSource = newMenu.venues;
        }

        void settings_Click(object sender, EventArgs e)
        {
             NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}