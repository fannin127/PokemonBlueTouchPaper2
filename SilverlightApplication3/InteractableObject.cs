using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace SilverlightApplication3
{
    public abstract class InteractableObject
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public Interaction AInteraction { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsEnabled { get; set; }
        public bool CanMoveOver { get; set; }
        public int defTop { get; set; }
        public int defLeft { get; set; }

        private Color color;
        
        public InteractableObject(int t, int l, Interaction a, string name, Color c, bool b)
        {
            Top = t;
            Left = l;
            AInteraction = a;
            Name = name;
            color = c;
            CanMoveOver = b;
            IsEnabled = true;
            defTop = t;
            defLeft = l;
        }

        public Rectangle drawRect()
        {
            Rectangle r = new Rectangle();
            r.Fill = new System.Windows.Media.SolidColorBrush(color);
            r.Name = Name;
            r.Height = Height;
            r.Width = Width;
            Canvas.SetTop(r, (Top * 40) + 160);
            Canvas.SetLeft(r, (Left * 40) + 160);

            /*
            if (CanMoveOver)
            {
                Canvas.SetZIndex(r, 5);
            }*/

            return r;
        }

        
        public virtual OnMoveEventInfo OnMoveEvent()
        {
            return null;
        }
    }
}