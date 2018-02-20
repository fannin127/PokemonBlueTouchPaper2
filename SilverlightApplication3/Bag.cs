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

  
    public class Bag
    {
        public Dictionary<HealAndStatusItem, int> items { get; }
        public Dictionary<Pokeball, int> balls { get; }
        public Dictionary<KeyItem, int> keyItems { get; }

        public Bag()
        {
            items = new Dictionary<HealAndStatusItem, int>();
            balls = new Dictionary<Pokeball, int>();
            keyItems = new Dictionary<KeyItem, int>();
        }
        public void Add(Item i)
        {
            if (i.Type == ItemType.HealAndStatus)
            {
               if (items.ContainsKey((HealAndStatusItem)i))
                {
                    ++items[(HealAndStatusItem)i];
                } else
                {
                    items.Add((HealAndStatusItem)i, 1); 
                }
                
            } else if (i.Type == ItemType.KeyItem)
            {
                if (keyItems.ContainsKey((KeyItem)i))
                {
                    ++keyItems[(KeyItem)i];
                }
                else
                {
                    keyItems.Add((KeyItem)i, 1);
                }
            } else if (i.Type == ItemType.Pokeball)
            {
                if (balls.ContainsKey((Pokeball)i))
                {
                    ++balls[(Pokeball)i];
                }
                else
                {
                    balls.Add((Pokeball)i, 1);
                }
            }
        }

        public void Add(Item item, int i)
        {
            for (int j = 0; j < i; ++j)
            {
                Add(item);
            }
        }

        public int getCountForItem(Item item, ItemType currentBagOpen)
        {
            if (currentBagOpen == ItemType.HealAndStatus)
            {
                var d = from b in items where b.Key.Name == item.Name select b.Key;
                return items[d.First()];
            } else if (currentBagOpen == ItemType.Pokeball)
            {
                var d = from b in balls where b.Key.Name == item.Name select b.Key;
                return balls[d.First()];

            } else if (currentBagOpen == ItemType.KeyItem)
            {
                return 1;
            }
            return 0;
        }

        internal void Remove(Item item, ItemType currentBagOpen)
        {
            if (currentBagOpen == ItemType.HealAndStatus)
            {
                --items[item as HealAndStatusItem];
                if (items[item as HealAndStatusItem] == 0)
                {
                    items.Remove(item as HealAndStatusItem);
                }
            }
            else if (currentBagOpen == ItemType.Pokeball)
            {
                --balls[item as Pokeball];
                if (balls[item as Pokeball] == 0)
                {
                    balls.Remove(item as Pokeball);
                }
            }
            else if (currentBagOpen == ItemType.KeyItem)
            {
                --keyItems[item as KeyItem];
                if (keyItems[item as KeyItem] == 0)
                {
                    keyItems.Remove(item as KeyItem);
                }
            }
        }

        public void Remove(Item i, ItemType currentBag, int n)
        {
            for(int l = 0; l < n; ++l)
            {
                Remove(i, currentBag);
            }
        }
    }
}
