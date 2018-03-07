using System;
using System.Collections.Generic;
using System.Linq;


namespace SilverlightApplication3
{
    public class ItemStore 
    {
        private static ItemStore instance;

        public static ItemStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemStore();
                }
                return instance;
            }
        }
        HashSet<Item> itemDefs = new HashSet<Item>();

        private ItemStore()
        {
            itemDefs.Add(new HealAndStatusItem(ItemName.Potion, "Restores the HP of a Pokémon by 20 points", 20));
            itemDefs.Add(new Pokeball(ItemName.PokeBall, "A tool for catching wild Pokémon", 1.0));
            itemDefs.Add(new Pokeball(ItemName.GreatBall, "	A good Ball with a higher catch rate than a Poké Ball", 1.5));
            itemDefs.Add(new Pokeball(ItemName.UltraBall, "A better Ball with a higher catch rate than a Great Ball", 2.0));
            itemDefs.Add(new Pokeball(ItemName.MasterBall, "The best Ball that catches a Pokémon without fail", 255.0));
            itemDefs.Add(new KeyItem(ItemName.OldRod, "Use by any body of water to fish for wild Pokémon"));
            itemDefs.Add(new NurseItem(ItemName.Nurse, "NurseJoy", Pokebuilder.Instance));
            itemDefs.Add(new EvolutionItem(ItemName.FireStone, "A peculiar stone that can be used to evolve specific pokemon.", new List<int>() { 37, 58, 133}));
        }

        public Item get(ItemName name)
        {
            return itemDefs.Where((n) => n.Name.Equals(name)).FirstOrDefault();
        }
   
    }

    public enum ItemType { HealAndStatus, Pokeball, KeyItem, NotBaggable };

    public enum ItemName { Null, Potion, PokeBall, GreatBall, UltraBall, MasterBall, OldRod, Nurse, WaterStone, ThunderStone, FireStone, LeafStone, MoonStone }


}