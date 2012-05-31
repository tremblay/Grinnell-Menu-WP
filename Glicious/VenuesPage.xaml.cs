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
        public Menu menu;
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
                dPicker = (DatePicker)appsettings["date"];
                dPicker.ValueStringFormat = "{0:D}";
                date.Text = dPicker.ValueString;
            }
            else
            {
                meal.Visibility = Visibility.Collapsed;
                date.Visibility = Visibility.Collapsed;
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ignore scenarios when we navigate back to this page and clear what was previously selected
            if (listBox.SelectedItem != null)
            {
                //var selectedGroupUri = string.Format("/Views/GroupView.xaml?id={0}", id);
                //NavigationService.Navigate(new Uri(selectedGroupUri, UriKind.Relative));
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
            using (var reader = new StreamReader(e.Result))
            {
                String json = reader.ReadToEnd();
                JObject o = JObject.Parse(json);
                Menu.Venue [] tempVens = new Menu.Venue[30];
                int i = 0;
                foreach (JToken venue in o[meal.Text.ToUpper()].Children())
                {
                    Menu.Venue.Dish [] tempDishes = new Menu.Venue.Dish[60];
                    String temp = venue.ToString().Substring(1, 40);
                    String venName = temp.Remove(temp.IndexOf("\""));
                    int j = 0;
                    foreach (JToken dish in venue.Children().Children())
                    {
                        bool ovolacto, vegan, halal, passover, hasNutrition;
                        String name = (String)dish["name"];
                        if (dish["ovolacto"].Equals("true"))
                            ovolacto = true;
                        else
                            ovolacto = false;
                        if (dish["vegan"].Equals("true"))
                            vegan = true;
                        else
                            vegan = false;
                        if (dish["passover"].Equals("true"))
                            passover = true;
                        else
                            passover = false;
                        if (dish["halal"].Equals("true"))
                            halal = true;
                        else
                            halal = false;

                        if (dish["nutrition"].Equals("NIL"))
                        {
                            hasNutrition = false;
                            tempDishes[j++] = new Menu.Venue.Dish(name, hasNutrition, ovolacto, vegan, passover, halal);
                        }
                        else
                        {
                            hasNutrition = true;
                            float[] nutrition = new float[20];
                            int k = 0;
                            foreach (JToken child in dish["nutrition"])
                                nutrition[k++] = (float)child;
                            tempDishes[j++] = new Menu.Venue.Dish(name, hasNutrition, ovolacto, vegan, passover, halal, nutrition);
                        }
                        
                    }
                    tempVens[i++] = new Menu.Venue(venName, tempDishes);;
                }
                menu = new Menu(tempVens);
            }
            foreach (Menu.Venue ven in menu.venues)
                if (ven != null)
                {
                    listBox.Items.Add(ven);
                    foreach (Menu.Venue.Dish dish in ven.dishes)
                        if (dish != null)
                            listBox.Items.Add(dish);
                    listBox.Items.Add(new Menu.Venue("\t", null));
                }
        }

        void settings_Click(object sender, EventArgs e)
        {
             NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}