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
using System.Collections.Generic;

namespace SilverlightApplication3
{

    
    public class RouteStore
    {
        private Dictionary<RouteName, Route> Routes;
        private Dictionary<Route, RouteName> reverseRoutes;
        private Parser parser;

        public RouteStore()
        {
            parser = new Parser();
            Routes = new Dictionary<RouteName, Route>();
            reverseRoutes = new Dictionary<Route, RouteName>();
        }

        public void Add(RouteName n, Route r)
        {
            Routes.Add(n, r);
            reverseRoutes.Add(r, n);
        }

        public Route Get(RouteName n)
        {
            return Routes[n];
        }

        public Route Get(string rn)
        {
            return Get(parser.parseRouteName(rn));
        }

        public RouteName Get(Route n)
        {
            return reverseRoutes[n];
        }

       
    }

    public enum RouteName { Route1, Route2, PokeCentreR2, OakLab, TurnTown, PokeCentreTurnTown, PokeMartTurnTown, GymTurnTown };
}
