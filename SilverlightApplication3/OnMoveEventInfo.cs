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
    public class OnMoveEventInfo
    {
        public Pokemon Encounter {
            get;
            set; }
        public RouteName NewRoute { get; set; }

        public NPCInteraction inter;
        public int PlayerLocTop { get; set; }
        public int PlayerLocLeft { get; set; }
    }
}
