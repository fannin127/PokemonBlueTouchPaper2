

using System;
using System.Collections.Generic;

namespace SilverlightApplication3
{


    public class Pokemon : IComparable
    {
        public Guid ID { get; private set; }
        public int Number { get; set; }
        public string Name { get; set; }

        public int level { get; set; }
        public StatusType Status { get; set; }
        public int captureRate { get; }
        public enum PokeType { Water, Grass, Fire, Normal, Poison, Psychic, Dark, Ground, Fighting, Rock, Electric, Bug, Fairy, Ghost, Dragon, Ice, Flying, Steel, Null };

        private int sleepCounter = 0;
        private int confusionCounter;
        private int perishCounter;
        public bool Infatuated;
        public bool Confused;
        public bool Perished;
        public bool Tormented;
        public bool HealBlocked;
        public bool Ingrained;
        public bool Leeched;
        public bool Leeching;
        public bool NightMared;
        public bool Embargoed;
        public bool NoImmunitied;
        public bool Telekenised;
        public Func<MoveDictionary, bool> Protection;
        public int LastProtectValue = 100;

        #region Stats

        public enum Stat { HP, Attack, Defense, SpAtk, SpDef, Speed, Accuracy, Evasion }
        public double Evasion
        {
            get
            {
                return 1 - (evasion / evasionBottom);
            }
        }
        public double Accuracy
        {
            get
            {
                return accuracy / accuracyBottom;
            }
        }

        private int hp;
        private int attack;
        private int defense;
        private int spAtk;
        private int spDef;
        private int speed;
        private int indiv = 151;
        private int effort = 151;
        private double nature = 1;

        private int changeInAtk = 0;
        private int changeInDef = 0;
        private int changeInSpAtk = 0;
        private int changeInSpDef = 0;
        private int changeInSpeed = 0;
        private int changeInAcc = 0;
        private int changeInEv = 0;
        private double evasion = 3;
        private double accuracy = 3;
        private double evasionBottom = 3;
        private double accuracyBottom = 3;
        public bool canChangeStat = true;
        public bool canSetStatus = true;

        public int CurrentHP { get; set; }

        public int HP
        {
            get
            {
                return (int)Math.Floor((2 * hp + indiv + effort) * level / (110 + level));
            }
            set
            {
                hp = value;
            }
        }

        public int Attack
        {
            get
            {
                return calcStat(attack) + changeInAtk;
            }
            set
            {
                attack = value;
            }
        }

        public int Defense
        {
            get
            {
                return calcStat(defense) + changeInDef;
            }
            set
            {
                defense = value;
            }
        }

        public int SpAtk
        {
            get
            {
                return calcStat(spAtk) + changeInSpAtk;
            }
            set
            {
                spAtk = value;
            }
        }

        public int SpDef
        {
            get
            {
                return calcStat(spDef) + changeInSpDef;
            }
            set
            {
                spDef = value;
            }
        }

        public int Speed
        {
            get
            {
                return calcStat(speed) + changeInSpeed;
            }
            set
            {
                speed = value;
            }
        }

        #endregion
        public List<EvolutionTrigger> evolvesInto { get; set; }

        public int Experience { get; set; }

        public PokeType TypeOne { get; set; }
        public PokeType TypeTwo { get; set; }
        public string[] Moves { get; set; }
        public int[] PP { get; set; }

        public Dictionary<int, string> MovesOnLevel;

        #region Constructors
        public Pokemon(int number, string name, int level, PokeType t1, PokeType t2, string[] m, int hp, int currentHP, int atk, int def, int satk, int sdef, int sped, int catchRate)
        {
            ID = Guid.NewGuid();
            Number = number;
            Name = name;
            this.level = level;
            TypeOne = t1;
            TypeTwo = t2;
            Moves = m;
            HP = hp;
            Attack = atk;
            Defense = def;
            SpAtk = satk;
            SpDef = sdef;
            Speed = sped;
            if (currentHP < 0)
            {
                currentHP = HP;
            }
            else
            {
                CurrentHP = currentHP;
            }
            this.captureRate = catchRate;
            Status = StatusType.Null;
            MovesOnLevel = new Dictionary<int, string>();
        }

        public Pokemon()
        {
        }
        #endregion

        #region Moves




        internal string newMove()
        {
            try
            {
                return MovesOnLevel[level];
            } catch
            {
                return null;
            }
        }
        public bool hasPP()
        {
            return (PP[0] > 0 && PP[1] > 0 && PP[2] > 0 && PP[3] > 0);
        }

        public string PerformMove(int moveNo)
        {
            //TODO implement
            return Moves[moveNo];

        }

        public string getMoveInfo()
        {
            string s = Moves[0] + " - PP: " + PP[0];
            s += "\n" + Moves[1] + " - PP: " + PP[1];
            s += "\n" + Moves[2] + " - PP: " + PP[2];
            s += "\n" + Moves[3] + " - PP: " + PP[3];
            return s;
        }

        public int getNextAvailableMoveSlot()
        {
            if (Moves[0] == null)
            {
                return 0;
            } else if (Moves[1] == null)
            {
                return 1;
            }
            else if (Moves[2] == null)
            {
                return 2;
            }
            else if (Moves[3] == null)
            {
                return 3;
            } else
            {
                return 99;
            }
        }
        #endregion

        #region UsingAnItem

        public void heal(int i)
        {
            CurrentHP += i;
            if (CurrentHP > HP)
            {
                CurrentHP = HP;
            }
        }
        public bool ThrowBall(Pokeball b)
        {
            double rateMod = captureRate * b.RateModifier;
            double statMod = 0;

            this.Experience = (int)Math.Pow(level, 3);

            if (Status == StatusType.Burn || Status == StatusType.Paralyze || Status == StatusType.Poison)
            {
                statMod = 5;
            }
            else if (Status == StatusType.Frozen || Status == StatusType.Sleep)
            {
                statMod = 10;
            }
            double prob = Math.Max(((3 * HP) - (2 * CurrentHP)) * (rateMod / (3 * HP)), 1) + statMod;

            Random r = new Random();
            int i = r.Next(255);


            if (i <= prob)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region StatMethods

        internal void resetStats()
        {
            changeInAcc = 0;
            changeInAtk = 0;
            changeInDef = 0;
            changeInEv = 0;
            changeInSpAtk = 0;
            changeInSpDef = 0;
            changeInSpeed = 0;
        }

        public string getStatsInfo()
        {
            string s = "HP: " + CurrentHP + "/" + HP;
            s += "\n";
            s += "Attack: " + Attack;
            s += "\n";
            s += "Defense: " + Defense;
            s += "\n";
            s += "Special Attack: " + SpAtk;
            s += "\n";
            s += "Special Defense: " + SpDef;
            s += "\n";
            s += "Speed: " + Speed;

            return s;
        }
        public Dictionary<string, int> getBaseStats()
        {
            Dictionary<String, int> d = new Dictionary<string, int>();
            d.Add("Atk", attack);
            d.Add("Def", defense);
            d.Add("SpAtk", spAtk);
            d.Add("SpDef", spDef);
            d.Add("Speed", speed);
            d.Add("HP", hp);

            return d;
        }

        private int calcStat(int val)
        {
            return (int)Math.Floor(Math.Floor((2 * val + indiv + effort) * level / (105)) * nature);
        }
        public bool changeStat(Stat s, int amount, bool force)
        {
            if (canChangeStat || force)
            {
                if (s == Stat.HP)
                {
                    CurrentHP += amount;
                }
                else if (s == Stat.Attack)
                {
                    changeInAtk += amount;
                    if (changeInAtk < -6)
                    {
                        changeInAtk = -6;
                    }
                    else if (changeInAtk > 6)
                    {
                        changeInAtk = 6;
                    }
                }
                else if (s == Stat.Defense)
                {
                    changeInDef += amount;
                    if (changeInDef < -6)
                    {
                        changeInDef = -6;
                    }
                    else if (changeInDef > 6)
                    {
                        changeInDef = 6;
                    }
                }
                else if (s == Stat.SpAtk)
                {
                    changeInSpAtk += amount;
                    if (changeInSpAtk < -6)
                    {
                        changeInSpAtk = -6;
                    }
                    else if (changeInSpAtk > 6)
                    {
                        changeInSpAtk = 6;
                    }
                }
                else if (s == Stat.SpDef)
                {
                    changeInSpDef += amount;
                    if (changeInSpDef < -6)
                    {
                        changeInSpDef = -6;
                    }
                    else if (changeInSpDef > 6)
                    {
                        changeInSpDef = 6;
                    }
                }
                else if (s == Stat.Speed)
                {
                    changeInSpeed += amount;
                    if (changeInSpeed < -6)
                    {
                        changeInSpeed = -6;
                    }
                    else if (changeInSpeed > 6)
                    {
                        changeInSpeed = 6;
                    }
                }
                else if (s == Stat.Accuracy)
                {
                    if (amount < 0)
                    {
                        if (accuracy > 3)
                        {
                            --accuracy;
                        }
                        else
                        {
                            ++accuracyBottom;
                        }
                    }
                    else
                    {
                        if (accuracyBottom > 3)
                        {
                            --accuracyBottom;
                        }
                        else
                        {
                            ++accuracy;
                        }
                    }
                }
                else if (s == Stat.Evasion)
                {
                    if (amount < 0)
                    {
                        if (evasion > 3)
                        {
                            --evasion;
                        }
                        else
                        {
                            ++evasionBottom;
                        }
                    }
                    else
                    {
                        if (evasionBottom > 3)
                        {
                            --evasionBottom;
                        }
                        else
                        {
                            ++evasion;
                        }
                    }
                }
            }
            

            return false;
        }


        #endregion     

        #region OverridenMethods
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override string ToString()
        {
            return Name + " - Lvl:" + level;
        }

        public int CompareTo(object obj)
        {
            return ((Pokemon)obj).ID.CompareTo(ID);
        }

        #endregion

        #region Evolution

        public int toEvolve { get; private set; }
        internal bool needToEvolve()
        {
            foreach (EvolutionTrigger trig in evolvesInto)
            {
                if (trig.evolveByLevel())
                {
                    if (level >= trig.getEvolveLevel())
                    {
                        toEvolve = trig.evolveInto;
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool needToEvolve(ItemName i)
        {
            foreach(EvolutionTrigger trig in evolvesInto)
            {
                if (trig.evolveByThisStone(i))
                {
                    toEvolve = trig.evolveInto;
                    return true;
                }
            }
            return false;
        }


        #endregion

        #region Status

        public bool isGrounded()
        {
            //levitate ability, air balloon, magnet rise or telekenis
            if (Telekenised || TypeOne == PokeType.Flying || TypeTwo == PokeType.Flying)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsConfused()
        {
            int nine = 99;
            return !statusCountDown(ref nine, 50);
        }

        public bool snapOut()
        {
            if (Confused)
            {
                return statusCountDown(ref confusionCounter, 25);
            }
            return true;
        }

        public bool statusCountDown(ref int counter, int threshold)
        {
            if (counter == 0)
            {
                Status = StatusType.Null;
                return true;
            }
            else
            {
                --counter;
                Random rand = new Random();
                if (rand.Next(100) <= threshold)
                {
                    counter = 0;
                    Status = StatusType.Null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool tryAndWake()
        {
            return statusCountDown(ref sleepCounter, 20);
        }
        public void switchOutResetCounters()
        {
            confusionCounter = 0;

        }

        public bool setStatus(StatusType s, bool force)
        {
            if (canSetStatus || force)
            {
                if (s == StatusType.Burn || s == StatusType.Frozen || s == StatusType.Paralyze || s == StatusType.Poison || s == StatusType.Sleep)
                {
                    if (Status == StatusType.Null)
                    {
                        Status = s;

                        if (s == StatusType.Sleep)
                        {
                            sleepCounter = 4;
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    switch (s)
                    {
                        case StatusType.Confusion:
                            confusionCounter = 4;
                            Confused = true;
                            return true;
                        case StatusType.HealBlock:
                            HealBlocked = true;
                            return true;
                        case StatusType.Infatuation:
                            Infatuated = true;
                            return true;
                        case StatusType.Ingrain:
                            Ingrained = true;
                            return true; ;
                        case StatusType.LeechSeed:
                            Leeched = true;
                            return true;
                        case StatusType.NightMare:
                            if (Status == StatusType.Sleep)
                            {
                                NightMared = true;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        case StatusType.NoHoldItem:
                            Embargoed = true;
                            return true;
                        case StatusType.NoImmunity:
                            NoImmunitied = true;
                            return true;
                        case StatusType.PerishSong:
                            Perished = true;
                            perishCounter = 3;
                            return true; ;
                        case StatusType.Telekenesis:
                            Telekenised = true;
                            return true;
                        case StatusType.Torment:
                            Tormented = true;
                            return true;
                        default:
                            return false;
                    }
                }
            }

            return false;
        
        }

        #endregion

        #region Battle
        public string EndOfTurnEffects(int leechingAmount)
        {
            string ret = "";

            if (Status == StatusType.Poison)
            {
                CurrentHP -= (int)Math.Ceiling((0.0625 * HP));
                ret += Name + " was hurt by poison";
            }
            else if (Status == StatusType.Burn)
            {
                CurrentHP -= (int)Math.Ceiling((0.0625 * HP));
                ret += Name + " was hurt by burn";
            }

            if (Ingrained)
            {
                CurrentHP += (int)Math.Ceiling(0.0625 * HP);
                if (CurrentHP > HP)
                {
                    CurrentHP = HP;
                }
                else
                {
                    ret += Name + " regained health from ingrain! \n";
                }

            }

            if (Leeched)
            {
                CurrentHP -= (int)Math.Ceiling(0.33 * CurrentHP);
                ret += Name + " was sapped by Leech Seed! \n";
            }

            if (Leeching)
            {
                CurrentHP += leechingAmount;
                if (CurrentHP > HP)
                {
                    CurrentHP = HP;
                }
                ret += Name + " gained health from Leech Seed \n";
            }

            if (NightMared)
            {
                CurrentHP -= (int)Math.Ceiling(0.25 * HP);
                ret += Name + " is having bad dreams! \n";
            }

            if (Perished)
            {
                --perishCounter;
                ret += ret += Name + "'s Perish Count fell to " + perishCounter + "! \n";
                if (perishCounter == 0)
                {
                    CurrentHP = 0;
                }
            }

            return ret;
        }

        public string stopAttackDueToStatus()
        {
            if (Status != StatusType.Null)
            {
                Random ranom = new Random();
                if (Status == StatusType.Frozen)
                {
                    //do check
                    if (ranom.Next(100) <= 20)
                    {
                        return "";
                    }
                    return Name + " is frozen solid!";
                }
                else if (Status == StatusType.Infatuation)
                {
                    //do check
                    if (ranom.Next(100) <= 50)
                    {
                        return Name + " is immobilised by love!";
                    }
                    else
                    {
                        return "";
                    }

                }
                else if (Status == StatusType.Sleep)
                {
                    if (!tryAndWake())
                    {
                        return Name + " is fast asleep!";
                    }
                    return Name + " woke up!";
                }
                else if (Status == StatusType.Paralyze)
                {

                    if (ranom.Next(100) <= 25)
                    {
                        return Name + " is fully paralysed!";
                    }
                    return "";
                }
            }
            return "";
        }



        internal void foeSwitchOutResetStatus()
        {
            if (Status == StatusType.Infatuation)
            {
                Status = StatusType.Null;
            }

        }


        #endregion




    }

    public enum StatusType { Sleep, Paralyze, Confusion, Poison, Burn, Frozen, LeechSeed, NightMare, NoImmunity, PerishSong, Infatuation, Torment, Ingrain, NoHoldItem, HealBlock, Telekenesis, Leeching, Null }
}