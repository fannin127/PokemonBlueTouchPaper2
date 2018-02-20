using System;
using System.Collections.Generic;
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
    public class NPC : InteractableObject
    {
        public List<NPCInteraction> interactions;
        public int currentInter { get; set; }
        public Route.direction facing;
        public int InvokeAmountSteps = 0;

        public NPC(int t, int l, NPCInteraction a, string name, Color c) : base(t, l, a, name, c, false)
        {
            Width = 40;
            Height = 40;
            currentInter = 0;
            interactions = new List<NPCInteraction>();
            interactions.Add(a);
        }

        public void addInteraction(NPCInteraction n)
        {
            interactions.Add(n);
        }

        public bool hasNextInteraction()
        {
            return currentInter < interactions.Count;
        }

        public bool canInteract()
        {
            return IsEnabled;
        }

        public Interaction AInteraction {
            get {
                if (hasNextInteraction())
                {
                    ++currentInter;
                    return interactions[currentInter - 1];
                } else
                {
                    return interactions[interactions.Count - 1];
                }
                
            } 
        }
    }
}
