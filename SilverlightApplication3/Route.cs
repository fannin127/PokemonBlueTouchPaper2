using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Route
    {
        public List<InteractableObject> objects;
        public enum direction { North, South, West, East};
        public direction facing = direction.North;
        public int x1, y1, x2, y2;
     
        public bool contains(String oName)
        {
            foreach(var o in objects)
            {
                if (o.Name == oName)
                {
                    return true;
                } 
            }
            return false;
        }

        public Route(List<InteractableObject> o, List<int> e, int max, int min)
        {
            x1 = -4;
            x2 = 4;
            y1 = -4;
            y2 = 4;

            objects = o; 
            Grass l;

            DecideTrainerVisionBattleInvoke();
            sortObjects();

            foreach(InteractableObject g in objects)
            {
                try
                {
                    l = (Grass)g;
                    l.Encounterable = e;
                    l.Min = min;
                    l.Max = max;      
                } catch
                {

                }      

            }
        }

        public Route(List<InteractableObject> o) : this(o, null, 0, 0)
        {

        }

        private void sortObjects()
        {
            int i = 0;
            InteractableObject temp;

            for (int j = 0; j < objects.Count; ++j)
            {
                if (objects[j].GetType() == typeof(NPC))
                {
                    temp = objects[i];
                    objects[i] = objects[j];
                    objects[j] = temp;
                }
            }

        }

        public void addVoids()
        {

             objects.Add(new DeadSpace(y1 - (y2 - y1), x1 , null, "voida", Colors.Black, false, x2 -x1 + 1, y2 - y1));
             
            objects.Add(new DeadSpace(y2 + 1, x1, null, "voidb", Colors.Black, false, x2 -x1 + 1, y2 - y1 + 1));

            objects.Add(new DeadSpace((y1 * 2), x2 + 1, null, "voidc", Colors.Black, false, x2 - x1, 4 * (y2 - y1)));

            objects.Add(new DeadSpace((y1 * 2), x1 - (x2 - x1) , null, "voidd", Colors.Black, false, x2 - x1 , 4 * (y2 - y1)));
           
        }

        public List<Rectangle> Draw(int pLeft, int pTop)
        {
            List<Rectangle> recs = new List<Rectangle>();
            foreach(InteractableObject o in objects)
            {
                if (o.IsEnabled)
                {
                    o.Left = o.defLeft - pLeft;
                    o.Top = o.defTop - pTop;
                    if (o.GetType() == typeof(DeadSpace))
                    {
                        recs.Add(o.drawRect());
                    } else
                    {
                        if (o.Left >= y1 && o.Left <= y2 && o.Top >= x1 && o.Top <= x2)
                        {
                            recs.Add(o.drawRect());
                        }
                    }
                    
                    
                }
            }

            return recs;
        }

        public Route(List<InteractableObject> o, int x1, int x2, int y1, int y2) : this(o, null, 0, 0, x1, x2, y1, y2)
        {

        }
        public Route(List<InteractableObject> o, List<int> e, int max, int min, int x1, int x2, int y1, int y2) : this(o, e, min, max)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }
        private List<InteractableObject> shiftVoidsToEnd()
        {
            List<InteractableObject> voids = new List<InteractableObject>();
            List<InteractableObject> ret = new List<InteractableObject>();

            foreach (var o in objects)
            {
                if (o.GetType() == typeof(DeadSpace))
                {
                    voids.Add(o);
                } else
                {
                    ret.Add(o);
                }

            }

            foreach (var v in voids)
            {
                ret.Add(v);
            }

            return ret;
        }
        internal bool checkLegalMove(double top, double left, Key k)
        {
            List<InteractableObject> sorted = shiftVoidsToEnd();
            foreach (InteractableObject o in sorted)
            {
                if (k == Key.W)
                {
                    facing = direction.North;
                    if (top == o.defTop + o.Height / 40 && left >= o.defLeft && left < o.defLeft + o.Width / 40 )
                    {
                        return o.CanMoveOver;

                    }
                
                } else if (k == Key.A)
                    {
                    facing = direction.West;
                    if (left == o.defLeft + o.Width / 40 && top >= o.defTop && top < o.defTop + o.Height / 40)
                    {
                             return o.CanMoveOver;
                    }
                   

                } else if (k == Key.S)
                 {
                    facing = direction.South;
                    if (top + 1 == o.defTop && left >= o.defLeft && left < o.defLeft + o.Width / 40)
                    {
                            return o.CanMoveOver;
                    }
                   
                } else if (k == Key.D)
                  {
                    facing = direction.East;
                    if (left + 1 == o.defLeft && top >= o.defTop && top < o.defTop + o.Height / 40)
                    {
                            return o.CanMoveOver;
                    }
                    
                } else
                    return false;
            }
            return true;
        }
       
        
        public void DecideTrainerVisionBattleInvoke()
        {
            List<InteractableObject> listOfExtensions = new List<InteractableObject>();

            foreach (NPC n in objects.Where((inter) => { return inter.GetType() == typeof(NPC); }))
            {
                if (n.InvokeAmountSteps != 0)
                {
                    switch (n.facing)
                    {
                        case direction.East:
                            for (int i = n.Left; i < n.Left + n.InvokeAmountSteps; i++)
                            {
                                listOfExtensions.Add(new ExtendedNPCDistanceInteraction(n.Top, i, n.interactions.First(), "extended" + i + n.Name, Colors.Transparent));
                            }
                            break;
                        case direction.North:
                            for (int i = n.Top; i < n.Top + n.InvokeAmountSteps; i++)
                            {
                                listOfExtensions.Add(new ExtendedNPCDistanceInteraction(i, n.Left, n.interactions.First(), "extended" + i + n.Name, Colors.Transparent));
                            }
                            break;
                        case direction.South:
                            for (int i = n.Top - n.InvokeAmountSteps; i < n.Top; i++)
                            {
                                listOfExtensions.Add(new ExtendedNPCDistanceInteraction(i, n.Left, n.interactions.First(), "extended" + i + n.Name, Colors.Transparent));
                            }
                            break;
                        case direction.West:
                            for (int i = n.Left - n.InvokeAmountSteps; i < n.Left; i++)
                            {
                                listOfExtensions.Add(new ExtendedNPCDistanceInteraction(n.Top, i, n.interactions.First(), "extended" + i + n.Name, Colors.Transparent));
                            }
                            break;
                    }
                }
                
            }

            objects.AddRange(listOfExtensions);
        }
  
        

        public Interaction APressed(double top, double left)
        {
            try
            {
                foreach (NPC n in objects)
                {
                    if (n.canInteract())
                    {
                        //player below
                        if (top == n.defTop + 1 && left >= n.defLeft && left < n.defLeft + 1 && facing == direction.North)
                        {
                            return n.AInteraction;
                        }

                        //player to right
                        if (left == n.defLeft + 1 && top >= n.defTop && top < n.defTop + 1 && facing == direction.West)
                        {
                            return n.AInteraction;
                        }

                        //player above
                        if (top + 1 == n.defTop && left >= n.defLeft && left < n.defLeft + 1 && facing == direction.South)
                        {
                            return n.AInteraction;
                        }

                        //player to left
                        if (left + 1 == n.defLeft && top >= n.defTop && top < n.defTop + 1 && facing == direction.East)
                        {
                            return n.AInteraction;
                        }
                    }
                }
            } catch
            {

            }
            
            foreach(InteractableObject o in objects)
            {
                if (o.AInteraction != null && o.IsEnabled)
                {
                    //player below
                    if (top == o.defTop + 1 && left >= o.defLeft && left < o.defLeft + 1 && facing == direction.North)
                    {
                        return o.AInteraction;
                    }

                    //player to right
                    if (left == o.defLeft + 1 && top >= o.defTop && top < o.defTop + 1 && facing == direction.West)
                    {
                        return o.AInteraction;
                    }

                    //player above
                    if (top +1 == o.defTop && left >= o.defLeft && left < o.defLeft + 1 && facing == direction.South)
                    {
                        return o.AInteraction;
                    }

                    //player to left
                    if (left +1 == o.defLeft && top >= o.defTop && top < o.defTop + 1 && facing == direction.East)
                    {
                        return o.AInteraction;
                    }
                }
            }

            return null;
        }

        internal OnMoveEventInfo PerformMove(double top, double left)
        {
            foreach(var o in objects)
            {
                if (left >= o.defLeft && left < o.defLeft + 1 && top >= o.defTop && top < o.defTop + 1)
                {
                   return o.OnMoveEvent();
                }
            }

            return null;
        }
    }
}
