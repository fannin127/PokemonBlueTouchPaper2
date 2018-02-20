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
    public class DeadSpace : InteractableObject
    {
        public DeadSpace(int t, int l, Interaction a, string name, Color c, bool b, int width, int height) : base(t, l, a, name, c, b)
        {
            CanMoveOver = false;
            this.Width = width * 40;
            this.Height = height * 40;
        }
    }
}
