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

namespace SilverlightApplication3
{
    public class MoveRoute : InteractableObject
    {
        private RouteName newRoute;
        private int nextTop;
        private int nextLeft;

        public MoveRoute(int t, int l, Interaction a, string name, Color c, RouteName nr, int top, int left) : base(t, l, a, name, c, true)
        {
            Width = 40;
            Height = 40;
            newRoute = nr;
            nextLeft = left;
            nextTop = top;
        }

        public override OnMoveEventInfo OnMoveEvent()
        {
            OnMoveEventInfo omi = new OnMoveEventInfo();
            omi.NewRoute = newRoute;
            omi.PlayerLocLeft = nextLeft;
            omi.PlayerLocTop = nextTop;
            return omi;
        }
    }
}
