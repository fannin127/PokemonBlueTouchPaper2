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
        MoveStore ms;
        Parser parser;
        public Pokebuilder(MoveStore ms)
        {
            parser = new Parser();
            this.ms = ms;
        }



 
        public void writePokemon()
        {

/*
            var doc = new XDocument();
            var Pokes = new XElement("Pokes");

            var bulb = new XElement("Poke");

            bulb.Add(new XElement("Number", 1));
            bulb.Add(new XElement("Name", "Bulbasaur"));
            bulb.Add(new XElement("TypeOne", Pokemon.PokeType.Grass));
            bulb.Add(new XElement("TypeTwo", Pokemon.PokeType.Poison));
            bulb.Add(new XElement("Evolves", 2));
            bulb.Add(new XElement("EvolveLevel", 16));
            bulb.Add(new XElement("HP", 45));
            bulb.Add(new XElement("Attack", 49));
            bulb.Add(new XElement("Defense", 49));
            bulb.Add(new XElement("Sp.Atk", 65));
            bulb.Add(new XElement("Sp.Def", 65));
            bulb.Add(new XElement("Speed", 45));
            bulb.Add(new XElement("CatchRate", 45));

            var bMoves = new XElement("Moves");

            var bMove = new XElement("Move");
            bMove.Add(new XElement("Level", 1));
            bMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Tackle));
            bMoves.Add(bMove);

            bMove = new XElement("Move");
            bMove.Add(new XElement("Level", 1));
            bMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Growl));
            bMoves.Add(bMove);

            bMove = new XElement("Move");
            bMove.Add(new XElement("Level", 4));
            bMove.Add(new XElement("Name", MoveDictionary.moveDictionary.VineWhip));
            bMoves.Add(bMove);

            bMove = new XElement("Move");
            bMove.Add(new XElement("Level", 4));
            bMove.Add(new XElement("Name", MoveDictionary.moveDictionary.PoisonPowder));
            bMoves.Add(bMove);

            bulb.Add(bMoves);

            Pokes.Add(bulb);

            var ivy = new XElement("Poke");

            ivy.Add(new XElement("Number", 2));
            ivy.Add(new XElement("Name", "Ivysaur"));
            ivy.Add(new XElement("TypeOne", Pokemon.PokeType.Grass));
            ivy.Add(new XElement("TypeTwo", Pokemon.PokeType.Poison));
            ivy.Add(new XElement("Evolves", 3));
            ivy.Add(new XElement("EvolveLevel", 32));
            ivy.Add(new XElement("HP", 60));
            ivy.Add(new XElement("Attack", 62));
            ivy.Add(new XElement("Defense", 63));
            ivy.Add(new XElement("Sp.Atk", 80));
            ivy.Add(new XElement("Sp.Def", 80));
            ivy.Add(new XElement("Speed", 60));
            ivy.Add(new XElement("CatchRate", 45));

            var iMoves = new XElement("Moves");

            var iMove = new XElement("Move");
            iMove.Add(new XElement("Level", 1));
            iMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Tackle));
            iMoves.Add(iMove);

            iMove = new XElement("Move");
            iMove.Add(new XElement("Level", 1));
            iMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Growl));
            iMoves.Add(iMove);

            iMove = new XElement("Move");
            iMove.Add(new XElement("Level", 4));
            iMove.Add(new XElement("Name", MoveDictionary.moveDictionary.VineWhip));
            iMoves.Add(iMove);

            iMove = new XElement("Move");
            iMove.Add(new XElement("Level", 4));
            iMove.Add(new XElement("Name", MoveDictionary.moveDictionary.PoisonPowder));
            iMoves.Add(iMove);

            ivy.Add(iMoves);

            Pokes.Add(ivy);

            var charm = new XElement("Poke");

            charm.Add(new XElement("Number", 4));
            charm.Add(new XElement("Name", "Charmander"));
            charm.Add(new XElement("TypeOne", Pokemon.PokeType.Fire));
            charm.Add(new XElement("TypeTwo", Pokemon.PokeType.Null));
            charm.Add(new XElement("Evolves", 5));
            charm.Add(new XElement("EvolveLevel", 16));
            charm.Add(new XElement("HP", 39));
            charm.Add(new XElement("Attack", 52));
            charm.Add(new XElement("Defense", 43));
            charm.Add(new XElement("Sp.Atk", 60));
            charm.Add(new XElement("Sp.Def", 50));
            charm.Add(new XElement("Speed", 65));
            charm.Add(new XElement("CatchRate", 45));

            var cMoves = new XElement("Moves");

            var cMove = new XElement("Move");
            cMove.Add(new XElement("Level", 1));
            cMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Scratch));
            cMoves.Add(cMove);

            cMove = new XElement("Move");
            cMove.Add(new XElement("Level", 1));
            cMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Leer));
            cMoves.Add(cMove);

            cMove = new XElement("Move");
            cMove.Add(new XElement("Level", 4));
            cMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Ember));
            cMoves.Add(cMove);

            cMove = new XElement("Move");
            cMove.Add(new XElement("Level", 4));
            cMove.Add(new XElement("Name", MoveDictionary.moveDictionary.SmokeScreen));
            cMoves.Add(cMove);

            charm.Add(cMoves);

            Pokes.Add(charm);

            var squ = new XElement("Poke");

            squ.Add(new XElement("Number", 8));
            squ.Add(new XElement("Name", "Squirtle"));
            squ.Add(new XElement("TypeOne", Pokemon.PokeType.Water));
            squ.Add(new XElement("TypeTwo", Pokemon.PokeType.Null));
            squ.Add(new XElement("Evolves", 9));
            squ.Add(new XElement("EvolveLevel", 16));
            squ.Add(new XElement("HP", 44));
            squ.Add(new XElement("Attack", 48));
            squ.Add(new XElement("Defense", 65));
            squ.Add(new XElement("Sp.Atk", 50));
            squ.Add(new XElement("Sp.Def", 64));
            squ.Add(new XElement("Speed", 43));
            squ.Add(new XElement("CatchRate", 45));

            var sMoves = new XElement("Moves");

            var sMove = new XElement("Move");
            sMove.Add(new XElement("Level", 1));
            sMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Tackle));
            sMoves.Add(sMove);

            sMove = new XElement("Move");
            sMove.Add(new XElement("Level", 1));
            sMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Growl));
            sMoves.Add(sMove);

            sMove = new XElement("Move");
            sMove.Add(new XElement("Level", 4));
            sMove.Add(new XElement("Name", MoveDictionary.moveDictionary.WaterGun));
            sMoves.Add(sMove);

            sMove = new XElement("Move");
            sMove.Add(new XElement("Level", 4));
            sMove.Add(new XElement("Name", MoveDictionary.moveDictionary.DefenseCurl));
            sMoves.Add(sMove);

            squ.Add(sMoves);

            Pokes.Add(squ);

            var pid = new XElement("Poke");

            pid.Add(new XElement("Number", 16));
            pid.Add(new XElement("Name", "Pidgey"));
            pid.Add(new XElement("TypeOne", Pokemon.PokeType.Normal));
            pid.Add(new XElement("TypeTwo", Pokemon.PokeType.Flying));
            pid.Add(new XElement("Evolves", 17));
            pid.Add(new XElement("EvolveLevel", 18));
            pid.Add(new XElement("HP", 40));
            pid.Add(new XElement("Attack", 45));
            pid.Add(new XElement("Defense", 40));
            pid.Add(new XElement("Sp.Atk", 35));
            pid.Add(new XElement("Sp.Def", 35));
            pid.Add(new XElement("Speed", 56));
            pid.Add(new XElement("CatchRate", 255));

            var pMoves = new XElement("Moves");

            var pMove = new XElement("Move");
            pMove.Add(new XElement("Level", 1));
            pMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Tackle));
            pMoves.Add(pMove);

            pMove = new XElement("Move");
            pMove.Add(new XElement("Level", 5));
            pMove.Add(new XElement("Name", MoveDictionary.moveDictionary.SandAttack));
            pMoves.Add(pMove);

            pMove = new XElement("Move");
            pMove.Add(new XElement("Level", 9));
            pMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Gust));
            pMoves.Add(pMove);

            pMove = new XElement("Move");
            pMove.Add(new XElement("Level", 13));
            pMove.Add(new XElement("Name", MoveDictionary.moveDictionary.QuickAttack));
            pMoves.Add(pMove);

            pid.Add(pMoves);

            Pokes.Add(pid);

            var mtw = new XElement("Poke");

            mtw.Add(new XElement("Number", 150));
            mtw.Add(new XElement("Name", "Mewtwo"));
            mtw.Add(new XElement("TypeOne", Pokemon.PokeType.Psychic));
            mtw.Add(new XElement("TypeTwo", Pokemon.PokeType.Null));
            mtw.Add(new XElement("Evolves", -1));
            mtw.Add(new XElement("EvolveLevel", -1));
            mtw.Add(new XElement("HP", 106));
            mtw.Add(new XElement("Attack", 110));
            mtw.Add(new XElement("Defense", 90));
            mtw.Add(new XElement("Sp.Atk", 154));
            mtw.Add(new XElement("Sp.Def", 90));
            mtw.Add(new XElement("Speed", 130));
            mtw.Add(new XElement("CatchRate", 3));

            var mMoves = new XElement("Moves");

            var mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 1));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Confusion));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 1));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Disable));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 1));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Swift));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 1));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Psychic));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 63));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Barrier));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 66));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Psychic));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 70));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Recover));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 75));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Mist));
            mMoves.Add(mMove);

            mMove = new XElement("Move");
            mMove.Add(new XElement("Level", 81));
            mMove.Add(new XElement("Name", MoveDictionary.moveDictionary.Amnesia));
            mMoves.Add(mMove);

            mtw.Add(mMoves);

            Pokes.Add(mtw);


            doc.Add(Pokes);

            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("pokedex.xml", FileMode.Create, isoFile))
                {
                    using (StreamWriter sw = new StreamWriter(isoStream))
                    {
                        doc.Save(sw);
                    }
                }
            }

            */
           

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
                    pp[i] = ms.get(m[i]).PP;
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
                Pokemon pok = new Pokemon(Int32.Parse(q.Element("Number").Value), q.Element("Name").Value, lvl, parser.parsePokeType(q.Element("TypeOne").Value), parser.parsePokeType(q.Element("TypeTwo").Value), moves, Int32.Parse(q.Element("HP").Value), -9, Int32.Parse(q.Element("Attack").Value), Int32.Parse(q.Element("Defense").Value), Int32.Parse(q.Element("Sp.Atk").Value), Int32.Parse(q.Element("Sp.Def").Value), Int32.Parse(q.Element("Speed").Value), Int32.Parse(q.Element("CatchRate").Value));

                var evolQ = from ev in q.Elements("Evolutions").Elements("Evolution") select ev;

                List<EvolutionTrigger> evoList = new List<EvolutionTrigger>();
                ItemStore its = new ItemStore();
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
                            evoList.Add(new EvolutionTrigger(parser.parseItemName(t.Element("EvolveLevel").Value), into));
                        }
                    }
                }
                pok.evolvesInto = evoList;
                pok.PP = getPPArray(moves);
                ret.Add(pok);
            }
           
            return ret;

        }

        public Pokemon evolvePokemon(Pokemon p)
        {
            Pokemon t = pokesForTrainer(new List<int> { p.toEvolve }, new List<int> { p.level }).First();

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
                pokes.Add(new Pokemon(Int32.Parse(p.Element("Number").Value), p.Element("Name").Value, Int32.Parse(p.Element("Level").Value),  parser.parsePokeType(p.Element("TypeOne").Value), parser.parsePokeType(p.Element("TypeTwo").Value), movD, Int32.Parse(p.Element("HP").Value), Int32.Parse(p.Element("currentHP").Value), Int32.Parse(p.Element("Atk").Value), Int32.Parse(p.Element("Def").Value), Int32.Parse(p.Element("SpAtk").Value), Int32.Parse(p.Element("SpDef").Value), Int32.Parse(p.Element("Speed").Value), 0));
                pokes.Last().Experience = int.Parse(p.Element("Experience").Value);
                pokes.Last().PP = pp;

                pokes.Last().evolvesInto = getEvolutionTriggers(pokes.Last().Number);
                pokes.Last().MovesOnLevel = MovesOnLevelForPokemon(int.Parse(p.Element("Number").Value));
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
