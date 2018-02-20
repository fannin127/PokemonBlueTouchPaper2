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
    public class EvolutionTrigger
    {
        private ItemName i = ItemName.Null;
        private int lvl = -99;
        public int evolveInto = 0;
        
        public EvolutionTrigger(int into)
        {
            //trade
            evolveInto = into;
        }
        public EvolutionTrigger(int i, int into)
        {
            lvl = i;
            evolveInto = into;
        }

        public EvolutionTrigger(ItemName name, int into)
        {
            i = name;
            evolveInto = into;
        }

        public bool evolveByLevel()
        {
            return lvl != -99;
        }

        public bool evolveByStone()
        {
            return i != ItemName.Null;
        }

        public bool evolveByThisStone(ItemName name)
        {
            return i == name;
        }

        public bool evolveByTrade()
        {
            return !(evolveByLevel() || evolveByStone());
        }


        public int getEvolveLevel()
        {
            return lvl;
        }

        public ItemName getEvolveItem()
        {
            return i;
        }

    }
}
