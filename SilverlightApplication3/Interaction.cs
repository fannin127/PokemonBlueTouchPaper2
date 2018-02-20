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
    public abstract class Interaction
    {
        public string Text;
        public Item ReturnItem;
        public Pokemon FreePokemon;
        internal Pokemon[] party;
        internal List<Pokemon> box;
        public bool Forced = false;
        public string yesText { get; set; }
        public string noText { get; set; }
        public bool closeable { get; set; }
        public bool hasNoOption { get; internal set; }



        public Interaction(string text)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
        }

        public Interaction(string text, Item returnItem)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
            ReturnItem = returnItem;
        }

        public Interaction(string text, Pokemon poke, Pokemon[] party)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
            poke.CurrentHP = poke.HP;
            FreePokemon = poke;
            this.party = party;
        }

        public Interaction(string text, Pokemon poke, List<Pokemon> box)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
            FreePokemon = poke;
            this.box = box;
        }

        public Interaction(string text, Func<bool> predicate)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
        }

        public Interaction(string text, Item returnItem, Func<bool> predicate)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
            ReturnItem = returnItem;
        }

        public Interaction(string text, Pokemon poke, Pokemon[] party, Func<bool> predicate)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
            FreePokemon = poke;
            this.party = party;
        }

        public Interaction(string text, Pokemon poke, List<Pokemon> box, Func<bool> predicate)
        {
            yesText = "Yes";
            noText = "No";
            Text = text;
            FreePokemon = poke;
            this.box = box;
        }


        public abstract void Interact();
        

    }
}
