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
    public class NurseItem : Item
    {
        private Pokebuilder ms;
        public NurseItem(ItemName n, string desc, Pokebuilder ms) : base(n, desc, ItemType.NotBaggable)
        {
            this.ms = ms;
        }

        public bool heal(Pokemon[] pokes)
        {
            foreach(Pokemon p in pokes){
                if (p != null)
                {
                    p.CurrentHP = p.HP;
                    p.PP = ms.getPPArray(p.Moves);
                }
              
            }
            return true;
        }
        
    }
}
