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
    public class BattleAction
    {
        private Func<string, string> m;
        public string md;
        private Action<int> useOn;
        private int u;
        private Func<Pokeball, Pokemon, bool> capt;
        private Pokeball b;
        private Pokemon w;
        private string desc;
        public BattleAction(Func<string, string> m, string md, string text)
        {
            this.m = m;
            this.md = md;
            desc = text;
        }

        public BattleAction(Action<int> useOn, int u, string text)
        {
            this.useOn = useOn;
            desc = text;
            this.u = u;
        }

        public BattleAction(Func<Pokeball, Pokemon, bool> capt, Pokeball b, Pokemon w, string text)
        {
            this.capt = capt;
            this.b = b;
            this.w = w;
            this.desc = text;
        }

        public BattleAction(string text)
        {
            desc = text;
        }

        public bool isAttacking()
        {
            return m != null;
        }

        public string Act()
        {
            if (m != null)
            {
                desc = m.Invoke(md);
            }
            if (useOn != null)
            {
                useOn.Invoke(u);
            }
            
            if (capt != null)
            {
                if (capt.Invoke(b, w))
                {
                    desc = "The Pokemon was caught!";
                } else
                {
                    desc = "The Pokemon broke free!";
                }
            }

            return desc;
        }

    }
}
