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
using System.Windows.Controls.Primitives;

namespace Glicious
{
    public partial class VenuesPage : PhoneApplicationPage
    {
        public Menu menu;
        private DatePicker dPicker;
        IsolatedStorageSettings appsettings = IsolatedStorageSettings.ApplicationSettings;
        Popup mealChange = new Popup();
       

        public VenuesPage()
        {
            InitializeComponent();
            mealChange.IsOpen = false;
            ApplicationBar = new ApplicationBar();
            ApplicationBarIconButton settings = new ApplicationBarIconButton();
            settings.IconUri = new Uri("/Images/settings.png", UriKind.Relative);
            settings.Text = "Settings";
            settings.Click += new EventHandler(settings_Click);
            ApplicationBarIconButton changeMeal = new ApplicationBarIconButton();
            changeMeal.IconUri = new Uri("/Images/change.png", UriKind.Relative);
            changeMeal.Text = "Change Meal";
            changeMeal.Click += new EventHandler(changeMeal_Click);
            ApplicationBar.Buttons.Add(changeMeal);
            ApplicationBar.Buttons.Add(settings);
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
                Menu.Venue.Dish dummy = new Menu.Venue.Dish("", false, false, false, false, false);
                Type type = listBox.SelectedItem.GetType();
                Type type2 = dummy.GetType();
                if (type.FullName.Equals(type2.FullName)) 
                {
                    //Menu.Venue.Dish dummy2 = (Menu.Venue.Dish)listBox.SelectedItem;
                    //if (dummy2.hasNutrition)
                    //{
                        if (appsettings.Contains("nutrDish"))
                        {
                            appsettings.Remove("nutrDish");
                        }
                        appsettings.Add("nutrDish", listBox.SelectedItem);
                        NavigationService.Navigate(new Uri("/NutritionPage.xaml", UriKind.Relative));
                    //}
                }
                listBox.SelectedIndex = -1;
                //var selectedGroupUri = string.Format("/Views/GroupView.xaml?id={0}", id);
                //NavigationService.Navigate(new Uri(selectedGroupUri, UriKind.Relative));
            }
        }

        private void OnNavigatedTo()
        {
            loadData();
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        private void loadData()
        {
            listBox.Items.Clear();
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

                if (o[meal.Text.ToUpper()] != null && o[meal.Text.ToUpper()].HasValues)
                    foreach (JToken venue in o[meal.Text.ToUpper()].Children())
                    {
                        Menu.Venue.Dish[] tempDishes = new Menu.Venue.Dish[60];
                        String temp = venue.ToString().Substring(1, 40);
                        String venName = temp.Remove(temp.IndexOf("\""));
                        int j = 0;
                        foreach (JToken dish in venue.Children().Children())
                        {
                            bool ovolacto, vegan, halal, passover, hasNutrition;

                            String name = (String)dish["name"];

                            String ovo = (String)dish["ovolacto"];
                            if (ovo.Equals("true"))
                                ovolacto = true;
                            else
                                ovolacto = false;

                            String veg = (String)dish["vegan"];
                            if (veg.Equals("true"))
                                vegan = true;
                            else
                                vegan = false;

                            String passO = (String)dish["passover"];
                            if (passO.Equals("true"))
                                passover = true;
                            else
                                passover = false;

                            String hal = (String)dish["halal"];
                            if (hal.Equals("true"))
                                halal = true;
                            else
                                halal = false;

                            Newtonsoft.Json.Linq.JValue dummy = new Newtonsoft.Json.Linq.JValue(false);
                            if (dummy.GetType().FullName.Equals(dish["nutrition"].GetType().FullName))
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
                        tempVens[i++] = new Menu.Venue(venName, tempDishes); ;
                    }
                else
                    listBox.Items.Add(new Menu.Venue("No menu available \nfor selected meal", null));
                menu = new Menu(tempVens);
            }
            if ((appsettings.Contains("vegan")) && (appsettings.Contains("ovolacto")))
            {
                if ((bool)appsettings["ovolacto"])
                {
                    foreach (Menu.Venue ven in menu.venues)
                        if (ven != null)
                        {
                            bool added = false;
                            listBox.Items.Add(ven);
                            foreach (Menu.Venue.Dish dish in ven.dishes)
                                if (dish != null && (dish.ovolacto || dish.vegan))
                                {
                                    listBox.Items.Add(dish);
                                    added = true;
                                }
                            if (!added)
                                listBox.Items.Remove(ven);
                            else
                                listBox.Items.Add(new Menu.Venue("\t", null));
                        }
                }
                else if ((bool)appsettings["vegan"])
                {
                    foreach (Menu.Venue ven in menu.venues)
                        if (ven != null)
                        {
                            bool added = false;
                            listBox.Items.Add(ven);
                            foreach (Menu.Venue.Dish dish in ven.dishes)
                                if (dish != null && (dish.vegan))
                                {
                                    listBox.Items.Add(dish);
                                    added = true;
                                }
                            if (!added)
                                listBox.Items.Remove(ven);
                            else
                                listBox.Items.Add(new Menu.Venue("\t", null));
                        }
                }
                else
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
            else
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

        void popupCancel_Click(object sender, EventArgs e)
        {
            mealChange.IsOpen = false;
        }

        void popupBFast_Click(object sender, EventArgs e)
        {
            if (!appsettings["meal"].ToString().Equals("Breakfast"))
            {
                appsettings.Remove("meal");
                appsettings.Add("meal", "Breakfast");
                meal.Text = "Breakfast";
                loadData();
            }
            mealChange.IsOpen = false;
        }

        void popupLunch_Click(object sender, EventArgs e)
        {
            if (!appsettings["meal"].ToString().Equals("Lunch"))
            {
                appsettings.Remove("meal");
                appsettings.Add("meal", "Lunch");
                meal.Text = "Lunch";
                loadData();
            }
            mealChange.IsOpen = false;
        }

        void popupDinner_Click(object sender, EventArgs e)
        {
            if (!appsettings["meal"].ToString().Equals("Dinner"))
            {
                appsettings.Remove("meal");
                appsettings.Add("meal", "Dinner");
                meal.Text = "Dinner";
                loadData();
            }
            mealChange.IsOpen = false;
        }

        void popupOuttakes_Click(object sender, EventArgs e)
        {
            if (!appsettings["meal"].ToString().Equals("Outtakes"))
            {
                appsettings.Remove("meal");
                appsettings.Add("meal", "Outtakes");
                meal.Text = "Outtakes";
                loadData();
            }
            mealChange.IsOpen = false;
        }
        void changeMeal_Click(object sender, EventArgs e)
        {
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(5.0);

            StackPanel panel1 = new StackPanel();
            panel1.Background = new SolidColorBrush(Colors.Gray);

            Button cancel = new Button();
            cancel.Content = "Cancel";
            cancel.Margin = new Thickness(5.0);
            cancel.Click += new RoutedEventHandler(popupCancel_Click);
            Button bFast = new Button();
            bFast.Content = "Breakfast";
            bFast.Margin = new Thickness(5.0);
            bFast.Click += new RoutedEventHandler(popupBFast_Click);
            Button lunch = new Button();
            lunch.Content = "Lunch";
            lunch.Margin = new Thickness(5.0);
            lunch.Click += new RoutedEventHandler(popupLunch_Click);
            Button dinner = new Button();
            dinner.Content = "Dinner";
            dinner.Margin = new Thickness(5.0);
            dinner.Click += new RoutedEventHandler(popupDinner_Click);
            Button outtakes = new Button();
            outtakes.Content = "Outtakes";
            outtakes.Margin = new Thickness(5.0);
            outtakes.Click += new RoutedEventHandler(popupOuttakes_Click);
            TextBlock textblock1 = new TextBlock();
            textblock1.Text = "Choose a meal";
            textblock1.FontSize = 24;
            textblock1.Margin = new Thickness(5.0);
            panel1.Children.Add(textblock1);
            DateTime dTime = (DateTime)dPicker.Value;
            if ((dTime.DayOfWeek == DayOfWeek.Saturday) || (dTime.DayOfWeek == DayOfWeek.Sunday))
                if (dTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    panel1.Children.Add(lunch);
                    panel1.Children.Add(dinner);
                    panel1.Children.Add(cancel);
                }
                else
                {
                    panel1.Children.Add(bFast);
                    panel1.Children.Add(lunch);
                    panel1.Children.Add(dinner);
                    panel1.Children.Add(cancel);
                }
            else
            {
                panel1.Children.Add(bFast);
                panel1.Children.Add(lunch);
                panel1.Children.Add(dinner);
                panel1.Children.Add(outtakes);
                panel1.Children.Add(cancel);
            }
            border.Child = panel1;
            // Set the Child property of Popup to the border 
            // which contains a stackpanel, textblock and button.
            mealChange.Child = border;

            // Set where the popup will show up on the screen.
            mealChange.VerticalOffset = 50;
            mealChange.HorizontalOffset = 150;

            // Open the popup.
            mealChange.IsOpen = true;
        }
    }
}