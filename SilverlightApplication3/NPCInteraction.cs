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
    public class NPCInteraction : Interaction
    {
        Func<bool> doCode;
        public Func<bool> noCode;
        public Interaction nextInter { get; set; }

        public List<int> foeParty { get; set; }
        public List<int> foeLevels { get; set; }
        public NPCInteraction(string text) : base(text)
        {
        }

        public NPCInteraction(string text, Item i) : base(text, i)
        {
        }
        public NPCInteraction(string text, Pokemon p, Pokemon[] pa) : base(text, p, pa)
        {
        }

        public NPCInteraction(string text, Pokemon p, System.Collections.Generic.List<Pokemon> pl) : base(text, p, pl)
        {
        }

        public NPCInteraction(string text, Pokemon p, Pokemon[] pa, Func<bool> f) : base(text, p, pa)
        {
            doCode = f;
        }

        public NPCInteraction(string text, Pokemon p, System.Collections.Generic.List<Pokemon> pl, Func<bool> f) : base(text, p, pl)
        {
            doCode = f;
        }

        public NPCInteraction(string text, Item i, Func<bool> f) : base(text, i)
        {
            doCode = f;
        }

        public NPCInteraction(string text, Func<bool> f) : base(text)
        {
            doCode = f;
        }

        public NPCInteraction(string text, Func<bool> f, Func<Boolean> g) : base(text)
        {
            doCode = f;
            noCode = g;
        }

        public override void Interact()
        {
           
            if (FreePokemon != null)
            {
                FreePokemon.Experience = (int)Math.Pow(FreePokemon.level, 3);
                if (party != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (party[i] == null)
                        {
                            party[i] = FreePokemon;
                            break;
                        }
                    }
                } else
                {
                    box.Add(FreePokemon);
                }
                
                
            }

            doCode?.Invoke();

        }

        public void NoInteract()
        {
            noCode?.Invoke();
        }
    }
}
