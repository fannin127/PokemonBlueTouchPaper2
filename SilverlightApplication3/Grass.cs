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
    public class Grass : InteractableObject
    {
        public List<int> Encounterable;
        public int Max { get; set; }
        public int Min { get; set; }
        private Pokebuilder pb;
        public Grass(int t, int l, Interaction a, String name, Color c, Pokebuilder pb) : base(t, l, a, name, c, true)
        {
            Width = 40;
            Height = 40;
            this.pb = pb;
        }



        public override OnMoveEventInfo OnMoveEvent()
        {

            if (isEncounter())
            {
                OnMoveEventInfo omi = new OnMoveEventInfo();
                Random r = new Random();
                omi.Encounter = pb.pokesForRoute(Encounterable, Max, Min).ToArray()[r.Next(Encounterable.Count)];
                return omi;
            }

            return null;
 
        }
        public bool isEncounter()
        {
            double likeliness = 0.2;

            Random rand = new Random();
            rand.NextDouble();

            if (rand.NextDouble() <= likeliness)
            {
                return true;
            } else
            {
                return false;
            }
            
        }
    }
}
