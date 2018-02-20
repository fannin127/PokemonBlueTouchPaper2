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
    public class ExtendedNPCDistanceInteraction : NPC
    {
        NPCInteraction interaction;
        public ExtendedNPCDistanceInteraction(int t, int l, NPCInteraction a, string name, Color c) : base(t, l, a, name, c)
        {
            this.IsEnabled = false;
            interaction = a;
            this.CanMoveOver = true;
        }

        override
        public OnMoveEventInfo OnMoveEvent()
        {
            return new OnMoveEventInfo() { inter = interaction };
        }
    }
}
