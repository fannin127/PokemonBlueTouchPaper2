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
    public class MoveDictionary : IComparable<MoveDictionary>
    {
        public string Name { get; set; }
        public int damage { get; set; }
        public Pokemon.PokeType Type { get;  set;}

        public MoveStore.Speciality Speciality { get; set; }
        public MoveCategory StatusValue { get; set; }
        public int Accuracy { get; set; }
        public int Priority { get; set; }

        public bool ToFoe { get; private set; }
        public List<Pokemon.Stat> StatsToChange { get; private set; }
        public Func<MoveDictionary, bool> Protection { get; private set; }
        public int PP { get; set; }
        public StatusType Status { get; private set; }
        public int statAmount { get; private set; }
        public bool statToFoe { get; private set; }

        public bool AttackAll { get; private set; }

        public bool IncreasinglyUnlikely { get; private set; }
        public int StatLikeliness
        {
            get
            {
                if (_statLikeliness > 0)
                {
                    return _statLikeliness;
                }
                else
                {
                    return 100;
                }
            }
            private set
            {
                _statLikeliness = value;
            }
        }
        private int _statLikeliness;
        public int StatusLikliness
        {
            get
            {
                if (_statusLikeliness > 0)
                {
                    return _statusLikeliness;
                }
                else
                {
                    return 100;
                }
            }
            private set
            {
                _statusLikeliness = value;
            }
        }
        private int _statusLikeliness;
        public int DrainAmount;

        public Action<Pokemon, Pokemon> uniqueActionTwoPokemon;
        public Action<bool> uniqueActionSetSomeBattleEffect;
        public Func<Pokemon, string> uniqueActionOnePokemon;

        public MoveDictionary(string name, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int pp, int acc, int pri)
        {
            Name = name;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            Priority = pri;
        }
        public MoveDictionary(string name, int d, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, int pri, string toFoe)
        {
            Name = name;
            damage = d;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            PP = pp;
            if (toFoe.Contains("all"))
            {
                AttackAll = true;
            }
            ToFoe = Parser.parseToFoe(toFoe);
            Priority = pri;
        }

        public MoveDictionary(string name, int d, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, List<Pokemon.Stat> statToChange, string toFoe, int pri) : this(name, d, pp, t, b, stat, acc, pri, toFoe)
        {
            StatsToChange = statToChange;
            if (toFoe.Contains("all"))
            {
                AttackAll = true;
            }
            ToFoe = Parser.parseToFoe(toFoe);
            statToFoe = Parser.parseToFoe(toFoe);
            statAmount = d;
            StatLikeliness = 100;
        }

        public MoveDictionary(string name, int damage, int statValue, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory category, int acc, List<Pokemon.Stat> statsToChange, string atkToFoe, string statToFoe, int statLikeliness, int pri)
        {
            Name = name;
            this.damage = damage;
            Type = t;
            Speciality = b;
            StatusValue = category;
            Accuracy = acc;
            PP = pp;

            statAmount = statValue;
            StatsToChange = statsToChange;
            if (atkToFoe.Contains("all"))
            {
                AttackAll = true;
            }
            ToFoe = Parser.parseToFoe(atkToFoe);
            StatLikeliness = statLikeliness;
            this.statToFoe = Parser.parseToFoe(statToFoe);
            Priority = pri;
        }

        public MoveDictionary(string name, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, StatusType status, string statusToFoe, List<Pokemon.Stat> statsToChange, int statVal, string statToFoe, int pri)
        {
            Name = name;
            PP = pp;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            Status = status;
            if (statusToFoe.Contains("all"))
            {
                AttackAll = true;
            }
            ToFoe = Parser.parseToFoe(statusToFoe);
            StatsToChange = statsToChange;
            statAmount = statVal;
            this.statToFoe = Parser.parseToFoe(statToFoe);
            Priority = pri;
        }

        public MoveDictionary(string name, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, StatusType status, string toFoe, int pri) : this(name, pp, 0, t, b, stat, acc, pri, toFoe)
        {
            this.Status = status;
            if (toFoe.Contains("all"))
            {
                AttackAll = true;
            }
            ToFoe = Parser.parseToFoe(toFoe);
            StatusLikliness = 100;
        }

        public MoveDictionary(string name, int d, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, StatusType status, string toFoe, int likeliness, int pri) : this (name, d, pp, t, b, stat, acc, pri, toFoe)
        {
            this.Status = status;
            this.StatusLikliness = likeliness;
        }

        public MoveDictionary(string name, int d, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, string toFoe, int drainAmount, int pri) :this (name, d, pp, t, b, stat, acc, pri, toFoe)
        {
            this.DrainAmount = drainAmount;
        }

        public MoveDictionary(string name, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, Action<Pokemon, Pokemon> act, int pri)
        {
            Name = name;
            PP = pp;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            uniqueActionTwoPokemon = act;
            Priority = pri;
        }

        public MoveDictionary(string name, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, Action<bool> act, int pri)
        {
            Name = name;
            PP = pp;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            uniqueActionSetSomeBattleEffect = act;
            Priority = pri;
        }

        public MoveDictionary(string name, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, string toFoe, Func<Pokemon, string> act, int pri)
        {
            Name = name;
            PP = pp;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            uniqueActionOnePokemon = act;
            if (toFoe.Contains("all"))
            {
                AttackAll = true;
            }
            ToFoe = Parser.parseToFoe(toFoe);
            Priority = pri;
        }

        public MoveDictionary(string name, int pp, Pokemon.PokeType t, MoveStore.Speciality b, MoveCategory stat, int acc, Func<MoveDictionary, bool> protection, int pri)
        {
            Name = name;
            PP = pp;
            Type = t;
            Speciality = b;
            StatusValue = stat;
            Accuracy = acc;
            Protection = protection;
            Priority = pri;
            IncreasinglyUnlikely = true;
        }

        public int CompareTo(MoveDictionary other)
        {
            if (other.Name != Name)
            {
                return 1;
            } else
            {
                return 0;
            }
        }
    }
}
