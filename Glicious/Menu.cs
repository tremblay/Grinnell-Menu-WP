using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Glicious
{
    public class Menu
    {
        public Venue[] venues;
        public class Venue
        {
            public String name;
            public Dish[] dishes;
            public class Dish
            {
                public bool hasNutrition;
                public bool ovolacto;
                public bool vegan;
                public bool passover;
                public bool halal;
                public String name;
                public float[] nutrition;
            }
        }
    }
}
