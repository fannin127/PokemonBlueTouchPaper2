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
using System.Linq;


namespace SilverlightApplication3
{
   
    public class BattleController
    {
        private Pokemon[] party;
        public Pokemon CurrentInBattle;
        private List<Pokemon> foe;
        public Pokemon CurrentFoe
        { get
            {
                return foe.First();  
            } }

        private bool lost;
        public bool notAttacking;
        public HashSet<Pokemon> pokesThatBattled;
        public bool finalMoveFinished;
        public HashSet<Func<Pokemon, string>> myEntryEffects = new HashSet<Func<Pokemon, string>>();
        public HashSet<Func<Pokemon, string>> foeEntryEffects = new HashSet<Func<Pokemon, string>>();

        public Queue<Action> battleQueue = new Queue<Action>();

        private enum effectiveness { normal, notVery, immune, super }
        public MoveDictionary FoeNextAttack { get; set; }
        public int FoeNextAttackI { get; set; }
        private effectiveness effective;
        public Dictionary<string, bool> BattleConditions;
        private Dictionary<string, int> ConditionCounters;

        private Dictionary<string, int> FoeSideConditionCounters;
        private Dictionary<string, int> MySideConditionCounters;


        private int MyLastUsedVal = 0;
        private int FoeLastUsedVal = 0;

        private string MyLastUsedMove = "";
        private string FoeLastUsedMove = "";

        public bool isWildBattle;
        public BattleController(List<Pokemon> foe, Pokemon[] party)
        {
            this.party = party;
            this.foe = foe;

            foreach (Pokemon p in this.foe)
            {
                p.CurrentHP = p.HP;
            }

            lost = false;
            notAttacking = false;
            pokesThatBattled = new HashSet<Pokemon>();

            CurrentInBattle = getNextAlive();
            pokesThatBattled.Add(CurrentInBattle);

            effective = effectiveness.normal;
            finalMoveFinished = true;

            BattleConditions = new Dictionary<string, bool>();
            ConditionCounters = new Dictionary<string, int>();

            BattleConditions.Add("sandstorm", false); //d
            BattleConditions.Add("rain-dance", false); //d
            BattleConditions.Add("sunny-day", false); //d
            BattleConditions.Add("hail", false); //d
            BattleConditions.Add("mud-sport", false);  //d
            BattleConditions.Add("water-sport", false); //d
            BattleConditions.Add("gravity", false); //nqd
            BattleConditions.Add("trick-room", false); //see todo list
            BattleConditions.Add("wonder-room", false); //d
            BattleConditions.Add("ion-deluge", false); //d
            BattleConditions.Add("grassy-terrain", false);//d
            BattleConditions.Add("misty-terrain", false);//not fully, need to stop yawn and rest
            BattleConditions.Add("fairy-lock", false);  //not implemented, stops switch out
            BattleConditions.Add("electric-terrain", false);//not fully, same as misty
            BattleConditions.Add("psychic-terrain", false);//not fully, stops priortiy moves
            BattleConditions.Add("magic-room", false); //not implemented, stops use of held items, which currently aren't implemeneted

            ConditionCounters.Add("sandstorm", 0);
            ConditionCounters.Add("rain-dance", 0);
            ConditionCounters.Add("sunny-day", 0);
            ConditionCounters.Add("hail", 0);
            ConditionCounters.Add("mud-sport", 0);
            ConditionCounters.Add("water-sport", 0);
            ConditionCounters.Add("gravity", 0);
            ConditionCounters.Add("trick-room", 0);
            ConditionCounters.Add("wonder-room", 0);
            ConditionCounters.Add("ion-deluge", 0);
            ConditionCounters.Add("grassy-terrain", 0);
            ConditionCounters.Add("misty-terrain", 0);
            ConditionCounters.Add("fairy-lock", 0);
            ConditionCounters.Add("electric-terrain", 0);
            ConditionCounters.Add("psychic-terrain", 0);
            ConditionCounters.Add("magic-room", 0);

            FoeSideConditionCounters = new Dictionary<string, int>();
            FoeSideConditionCounters.Add("reflect", 0);
            FoeSideConditionCounters.Add("light-screen", 0);
            FoeSideConditionCounters.Add("mist", 0);
            FoeSideConditionCounters.Add("safeguard", 0);
            FoeSideConditionCounters.Add("tailwind", 0);
            FoeSideConditionCounters.Add("lucky-chant", 0);
            FoeSideConditionCounters.Add("Aurora-veil", 0);

            MySideConditionCounters = new Dictionary<string, int>();
            MySideConditionCounters.Add("reflect", 0);
            MySideConditionCounters.Add("light-screen", 0);
            MySideConditionCounters.Add("mist", 0);
            MySideConditionCounters.Add("safeguard", 0);
            MySideConditionCounters.Add("tailwind", 0);
            MySideConditionCounters.Add("lucky-chant", 0);
            MySideConditionCounters.Add("Aurora-veil", 0);
        }

        private Pokemon getNextAlive()
        {
            foreach (Pokemon p in party)
            {
                if (p.CurrentHP > 0)
                {
                    return p;
                }
            }

            return null;
        }


        public Dictionary<string, string> DealWithDamage()
        {
            if (CurrentInBattle.CurrentHP < 0)
            {
                CurrentInBattle.CurrentHP = 0;
            }
            if (foe.First().CurrentHP < 0)
            {
                foe.First().CurrentHP = 0;
            }

            Dictionary<string, string> ret = new Dictionary<string, string>();

            ret.Add("MyStatus", StatusTypeToString(CurrentInBattle.Status));
            ret.Add("FoeStatus", StatusTypeToString(CurrentFoe.Status));
            ret.Add("MyName", CurrentInBattle.Name + " Lvl:" + CurrentInBattle.level);
            ret.Add("FoeName", CurrentFoe.Name + " Lvl:" + CurrentFoe.level);
            ret.Add("MyHP", CurrentInBattle.CurrentHP.ToString() + " / " + CurrentInBattle.HP.ToString());
            ret.Add("FoeHP", CurrentFoe.CurrentHP.ToString() + " / " + CurrentFoe.HP.ToString());

            notAttacking = false;
            effective = effectiveness.normal;

            

            return ret;
        }
        MoveStore ms = new MoveStore();
        private MoveDictionary getRandomMove()
        {
            
            Random r = new Random();
            return ms.get(r.Next(1, 718));
        }

        public Dictionary<string, string> generalAttack(Pokemon attacker, Pokemon defender, MoveDictionary move, int mNumber, string foeOrMe)
        {
            Dictionary<string, string> retDic = new Dictionary<string, string>();
            if (move.Name == "metronome")
            {
                Dictionary<string, string> dir = generalAttack(attacker, defender, getRandomMove(), mNumber, foeOrMe);
                dir["return"] = attacker.Name + " used Metronome!\n" + dir["return"];
                return dir;
            } else if (move.Name == "splash")
            {
                retDic.Add("return", attacker.Name + " used splash!\nBut nothing happened!");
                return retDic;
            } else if (move.Name == "mirror-move")
            {
                if (FoeLastUsedMove == string.Empty)
                {
                    retDic.Add("return", attacker.Name + " used mirror move!\nBut it failed!");
                    return retDic;
                }
                MoveDictionary md = ms.get(FoeLastUsedMove);
                if (md.ToFoe)
                {
                    Dictionary<string, string> dir = generalAttack(attacker, defender, ms.get(FoeLastUsedMove), mNumber, foeOrMe);
                    dir["return"] = attacker.Name + " user mirror move!\n" + dir["return"];
                    return dir;
                } else
                {
                    retDic.Add("return", attacker.Name + " used mirror move!\nBut it failed!");
                    return retDic;
                }
               
            }
            
            StatusType initStat = attacker.Status;
            if (attacker.Status != StatusType.Null)
            {
                string stopped = attacker.stopAttackDueToStatus();
                if (stopped != "")
                {
                    retDic.Add("return", stopped);
                    return retDic;
                }
            }
            else
            {
                if (foeOrMe == "")
                {
                    retDic.Add("MyStatusLabel","");
                }
                else
                {
                    retDic.Add("FoeStatusLabel", "");
                }
            }
            if (mNumber != -1)
            {
                attacker.PP[mNumber] = Math.Max(--attacker.PP[mNumber], 0);
            }

            string ret = "";

            Random r = new Random();

            if (move.IncreasinglyUnlikely)
            {
                if (foeOrMe == "")
                {
                    if (MyLastUsedMove == move.Name)
                    {
                        if (r.Next(0, 100) > 100 - MyLastUsedVal)
                        {
                            retDic.Add("return", ret + "But it failed!");
                            return retDic;
                        }
                    }
                    
                } else
                {
                    if (FoeLastUsedMove == move.Name)
                    {
                        if (r.Next(0, 100) > 100 - FoeLastUsedVal)
                        {
                            retDic.Add("return", ret + "But it failed!");
                            return retDic;
                        }
                    }
                    
                }
            }
            if (move.StatusValue == MoveCategory.Switch || move.Name == "teleport")
            {
                //check immunutiy due to abilities and such
                if (attacker.level < defender.level)
                {
                    ret = "The Move Failed!";
                    retDic.Add("return", ret);                    
                    return retDic;
                } else
                {
                    if (isWildBattle)
                    {
                        retDic.Add("END_BATTLE_NOW", "END_BATTLE_NOW");
                        return retDic;
                    } else
                    {
                        if (move.Name == "teleport")
                        { 
                            retDic.Add("return", "The move failed!");
                            return retDic;
                        }
                        //sort out switching 
                        retDic.Add("return", "The foe was forced to switch!");
                        return retDic;
                    }
                }

            }

            
            
            double rd = r.NextDouble();
            ret += foeOrMe + " " + attacker.Name + " used " + move.Name + "\n";
            double accuracyMod = 1;
            if (BattleConditions["gravity"])
            {
                accuracyMod = 1.67;
            }

            if (defender.Protection != null && defender.Protection(move))
            {
                ret += "The defender is protected!";
                retDic.Add("return", ret);
                return retDic;
            } 
            if (rd <= ((double)move.Accuracy / (double)100) * (attacker.Accuracy * accuracyMod) * (1 - defender.Evasion) || move.Accuracy < 0)
            {
                if (!attacker.snapOut())
                {
                    if (attacker.IsConfused())
                    {
                        attacker.CurrentHP = calculateDamage(new MoveDictionary("ConfusedMoveAttackYourself", 40, 99999, Pokemon.PokeType.Null, MoveStore.Speciality.Physical, MoveCategory.Attacking, 1000, 0, "user"), attacker, attacker);
                        retDic.Add("return", attacker.Name + " hit itself in its confusion! \n");
                        return retDic;
                    }

                }
                else
                {
                    if (initStat == StatusType.Confusion)
                    {
                        if (foeOrMe == "")
                        {
                            retDic.Add("MyStatusLabel","");
                        }
                        else
                        {
                            retDic.Add("FoeStatusLabel", "");
                        }
                        ret += attacker.Name + " snapped out of confusion! \n";
                    }
                }
                if (!isImmune(move, defender))
                {
                    if (move.StatusValue == MoveCategory.Attacking || move.StatusValue == MoveCategory.DamageAndRaise || move.StatusValue == MoveCategory.DamageAndLower || move.StatusValue == MoveCategory.DamageAndAilement || move.StatusValue == MoveCategory.Absorbing)
                    {

                        int dam = calculateDamage(move, attacker, defender);

                        defender.CurrentHP = defender.CurrentHP - dam;

                        if (defender.CurrentHP < 0)
                        {
                            defender.CurrentHP = 0;
                        }

                        if (move.Type == Pokemon.PokeType.Fire)
                        {
                            if (defender.Status == StatusType.Frozen)
                            {
                                defender.Status = StatusType.Null;
                                if (foeOrMe == "")
                                {
                                    retDic.Add("MyStatusLabel", "");
                                }
                                else
                                {
                                    retDic.Add("FoeStatusLabel", "");
                                }
                            }
                        }

                        if (move.StatusValue == MoveCategory.Absorbing)
                        {
                            attacker.CurrentHP += (int)Math.Ceiling((((double)move.DrainAmount / 100) * (double)dam));

                            if (attacker.CurrentHP > attacker.HP)
                            {
                                attacker.CurrentHP = attacker.HP;
                            }
                        }
                    }

                    if (move.StatusValue == MoveCategory.StatChange || move.StatusValue == MoveCategory.DamageAndRaise || move.StatusValue == MoveCategory.DamageAndLower || move.StatusValue == MoveCategory.StatusAndRaiseStats)
                    {
                        Random rand = new Random();
                        int d = rand.Next(100);
                        if (d < move.StatLikeliness)
                        {
                            if (move.statToFoe)
                            {
                                foreach (Pokemon.Stat stat in move.StatsToChange)
                                {
                                    defender.changeStat(stat, move.statAmount, false);
                                }
                            }
                            else
                            {
                                foreach (Pokemon.Stat stat in move.StatsToChange)
                                {
                                    attacker.changeStat(stat, move.statAmount, false);
                                }
                            }
                        }


                    }

                    if (move.StatusValue == MoveCategory.Status || move.StatusValue == MoveCategory.DamageAndAilement || move.StatusValue == MoveCategory.StatusAndRaiseStats)
                    {
                        Random rand = new Random();
                        if (rand.Next(100) <= move.StatusLikliness)
                        {
                            if (defender.setStatus(move.Status, false))
                            {
                                ret += defender.Name + " has the status: " + move.Status.ToString() + "\n";
                                if (move.Status == StatusType.Burn || move.Status == StatusType.Frozen || move.Status == StatusType.Frozen || move.Status == StatusType.Paralyze || move.Status == StatusType.Poison || move.Status == StatusType.Sleep)
                                    if (foeOrMe != "")
                                    {
                                        retDic.Add("MyStatusLabel", move.Status.ToString());
                                    }
                                    else
                                    {
                                        retDic.Add("FoeStatusLabel", move.Status.ToString());
                                    }
                            }
                            else
                            {
                                ret += "But it failed! \n";
                            }
                        }


                    }

                    if (move.StatusValue == MoveCategory.KO)
                    {
                        ret += "It's a one-hit-KO!";
                        defender.CurrentHP = 0;
                    }

                    if (move.StatusValue == MoveCategory.Healing)
                    {
                        if (move.ToFoe)
                        {
                            defender.CurrentHP += (int)Math.Ceiling((double)((double)((double)move.damage / 100) * (double)defender.HP));
                            ret += defender.Name + " had its health restored! \n";
                            if (defender.CurrentHP > defender.HP)
                            {
                                defender.CurrentHP = defender.HP;
                            }
                        }
                        else
                        {
                            attacker.CurrentHP += (int)Math.Ceiling((double)((double)((double)move.damage / 100) * (double)attacker.HP));
                            ret += attacker.Name + " had its health restored! \n";
                            if (attacker.CurrentHP > attacker.HP)
                            {
                                attacker.CurrentHP = attacker.HP;
                            }
                        }
                    }

                    if (move.StatusValue == MoveCategory.Field)
                    {
                        if (move.Name == "Haze")
                        {
                            move.uniqueActionTwoPokemon(attacker, defender);
                        } else
                        {
                            if (move.Name == "hail" || move.Name == "sunny-day" || move.Name == "rain-dance" || move.Name == "sandstorm")
                            {
                                BattleConditions["hail"] = false;
                                BattleConditions["sunny-day"] = false;
                                BattleConditions["rain-dance"] = false;
                                BattleConditions["sandstorm"] = false;
                                ConditionCounters["hail"] = 0;
                                ConditionCounters["sunny-day"] = 0;
                                ConditionCounters["rain-dance"] = 0;
                                ConditionCounters["sandstorm"] = 0;
                            }

                            if (move.Name == "grassy-terrain" || move.Name == "misty-terrain" || move.Name == "electric-terrain" || move.Name == "psychic-terrain")
                            {
                                BattleConditions["grassy-terrain"] = false;
                                BattleConditions["misty-terrain"] = false;
                                BattleConditions["electric-terrain"] = false;
                                BattleConditions["psychic-terrain"] = false;
                                ConditionCounters["psychic-terrain"] = 0;
                                ConditionCounters["misty-terrain"] = 0;
                                ConditionCounters["electric-terrain"] = 0;
                                ConditionCounters["grassy-terrain"] = 0;
                            }

                            BattleConditions[move.Name] = true;

                            if (move.Name == "ion-deluge" || move.Name == "fairy-lock")
                            {
                                ConditionCounters[move.Name] = 1;
                            } else
                            {
                                ConditionCounters[move.Name] = 5;
                            }
                        }
                    }

                    if (move.StatusValue == MoveCategory.SideField)
                    {

                        if (move.Name.Contains("guard") || move.Name.Contains("shield") || move.Name.Contains("block"))
                        {
                            if (foeOrMe == "")
                            {
                                attacker.Protection = move.Protection;
                                if (MyLastUsedVal == 0)
                                {
                                    MyLastUsedVal = 25;
                                } else
                                {
                                    MyLastUsedVal = 100;
                                }
                            } else
                            {
                                defender.Protection = move.Protection;
                                if (FoeLastUsedVal == 0)
                                {
                                    FoeLastUsedVal = 25;
                                }
                                else
                                {
                                    FoeLastUsedVal = 100;
                                }
                            }

                        } else
                        {
                            if (foeOrMe == "")
                            {
                                MyLastUsedVal = 0;
                            } else
                            {
                                MyLastUsedVal = 0;
                            }
                        }
                        if (move.Name == "aurora-veil" && !BattleConditions["hail"])
                        {
                            ret = "The Move Failed!";
                        }

                        if (!move.ToFoe && foeOrMe == "" || move.ToFoe && foeOrMe != "")
                        {
                            if (myEntryEffects.Contains(move.uniqueActionOnePokemon))
                            {
                                ret = "The Move Failed!";
                            }
                        } else
                        {
                            if (foeEntryEffects.Contains(move.uniqueActionOnePokemon))
                            {
                                ret = "The Move Failed!";
                            }
                        }

                        if (ret != "The Move Failed!"){
                            if (move.uniqueActionOnePokemon != null)
                            {
                                if (!move.ToFoe && foeOrMe == "" || move.ToFoe && foeOrMe != "")
                                {
                                    myEntryEffects.Add(move.uniqueActionOnePokemon);
                                }
                                else
                                {
                                    foeEntryEffects.Add(move.uniqueActionOnePokemon);
                                }
                            }

                            if (MySideConditionCounters.ContainsKey(move.Name))
                            {
                                if (!move.ToFoe && foeOrMe == "" || move.ToFoe && foeOrMe != "")
                                {
                                    MySideConditionCounters[move.Name] = 5;
                                    move.uniqueActionOnePokemon(CurrentInBattle);
                                }
                                else
                                {
                                    FoeSideConditionCounters[move.Name] = 5;
                                    move.uniqueActionOnePokemon(CurrentFoe);
                                }
                            }
                        } else
                        {
                            if (move.Name != "protect" || move.Name != "detect")
                            {
                                if (foeOrMe == "")
                                {
                                    MyLastUsedVal = 0;
                                }
                                else
                                {
                                    FoeLastUsedVal = 0;
                                }
                            }
                            
                        }

                       
                        
                    }

                    if (move.StatusValue == MoveCategory.Unique)
                    {
                        if (move.Name == "protect" || move.Name == "detect")
                        {
                            if (foeOrMe == "")
                            {
                                attacker.Protection = move.Protection;
                                if (MyLastUsedVal == 0)
                                {
                                    MyLastUsedVal = 25;
                                }
                                else
                                {
                                    MyLastUsedVal = 100;
                                }
                            }
                            else
                            {
                                defender.Protection = move.Protection;
                                if (FoeLastUsedVal == 0)
                                {
                                    FoeLastUsedVal = 25;
                                }
                                else
                                {
                                    FoeLastUsedVal = 100;
                                }
                            }
                        } 
                    }


                    if (effective == effectiveness.super)
                    {
                        ret += "\n" + "It was super effective!";
                    }
                    else if (effective == effectiveness.immune)
                    {
                        ret += "\n" + "It does not effect " + defender.Name;
                    }
                    else if (effective == effectiveness.notVery)
                    {
                        ret += "\n" + "It was not very effective";
                    }
                }
                else
                {
                    ret += "\n" + "It does not effect " + defender.Name;
                }

               
            }
            else
            {
                ret = foeOrMe + attacker.Name + "'s move missed!";
            }

           
            if (foeOrMe == "")
            {
                MyLastUsedMove = move.Name;
            } else
            {
                FoeLastUsedMove = move.Name;
            }


            DealWithDamage();


            retDic.Add("return", ret);

            return retDic;
        }

        public double getWeakness(Pokemon.PokeType typeOne, Pokemon.PokeType typeTwo, Pokemon.PokeType type)
        {
            if (BattleConditions["gravity"] && (typeOne == Pokemon.PokeType.Flying || typeTwo == Pokemon.PokeType.Flying)){
                return 1;
            } 
            return weakAmount(typeOne, type) * weakAmount(typeTwo, type);
        }

        private double weakAmount(Pokemon.PokeType type, Pokemon.PokeType moveType)
        {
            double weak = 1;
            if (type == Pokemon.PokeType.Fire)
            {
                if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fairy)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Water)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Grass)
            {
                if (moveType == Pokemon.PokeType.Electric)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Water)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Flying)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Water)
            {
                if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Water)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Electric)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Normal)
            {
                if (moveType == Pokemon.PokeType.Ghost)
                {
                    weak = 0;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Fighting)
            {
                if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Dark)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fairy)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Flying)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Psychic)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Bug)
            {
                if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Flying)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Ground)
            {
                if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Water)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Electric)
                {
                    weak = 0;
                }
            }
            else if (type == Pokemon.PokeType.Poison)
            {
                if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Psychic)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Psychic)
            {
                if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Psychic)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Dark)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ghost)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Dark)
            {
                if (moveType == Pokemon.PokeType.Dark)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ghost)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Fairy)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Psychic)
                {
                    weak = 0;
                }
            }
            else if (type == Pokemon.PokeType.Ghost)
            {
                if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Dark)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ghost)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Normal)
                {
                    weak = 0;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 0;
                }
            }
            else if (type == Pokemon.PokeType.Steel)
            {
                if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Dragon)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fairy)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Flying)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Normal)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Psychic)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 0;
                }
            }
            else if (type == Pokemon.PokeType.Dragon)
            {
                if (moveType == Pokemon.PokeType.Electric)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Water)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Dragon)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Fairy)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Fairy)
            {
                if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Dark)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Dragon)
                {
                    weak = 0;
                }
            }
            else if (type == Pokemon.PokeType.Ice)
            {
                if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Rock)
            {
                if (moveType == Pokemon.PokeType.Fire)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Flying)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Normal)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Poison)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Water)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Electric)
            {
                if (moveType == Pokemon.PokeType.Electric)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Flying)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Steel)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 2;
                }
            }
            else if (type == Pokemon.PokeType.Flying)
            {
                if (moveType == Pokemon.PokeType.Bug)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Fighting)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Grass)
                {
                    weak = 0.5;
                }
                else if (moveType == Pokemon.PokeType.Electric)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ice)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Rock)
                {
                    weak = 2;
                }
                else if (moveType == Pokemon.PokeType.Ground)
                {
                    weak = 0;
                }
            }

            return weak;
        }

        public bool isImmune(MoveDictionary m, Pokemon def)
        {
            double eff = getWeakness(def.TypeOne, def.TypeTwo, m.Type);
            if (eff <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int calculateDamage(MoveDictionary m, Pokemon attacker, Pokemon defender)
        {
            int a;
            int d;
            double modifier = 1;

            if (m.Speciality == MoveStore.Speciality.Physical)
            {
                a = attacker.Attack;
                if (BattleConditions["wonder-room"])
                {
                    d = defender.SpDef;
                } else
                {
                    d = defender.Defense;
                }

                if (attacker.Status == StatusType.Burn)
                {
                    modifier = modifier * 0.5;
                }
            }
            else if (m.Speciality == MoveStore.Speciality.Special)
            {
                a = attacker.SpAtk;

                if (BattleConditions["wonder-room"])
                {
                    d = defender.Defense;
                }
                else
                {
                    d = defender.SpDef;
                }
            }
            else
            {
                modifier = 0;
                a = 10;
                d = 10;
            }

            double eff = 0;

            if (BattleConditions["ion-deluge"] && m.Type == Pokemon.PokeType.Normal)
            {
                eff = getWeakness(defender.TypeOne, defender.TypeTwo, Pokemon.PokeType.Electric);
            } else
            {
                eff = getWeakness(defender.TypeOne, defender.TypeTwo, m.Type);
            }

            if (eff == 2)
            {
                effective = effectiveness.super;
            }
            else if (eff == 0.5 || eff == 0.25)
            {
                effective = effectiveness.notVery;
            }
            else if (eff == 1)
            {
                effective = effectiveness.normal;
            }
            else
            {
                effective = effectiveness.immune;
            }

            if((m.Type == Pokemon.PokeType.Water && BattleConditions["rain-dance"]) || (m.Type == Pokemon.PokeType.Fire && BattleConditions["sunny-day"]))
            {
                modifier = modifier * 1.5;
            }

            if ((m.Type == Pokemon.PokeType.Water && BattleConditions["sunny-day"]) || (m.Type == Pokemon.PokeType.Fire && BattleConditions["sunn-day"]))
            {
                modifier = modifier * 1.5;
            }

            if (BattleConditions["mud-sport"] && m.Type == Pokemon.PokeType.Electric)
            {
                modifier = modifier * 0.5;
            }

            if (BattleConditions["water-sport"] && m.Type == Pokemon.PokeType.Fire)
            {
                modifier = modifier * 0.5;
            }

            if (BattleConditions["grassy-terrain"] && m.Type == Pokemon.PokeType.Grass)
            {
                modifier = modifier * 1.5;
            }

            if (BattleConditions["misty-terrain"] && m.Type == Pokemon.PokeType.Dragon)
            {
                modifier = modifier * 0.5;
            }

            if (BattleConditions["electric-terrain"] && m.Type == Pokemon.PokeType.Electric)
            {
                modifier = modifier * 1.5;
            }

            if (BattleConditions["psychic-terrain"] && m.Type == Pokemon.PokeType.Psychic)
            {
                modifier = modifier * 1.5; ;
            }


            Random r = new Random();
            modifier = modifier *( r.NextDouble() * (1 - 0.85) + 0.85);

            int damage = (int)Math.Ceiling(((((((2 * attacker.level) / 5) + 2) * m.damage * (a / d)) / 50) + 2) * modifier * eff);
            return damage;
        }


        public string[] getButtonContent()
        {
            int[] p = new int[] { 10, 10, 10, 10 };
            return new string[] { CurrentInBattle.Moves[0] + "\n" + CurrentInBattle.PP[0] + "/" + p[0], CurrentInBattle.Moves[1] + "\n" + CurrentInBattle.PP[1] + "/" + p[1], CurrentInBattle.Moves[2] + "\n" + CurrentInBattle.PP[2] + "/" + p[2], CurrentInBattle.Moves[3] + "\n" + CurrentInBattle.PP[3] + "/" + p[3] };
        }
        public double getHPBarWidth(Pokemon p)
        {
            return ((double)p.CurrentHP / (double)p.HP);
        }

        public double getExpBarWidth()
        {
            double o = Math.Pow(CurrentInBattle.level, 3);

           return (Math.Max((double)CurrentInBattle.Experience - o, 0) / (Math.Pow(CurrentInBattle.level + 1, 3) - o)) * 160;
        }

        public SolidColorBrush getHealthColor(Pokemon p)
        {
            if (p.CurrentHP < p.HP * 0.10)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (p.CurrentHP < p.HP * 0.5)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else
            {
               return new SolidColorBrush(Colors.Green);
            }
        }
        private string StatusTypeToString(StatusType s)
        {
            if (s == StatusType.Null)
            {
                return "";
            }
            else
            {
                return s.ToString();
            }
        }

        public void removeFromFoe(Pokemon p)
        {
            foe.Remove(p);
        }

        public bool battleEnded()
        {
            return foe.Count == 0;
        }

        public List<String> calculateExperience()
        {
            int i = 0;
            bool levelUp = false;
            List<string> comments = new List<string>();
            string s;

            foreach (Pokemon p in pokesThatBattled)
            {
                levelUp = false;
                int dExp = changeInExp(true, foe.First().level, pokesThatBattled.Count());
                p.Experience += dExp;
                ++i;
                if (Math.Pow(p.level + 1, 3) < p.Experience)
                {
                    ++p.level;
                    levelUp = true;
                }
                s = p.Name + " has earned " + dExp + " exp ";
                if (levelUp)
                {
                    s += " and grew to level " + p.level;
                    if (p.newMove() != null)
                    {
                        comments.Add("NEW-MOVE:" + p.ID + ","+ p.newMove());
                    }
                }
                comments.Add(s);
            }


            return comments;
        }

        private int changeInExp(bool wild, int levelFaint, int numberOfParticipants)
        {
            if (wild)
            {
                return (int)(130 * levelFaint) / (7 * numberOfParticipants);
            }
            else
            {
                return (int)(1.5 * 130 * levelFaint) / (7 * numberOfParticipants);
            }
        }

       

        private bool moreAlive()
        {
            foreach (Pokemon p in party)
            {
                if (p != null)
                {
                    if (p.CurrentHP > 0)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        private string buffetByHail()
        {
            string ret = "The Hail continues! \n";
            if (CurrentInBattle.TypeOne != Pokemon.PokeType.Ice || CurrentInBattle.TypeTwo == Pokemon.PokeType.Ice)
            {
                //also some items and abilities - not done yet
                CurrentInBattle.CurrentHP -= (int)Math.Ceiling(0.0625 * (double)CurrentInBattle.HP);
                ret += CurrentInBattle.Name + " was buffeted by hail! \n";
            }

            if (CurrentFoe.TypeOne != Pokemon.PokeType.Ice || CurrentFoe.TypeTwo == Pokemon.PokeType.Ice)
            {
                //also some items and abilities - not done yet
                CurrentFoe.CurrentHP -= (int)Math.Ceiling(0.0625 * (double)CurrentFoe.HP);
                ret += CurrentFoe.Name + " was buffeted by hail! \n";
            }

            return ret;
        }

        private string buffetBySandstorm()
        {
            string ret = "A Sandstorm Rages! \n";
            if (CurrentInBattle.TypeOne != Pokemon.PokeType.Rock || CurrentInBattle.TypeTwo == Pokemon.PokeType.Rock || CurrentInBattle.TypeOne != Pokemon.PokeType.Steel || CurrentInBattle.TypeTwo == Pokemon.PokeType.Steel || CurrentInBattle.TypeOne != Pokemon.PokeType.Ground || CurrentInBattle.TypeTwo == Pokemon.PokeType.Ground)
            {
                //also some items and abilities - not done yet
                CurrentInBattle.CurrentHP -= (int)Math.Ceiling(0.0625 * (double)CurrentInBattle.HP);
                ret += CurrentInBattle.Name + " was buffeted by Sandstorm! \n";
            }

            if (CurrentFoe.TypeOne != Pokemon.PokeType.Rock || CurrentFoe.TypeTwo == Pokemon.PokeType.Rock || CurrentFoe.TypeOne != Pokemon.PokeType.Steel || CurrentFoe.TypeTwo == Pokemon.PokeType.Steel || CurrentFoe.TypeOne != Pokemon.PokeType.Ground || CurrentFoe.TypeTwo == Pokemon.PokeType.Ground)
            {
                //also some items and abilities - not done yet
                CurrentFoe.CurrentHP -= (int)Math.Ceiling(0.0625 * (double)CurrentFoe.HP);
                ret += CurrentFoe.Name + " was buffeted by Sandstorm! \n";
            }

            return ret;
        }

        
        internal string EndOfAttackPhaseEffects()
        {
            string ret = "";

            foreach (KeyValuePair<string, bool> kvp in BattleConditions.Where( k => k.Value == true))
            {
                --ConditionCounters[kvp.Key];
                
                if (kvp.Key == "hail")
                {
                    if (ConditionCounters["hail"] == 0)
                    {
                        ret = "The Hail Stopped! \n";
                    } else
                    {
                        ret = buffetByHail();
                    }
                    
                } else if (kvp.Key == "rain-dance")
                {
                    if (ConditionCounters["rain-dance"] == 0)
                    {
                        ret = "The Rain Stopped! \n";
                    }
                    else
                    {
                        ret = "It is Raining heavily! \n";
                    }
                }
                else if (kvp.Key == "sunny-day")
                {
                    if (ConditionCounters["sunny-day"] == 0)
                    {
                        ret = "The Sunshine Subsided! \n";
                    }
                    else
                    {
                        ret = "The Sunshine is bright! \n";
                    }
                }
                else if (kvp.Key == "sandstorm")
                {
                    if (ConditionCounters["sandstorm"] == 0)
                    {
                        ret = "The Sandstorm Subsided! \n";
                    }
                    else
                    {
                        ret = buffetBySandstorm();
                    }
                } else if (kvp.Key == "grassy-terrain")
                {
                    if (CurrentFoe.isGrounded())
                    {
                        ret += CurrentFoe.Name + " regained health from the terrain! \n";
                        CurrentFoe.CurrentHP += (int)Math.Ceiling(0.0625 * (double)CurrentFoe.HP);
                        CurrentFoe.CurrentHP = Math.Min(CurrentFoe.CurrentHP, CurrentFoe.HP);
                    }
                    if (CurrentInBattle.isGrounded())
                    {
                        ret += CurrentInBattle.Name + " regained health from the terrain! \n";
                        CurrentInBattle.CurrentHP += (int)Math.Ceiling(0.0625 * (double)CurrentInBattle.HP);
                        CurrentInBattle.CurrentHP = Math.Min(CurrentInBattle.CurrentHP, CurrentInBattle.HP);
                    }
                }


            }


            foreach(KeyValuePair<string, int> kvp in ConditionCounters.Where(k => k.Value == 0))
            {
                BattleConditions[kvp.Key] = false;
            }

            var b = MySideConditionCounters.Where(k => k.Value > 0).ToList();
            foreach (KeyValuePair<string, int> kvp in b)
            {
                --MySideConditionCounters[kvp.Key];
                if (MySideConditionCounters[kvp.Key] == 0)
                {
                    resetBattleConditions(kvp.Key, CurrentInBattle);
                }
            }

            b = FoeSideConditionCounters.Where(k => k.Value > 0).ToList();
            foreach (KeyValuePair<string, int> kvp in b)
            {
                --FoeSideConditionCounters[kvp.Key];
                if (FoeSideConditionCounters[kvp.Key] == 0)
                {
                    resetBattleConditions(kvp.Key, CurrentFoe);
                }
            }

            return ret;
        }

        private void resetBattleConditions(string dict, Pokemon p)
        {
            if (dict == "mist")
            {
                p.canChangeStat = true;
            } else if (dict == "light-screen")
            {
                p.changeStat(Pokemon.Stat.SpDef,-1, true);
            }
            else if (dict == "reflect")
            {
                p.changeStat(Pokemon.Stat.Defense,-1, true);
            }
            else if (dict == "safegaurd")
            {
                p.canSetStatus = true;
            }
            else if (dict == "tailwind")
            {
                p.changeStat(Pokemon.Stat.Speed, -1, true);
            }
            else if (dict == "lucky-chant")
            {

            }
            else if (dict == "aurora-veil")
            {
                p.changeStat(Pokemon.Stat.SpDef, -1, true);
                p.changeStat(Pokemon.Stat.Defense, -1, true);
            }
        }
        internal string InvokeMyEntryEffects()
        {
            string ret = CurrentInBattle.Name + " is infliced by ";

            foreach(var t in myEntryEffects)
            {
                ret += t(CurrentInBattle);
            }

            return ret;
        }

        internal string  InvokeFoeEntryEffects()
        {
            string ret = CurrentFoe.Name + " is infliced by ";
            foreach (var t in foeEntryEffects)
            {
               ret += t(CurrentFoe);
            }
            return ret;
        }
    }
}
