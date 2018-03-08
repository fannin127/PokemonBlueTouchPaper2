using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;


namespace SilverlightApplication3
{
    public class Pokebuilder
    {

        private static Pokebuilder instance;

        public static Pokebuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Pokebuilder();
                }
                return instance;
            }
        }

        private Pokebuilder()
        {
            //nothing to be done but should be private 
        }

        //for testing only
        public List<Pokemon> SpecificPokesForTrainer()
        {
            List<Pokemon> pList = new List<Pokemon>();
            pList = pokesForTrainer(new List<int> { 7 }, new List<int> { 5 });

            pList.First().Moves = new string[] { "tackle", "tackle", "tackle", "tackle"};
            pList.First().PP = new int[] {1,1,1,10 };
            return pList;
        }

        public List<Pokemon> pokesForTrainer(List<int> pokes, List<int> level)
        {
            List<Pokemon> pList = new List<Pokemon>();
            int i = 0;
            foreach(int p in pokes)
            {
                pList.Add(pokesForRoute(new List<int> { p }, level[i], level[i]).First());
                i++;
            }

            return pList;
        }

        public int[] getPPArray(string [] m)
        {
            try
            {
                int[] pp = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    pp[i] = MoveStore.Instance.get(m[i]).PP;
                }
                return pp;
            } catch (Exception)
            {
                return new int[] { 10, 10, 10, 10 };
            }
            
        }

        public Dictionary<int, string> MovesOnLevelForPokemon(int i)
        {
            Dictionary<int, string> ret = new Dictionary<int, string>();

            var doc = new XDocument();
            using (Stream isoStream = this.GetType().Assembly.GetManifestResourceStream("SilverlightApplication3.pokedex.xml"))
            {
                using (StreamReader sw = new StreamReader(isoStream))
                {
                    doc = XDocument.Load(sw);
                }
            }

            var query = from elem in doc.Elements("Pokes").Elements("Poke") where (Int32.Parse(elem.Element("Number").Value) == i) select elem.Element("Moves");

            var t = query.Elements("Move");

            foreach(var m in t)
            {
                try
                {
                    ret.Add(int.Parse(m.Element("Level").Value), m.Element("Name").Value.ToString());
                }
                catch
                {
                    //pokemon that learn multiple moves at the same level
                }
               
            }

            return ret;
        }

        public List<Pokemon> pokesForRoute(List<int> p, int max, int min)
        {
            var doc = new XDocument();

           
                using (Stream isoStream = this.GetType().Assembly.GetManifestResourceStream("SilverlightApplication3.pokedex.xml"))
                {
                    using (StreamReader sw = new StreamReader(isoStream))
                    {
                        doc = XDocument.Load(sw);
                    }
                }
            
    

            var query = from elem in doc.Elements("Pokes").Elements("Poke") where p.Contains(Int32.Parse(elem.Element("Number").Value)) select elem;

            List<Pokemon> ret = new List<Pokemon>();

            Random r = new Random();

            int lvl;
            

            foreach (var q in query)
            {
                lvl = r.Next(min, max);
                string[] moves = new string[4];
                var movQ = from mo in q.Elements("Moves").Elements("Move") where Int32.Parse(mo.Element("Level").Value) <= lvl select mo;
                int i = 0;
                foreach (var m in movQ.Take(4))
                {

                    moves[i] = m.Element("Name").Value;
                    i++;
                }
                Pokemon pok = new Pokemon(Int32.Parse(q.Element("Number").Value), q.Element("Name").Value, lvl, Parser.parsePokeType(q.Element("TypeOne").Value), Parser.parsePokeType(q.Element("TypeTwo").Value), moves, Int32.Parse(q.Element("HP").Value), -9, Int32.Parse(q.Element("Attack").Value), Int32.Parse(q.Element("Defense").Value), Int32.Parse(q.Element("Sp.Atk").Value), Int32.Parse(q.Element("Sp.Def").Value), Int32.Parse(q.Element("Speed").Value), Int32.Parse(q.Element("CatchRate").Value));

                var evolQ = from ev in q.Elements("Evolutions").Elements("Evolution") select ev;

                List<EvolutionTrigger> evoList = new List<EvolutionTrigger>();

                foreach (var t in evolQ)
                {
                    int into = int.Parse(t.Element("Evolves").Value);
                    int eLvl = -99;
                    try
                    {
                        eLvl = Int32.Parse(t.Element("EvolveLevel").Value);
                        evoList.Add(new EvolutionTrigger(eLvl, into));
                    } catch (Exception)
                    {
                        if (t.Element("EvolveLevel").Value == "Trade")
                        {
                            evoList.Add(new EvolutionTrigger(into));
                        } else
                        {
                            evoList.Add(new EvolutionTrigger(Parser.parseItemName(t.Element("EvolveLevel").Value), into));
                        }
                    }
                }

                var abilityQ = from ab in query.Elements("Abilities").Elements("Ability") select ab.Value;

                int ran = r.Next(0, abilityQ.Count());

                pok.ability = new Ability(abilityQ.ToArray()[ran]);

                pok.evolvesInto = evoList;
                pok.PP = getPPArray(moves);
                ret.Add(pok);
            } 
            return ret;
        }

        public Pokemon evolvePokemon(Pokemon p)
        {
            Pokemon t = pokesForTrainer(new List<int> { p.toEvolve }, new List<int> { p.level }).First();
            t.Moves = p.Moves;

            t.level = p.level;
            t.Experience = p.Experience;
            t.CurrentHP = (int)(((double)p.CurrentHP / (double)p.HP) * ((double)t.HP));
            t.Status = p.Status;
            
            return t;
        }

        public List<Pokemon> openPokes(XElement elem)
        {
            List<Pokemon> pokes = new List<Pokemon>();
            foreach(var p in elem.Elements("poke"))
            {
                string[] movD = new string[4];
                var moves = p.Element("Moves");
                int[] pp = new int[4];
                int i = 0;
                foreach (var m in moves.Elements("Move"))
                {
                    movD[i] = m.Value;
                    ++i;
                }
                i = 0;
                foreach( var ppp in moves.Elements("PP"))
                {
                    pp[i] = int.Parse(ppp.Value);
                    ++i;
                }
                pokes.Add(new Pokemon(Int32.Parse(p.Element("Number").Value), p.Element("Name").Value, Int32.Parse(p.Element("Level").Value),  Parser.parsePokeType(p.Element("TypeOne").Value), Parser.parsePokeType(p.Element("TypeTwo").Value), movD, Int32.Parse(p.Element("HP").Value), Int32.Parse(p.Element("currentHP").Value), Int32.Parse(p.Element("Atk").Value), Int32.Parse(p.Element("Def").Value), Int32.Parse(p.Element("SpAtk").Value), Int32.Parse(p.Element("SpDef").Value), Int32.Parse(p.Element("Speed").Value), 0));
                pokes.Last().Experience = int.Parse(p.Element("Experience").Value);
                pokes.Last().PP = pp;

                pokes.Last().evolvesInto = getEvolutionTriggers(pokes.Last().Number);
                pokes.Last().MovesOnLevel = MovesOnLevelForPokemon(int.Parse(p.Element("Number").Value));
                pokes.Last().ability = new Ability(p.Element("Ability").Value);
            }

            return pokes;
        }

        private List<EvolutionTrigger> getEvolutionTriggers(int number)
        {
            return pokesForTrainer(new List<int> { number }, new List<int> { 0 }).First().evolvesInto;
        }

        public XElement savePokes(Pokemon[] pokes, string name)
        {
            XElement ret = new XElement(name);

            foreach(var p in pokes)
            {
                if (p != null)
                {
                    Dictionary<string, int> d = p.getBaseStats();

                    XElement poke = new XElement("poke");
                    poke.Add(new XElement("Number", p.Number));
                    poke.Add(new XElement("Ability", p.ability.Name));
                    poke.Add(new XElement("Experience", p.Experience));
                    poke.Add(new XElement("Level", p.level));
                    poke.Add(new XElement("Speed", d["Speed"]));
                    poke.Add(new XElement("Atk", d["Atk"]));
                    poke.Add(new XElement("Def", d["Def"]));
                    poke.Add(new XElement("SpAtk", d["SpAtk"]));
                    poke.Add(new XElement("SpDef", d["SpDef"]));
                    poke.Add(new XElement("HP", d["HP"]));
                    poke.Add(new XElement("currentHP", p.CurrentHP));
                    poke.Add(new XElement("Status", p.Status));
                    poke.Add(new XElement("Name", p.Name));
                    poke.Add(new XElement("TypeOne", p.TypeOne));
                    poke.Add(new XElement("TypeTwo", p.TypeTwo));



                    XElement moves = new XElement("Moves");
                    int i = 0;
                    foreach (string m in p.Moves)
                    {
                        moves.Add(new XElement("Move", m));
                        moves.Add(new XElement("PP", p.PP[i]));
                        ++i;
                    }

                    poke.Add(moves);

                    ret.Add(poke);
                } 
                
            }

            return ret;
        }

        internal List<Pokemon> getAllPokesIDName()
        {
            List<int> allNumbers = new List<int>();
            for (int i = 1; i < 152; i++)
            {
                allNumbers.Add(i);
            }

            return pokesForRoute(allNumbers, 0, 0);
        }

        internal string getFlavourText(int number)
        {
            var doc = new XDocument();


            using (Stream isoStream = this.GetType().Assembly.GetManifestResourceStream("SilverlightApplication3.pokedexEntries.xml"))
            {
                using (StreamReader sw = new StreamReader(isoStream))
                {
                    doc = XDocument.Load(sw);
                }
            }

            var query = from elem in doc.Elements("Pokes").Elements("Poke") where int.Parse(elem.Element("Number").Value) == number select elem.Element("FlavourText").Value;

            return query.ToList().First();
        }


        /*
        public MoveDictionary.moveDictionary parseMoveName(string s)
        {
            switch (s.ToLower())
            {
                case "tackle":
                    return MoveDictionary.moveDictionary.Tackle;
                case "leer":
                    return MoveDictionary.moveDictionary.Leer;
                case "scratch":
                    return MoveDictionary.moveDictionary.Scratch;
                case "growl":
                    return MoveDictionary.moveDictionary.Growl;
                case "vinewhip":
                    return MoveDictionary.moveDictionary.VineWhip;
                case "watergun":
                    return MoveDictionary.moveDictionary.WaterGun;
                case "ember":
                    return MoveDictionary.moveDictionary.Ember;
                case "poisonpowder":
                    return MoveDictionary.moveDictionary.PoisonPowder;
                case "smokescreen":
                    return MoveDictionary.moveDictionary.SmokeScreen;
                case "defensecurl":
                    return MoveDictionary.moveDictionary.DefenseCurl;
                case "sandattack":
                    return MoveDictionary.moveDictionary.SandAttack;
                case "quickattack":
                    return MoveDictionary.moveDictionary.QuickAttack;
                case "gust":
                    return MoveDictionary.moveDictionary.Gust;
                case "confusion":
                    return MoveDictionary.moveDictionary.Confusion;
                case "disable":
                    return MoveDictionary.moveDictionary.Disable;
                case "swift":
                    return MoveDictionary.moveDictionary.Swift;
                case "psychic":
                    return MoveDictionary.moveDictionary.Psychic;
                case "barrier":
                    return MoveDictionary.moveDictionary.Barrier;
                case "recover":
                    return MoveDictionary.moveDictionary.Recover;
                case "mist":
                    return MoveDictionary.moveDictionary.Mist;
                case "amnesia":
                    return MoveDictionary.moveDictionary.Amnesia;
            }

            return MoveDictionary.moveDictionary.ExampleMoveNotReal;
        }

    */

    }
}
