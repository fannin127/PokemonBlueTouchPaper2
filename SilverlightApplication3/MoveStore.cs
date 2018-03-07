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
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.IO;

namespace SilverlightApplication3
{
    public sealed class MoveStore
    {
        private HashSet<MoveDictionary> moves;
        private MoveDictionary struggleMove;

        private static MoveStore instance;

        public static MoveStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MoveStore();
                }
                return instance;
            }
        }
        private MoveStore()
        {
            moves = new HashSet<MoveDictionary>();
            createMoves();
        }

        private void createMoves()
        {
            /*
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.DefenseCurl, 1, 40, Pokemon.PokeType.Normal,Speciality.Null , Status.StatChange, -5, Pokemon.Stat.Defense, false));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Ember, 40, 25, Pokemon.PokeType.Fire, Speciality.Special, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Growl, -1, 40, Pokemon.PokeType.Normal, Speciality.Null, Status.StatChange, 100, Pokemon.Stat.Attack, true));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Leer, -1, 40, Pokemon.PokeType.Normal, Speciality.Null, Status.StatChange, 100, Pokemon.Stat.Defense, true));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.PoisonPowder, 35, Pokemon.PokeType.Poison, Speciality.Null, Status.Status, 75, StatusType.Poison, true));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Scratch, 40, 35, Pokemon.PokeType.Normal, Speciality.Physical, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.SmokeScreen, -1, 20, Pokemon.PokeType.Normal, Speciality.Null, Status.StatChange, 100, Pokemon.Stat.Accuracy, true));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Tackle, 40, 35, Pokemon.PokeType.Normal, Speciality.Physical, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.VineWhip, 45, 25, Pokemon.PokeType.Grass, Speciality.Physical, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.WaterGun, 40, 25, Pokemon.PokeType.Water, Speciality.Special, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.SandAttack, -1, 15, Pokemon.PokeType.Ground, Speciality.Null, Status.StatChange, 100, Pokemon.Stat.Accuracy, true));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.QuickAttack, 40, 30, Pokemon.PokeType.Normal, Speciality.Physical, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Gust, 40, 35, Pokemon.PokeType.Flying, Speciality.Physical, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Confusion, 50, 25, Pokemon.PokeType.Psychic, Speciality.Special, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Disable, 0, 20, Pokemon.PokeType.Normal, Speciality.Null, Status.Status, 100)); //disble is hard
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Swift, 60, 20, Pokemon.PokeType.Normal, Speciality.Special, Status.Attacking, -5));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Psychic, 90, 10, Pokemon.PokeType.Psychic, Speciality.Special, Status.Attacking, 100));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Barrier, 2, 20, Pokemon.PokeType.Psychic, Speciality.Null, Status.StatChange, -5, Pokemon.Stat.Defense, false));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Recover, 5, 10, Pokemon.PokeType.Normal, Speciality.Null, Status.StatChange, -5, Pokemon.Stat.HP, false));
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Mist, 0, 30, Pokemon.PokeType.Ice, Speciality.Null, Status.Status, -5)); //mist is hard too
            moves.Add(new MoveDictionary(MoveDictionary.moveDictionary.Amnesia, 2, 20, Pokemon.PokeType.Psychic, Speciality.Null, Status.StatChange, -5, Pokemon.Stat.SpDef, false));
            */

            struggleMove = new MoveDictionary("Struggle", 50, int.MaxValue, Pokemon.PokeType.Normal, Speciality.Physical, MoveCategory.Attacking, 100, 0,"foe");
            moves.Add(struggleMove);

            moves.Add(new MoveDictionary("ConfusedMoveAttackYourself", 40, 99999, Pokemon.PokeType.Null, Speciality.Physical, MoveCategory.Attacking, 1000, 0, "user"));


            AddUniqueMoves();
               
        }

        private void AddUniqueMoves()
        {
            moves.Add(new MoveDictionary("protect", 10, Pokemon.PokeType.Normal, Speciality.Null, MoveCategory.Unique, 100, getUniqueProtectionAction("protect"), 4));
            moves.Add(new MoveDictionary("detect", 5, Pokemon.PokeType.Fighting, Speciality.Null, MoveCategory.Unique, 100, getUniqueProtectionAction("detect"), 4));
            moves.Add(new MoveDictionary("metronome", 0, 10, Pokemon.PokeType.Normal, Speciality.Null, MoveCategory.Unique, 100, 0, "foe"));
            moves.Add(new MoveDictionary("splash", 0, 40, Pokemon.PokeType.Normal, Speciality.Null, MoveCategory.Unique, 100, 0, "foe"));
            moves.Add(new MoveDictionary("mirror-move", 0, 20, Pokemon.PokeType.Flying, Speciality.Null, MoveCategory.Unique, 100, 0, "foe"));
        }

        public MoveDictionary get(string m)
        {
            if (m == null)
            {
                return struggleMove;
            }
            var q = moves.Where(c => c.Name == m);
            if (!q.Any())
            {
                moves.Add(readInMoveFromXML(m));
            }
            return q.ToArray()[0];
        }

        private Func<Pokemon, string> getUniqueActionOnePokemon(string name)
        {
            if (name == "spikes")
            {
                return (p) => { p.CurrentHP -= (int)Math.Ceiling(0.125 * (double)p.HP); return "Spikes!\n"; };
            }
            else if (name == "toxic-spikes")
            {
                return (p) => { p.Status = StatusType.Poison; return "Toxic spikes! \n"; };
            }
            else if (name == "stealth-rock")
            {
                return (p) => { p.CurrentHP -= (int)Math.Ceiling(0.125 * (double)p.HP); return "Sealth Rock!\n"; };
            }
            else if (name == "sticky-web")
            {
                return (p) => { p.changeStat(Pokemon.Stat.Speed, -1, true); return "Sticky Web\n"; };
            }
            else if (name == "mist")
            {
                return (p) => { p.canChangeStat = false; return "Mist\n"; };
            }
            else if (name == "reflect")
            {
                return (p) => { p.changeStat(Pokemon.Stat.Defense, 1, true); return "Reflect"; };
            }
            else if (name == "light-screen")
            {
                return (p) => { p.changeStat(Pokemon.Stat.SpDef, 1, true); return "Light Screen"; };
            }
            else if (name == "safeguard")
            {
                return (p) => { p.canSetStatus = false; return "Safegaurd\n"; };
            }
            else if (name == "tailwind")
            {
                return (p) => { p.changeStat(Pokemon.Stat.Speed, 1, true); return "tailwind"; };
            }
            else if (name == "lucky-chant")
            {
                //TODO
            }
            else if (name == "aurora-veil")
            {
                return (p) => { p.changeStat(Pokemon.Stat.SpDef, 1, true); p.changeStat(Pokemon.Stat.Defense, 1, true); return "aurora-veil"; };
            }

            return null;
        }

        private Func<MoveDictionary, bool> getUniqueProtectionAction(string name)
        {
            switch (name)
            {
                case "protect":
                    return (md) => { return true; };
                case "detect":
                    return (md) => { return true; };
                case "wide-guard":
                    return (md) => { return md.AttackAll;  };
                case "quick-guard":
                    return (md) => { return (md.Priority > 0); };
                case "mat-block":
                    return (md) => { return md.StatusValue != MoveCategory.Status; };
                case "crafty-shield":
                    return (md) => { return md.StatusValue == MoveCategory.Status; };

                default:
                    return (md) => { return true; }; 
            }
        }

        public MoveDictionary get(int i)
        {
            var doc = new XDocument();



            using (Stream isoStream = this.GetType().Assembly.GetManifestResourceStream("SilverlightApplication3.moves.xml"))
            {
                using (StreamReader sw = new StreamReader(isoStream))
                {
                    doc = XDocument.Load(sw);
                }
            }



            var query = (from mo in doc.Elements("Moves").Elements("Move") where int.Parse(mo.Element("ID").Value) == i select mo.Element("Name").Value).ToArray()[0];

            return (get(query.ToString()));

        }

        private MoveDictionary readInMoveFromXML(string m)
        {
            if (m != null)
            {
                var doc = new XDocument();

                using (Stream isoStream = this.GetType().Assembly.GetManifestResourceStream("SilverlightApplication3.moves.xml"))
                {
                    using (StreamReader sw = new StreamReader(isoStream))
                    {
                        doc = XDocument.Load(sw);
                    }
                }

                MoveDictionary md = null;
                var query = (from mo in doc.Elements("Moves").Elements("Move") where mo.Element("Name").Value == m select mo).ToArray()[0];

                
                if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.Attacking)
                {
                    md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("Damage").Value), int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), Parser.parseMoveCategory(query.Element("StatusValue").Value), Parser.tryParseInteger(query.Element("Accuracy").Value, 100), int.Parse(query.Element("Priority").Value), query.Element("ToFoe").Value);
                }
                else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.StatChange)
                {
                    int val = 0;
                    List<Pokemon.Stat> stats = new List<Pokemon.Stat>();
                    foreach (var v in query.Elements("StatChanges"))
                    {
                        val = int.Parse(v.Element("StatAmount").Value);
                        stats.Add(Parser.parsePokemonStat(v.Element("StatToChange").Value));
                    }
                    md = new MoveDictionary(query.Element("Name").Value, val, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Speciality.Null, MoveCategory.StatChange, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), stats, query.Element("ToFoe").Value, int.Parse(query.Element("Priority").Value));
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.DamageAndLower || Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.DamageAndRaise)
                {
                    int val = 0;
                    List<Pokemon.Stat> stats = new List<Pokemon.Stat>();
                    val = int.Parse(query.Element("StatChanges").Element("StatAmount").Value);
                    foreach (var v in query.Element("StatChanges").Elements("StatToChange"))
                    {
                        stats.Add(Parser.parsePokemonStat(v.Value));
                    }

                    if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.DamageAndLower)
                    {
                        md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("Damage").Value), val, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.DamageAndLower, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), stats, "foe", "foe", int.Parse(query.Element("StatChance").Value), int.Parse(query.Element("Priority").Value));
                    }
                    else
                    {
                        md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("Damage").Value), val, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.DamageAndRaise, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), stats, "foe", "user", int.Parse(query.Element("StatChance").Value), int.Parse(query.Element("Priority").Value));
                    }

                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.Status)
                {
                    md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Speciality.Null, MoveCategory.Status, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), Parser.parseStatus(query.Element("Status").Value), query.Element("ToFoe").Value, int.Parse(query.Element("Priority").Value)); 
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.DamageAndAilement)
                {
                    md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("Damage").Value), int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.DamageAndAilement, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), Parser.parseStatus(query.Element("Status").Value), query.Element("ToFoe").Value, int.Parse(query.Element("StatusChance").Value), int.Parse(query.Element("Priority").Value));
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.Healing)
                {
                    md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("Healing").Value), int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.Healing, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), int.Parse(query.Element("Priority").Value), query.Element("ToFoe").Value);
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.StatusAndRaiseStats)
                {
                    int val = 0;
                    List<Pokemon.Stat> stats = new List<Pokemon.Stat>();
                    val = Parser.tryParseInteger(query.Element("StatChanges").Element("StatAmount").Value, 1);
                    foreach (var v in query.Element("StatChanges").Elements("StatToChange"))
                    {
                        stats.Add(Parser.parsePokemonStat(v.Value));
                    }
                    if (stats.Count == 0){
                        stats.Add(Pokemon.Stat.Speed);
                    }
                    md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.StatusAndRaiseStats, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), Parser.parseStatus(query.Element("Status").Value), "foe", stats, val, "foe", int.Parse(query.Element("Priority").Value));
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.Absorbing)
                {
                    md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("Damage").Value), int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), Parser.parseMoveCategory(query.Element("StatusValue").Value), Parser.tryParseInteger(query.Element("Accuracy").Value, 100), query.Element("ToFoe").Value, int.Parse(query.Element("Drain").Value), int.Parse(query.Element("Priority").Value));
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.KO)
                {
                    md = new MoveDictionary(query.Element("Name").Value, 0, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.KO, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), int.Parse(query.Element("Priority").Value), query.Element("ToFoe").Value);
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.Field)
                {
                    if (query.Name == "Haze") //bit more unique than others in the same group
                    {
                        md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.Field, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), PerformUniqueMove(query.Element("Name").Value), int.Parse(query.Element("Priority").Value));
                    } else
                    {
                        md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.Field, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), (bool b) => { b = true; }, int.Parse(query.Element("Priority").Value));
                    }
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.SideField)
                {
                    if (query.Element("Name").Value == "spikes" || query.Element("Name").Value == "toxic-spikes" || query.Element("Name").Value == "stealth-rock" || query.Element("Name").Value == "sticky-web" || query.Element("Name").Value == "light-screen" || query.Element("Name").Value == "reflect" || query.Element("Name").Value == "mist" || query.Element("Name").Value == "safeguard" || query.Element("Name").Value == "tailwind" ||  query.Element("Name").Value == "lucky-chant" || query.Element("Name").Value == "aurora-veil") 
                    {
                        md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.SideField, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), query.Element("ToFoe").Value, getUniqueActionOnePokemon(query.Element("Name").Value), int.Parse(query.Element("Priority").Value));
                    } else
                    {
                        md = new MoveDictionary(query.Element("Name").Value, int.Parse(query.Element("PP").Value), Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.SideField, Parser.tryParseInteger(query.Element("Accuracy").Value, 100), getUniqueProtectionAction(query.Element("Name").Value), int.Parse(query.Element("Priority").Value));
                    }
                } else if (Parser.parseMoveCategory(query.Element("StatusValue").Value) == MoveCategory.Switch)
                {
                    md = new MoveDictionary(query.Element("Name").Value, Parser.parsePokeType(query.Element("Type").Value), Parser.parseSpeciality(query.Element("Speciality").Value), MoveCategory.Switch, int.Parse(query.Element("PP").Value), Parser.tryParseInteger(query.Element("Accuracy").Value, 100), int.Parse(query.Element("Priority").Value));
                }

                return md;
            } else
            {
                return struggleMove;
            }
            
        }

        public Action<Pokemon, Pokemon> PerformUniqueMove(String m)
        {
            if (m == "haze")
            {
                return (Pokemon a, Pokemon d) =>
                {
                    a.resetStats();
                    d.resetStats();
                };
                
            }

            return null;
        }





        public enum Speciality { Special, Physical, Null};

       
        }
        public enum MoveCategory { Attacking, Status, StatChange, Healing, DamageAndAilement, StatusAndRaiseStats, DamageAndLower, DamageAndRaise, Absorbing, KO, Field, SideField, Switch, Unique}
    }

