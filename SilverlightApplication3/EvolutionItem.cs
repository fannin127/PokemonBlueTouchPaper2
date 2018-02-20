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
    public class EvolutionItem : HealAndStatusItem
    {
        private List<int> pokesItCanEvolve;
        public EvolutionItem(ItemName n, string d, List<int> list) : base(n, d, 0)
        {
            pokesItCanEvolve = list;
        }

        public bool CanEvolve(int i)
        {
            return pokesItCanEvolve.Contains(i);
        }
    }
}
