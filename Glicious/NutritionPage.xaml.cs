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
    public partial class NutritionPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appsettings = IsolatedStorageSettings.ApplicationSettings;
        private Menu.Venue.Dish dish;
        public NutritionPage()
        {
            InitializeComponent();

            if (appsettings.Contains("nutrDish"))
            {
                dish = (Menu.Venue.Dish)appsettings["nutrDish"];
                dishName.Visibility = Visibility.Visible;
                dishName.Text = dish.name;
                if (dish.hasNutrition)
                {
                    String temp = System.String.Format("Calories {0}\nTotal Fat {1}g\n\tSaturated Fat {4}g\n\tTrans Fat {14}g\n\tPolyunsaturated Fat {5}g\n\tMonounsaturated Fat {6}g\nCholesterol {7}mg\nSodium {11}mg\nPotassium {15}g\nTotal Carbohydrate {2}g\n\tDietary Fiber {8}g\n\tSugars {19}g\nProtein {3}g\n\nVitamin A (IU): {17}\nVitamin C {9}mg\nVitamin B6 {18}mg\nVitamin B12 {10}mcg\nZinc {12}mg\nIron {13}mg\nCalcium {16}mg", dish.nutrition[0], dish.nutrition[1], dish.nutrition[2], dish.nutrition[3], dish.nutrition[4], dish.nutrition[5], dish.nutrition[6], dish.nutrition[7], dish.nutrition[8], dish.nutrition[9], dish.nutrition[10], dish.nutrition[11], dish.nutrition[12], dish.nutrition[13], dish.nutrition[14], dish.nutrition[15], dish.nutrition[16], dish.nutrition[17], dish.nutrition[18], dish.nutrition[19]);
                    nutrTxt.Text = temp;
                }
                else
                {
                    nutrTxt.FontSize = 44;
                    nutrTxt.Text = "No nutritional \ninformation is \ncurrently available \nfor this dish.";
                }
            }
            else
            {
                dishName.Visibility = Visibility.Collapsed;
            }
        }
    }
}