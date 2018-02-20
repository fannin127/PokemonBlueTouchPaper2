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
    public class UxInteraction : Interaction
    {
        Action action;
        public UxInteraction(Action a) : base ("")
        {
            action = a;
        }
        public override void Interact()
        {
            action.Invoke();
        }
    }
}
