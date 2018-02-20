using System;

namespace SilverlightApplication3
{
    public abstract class Item : IEquatable<Item>
    {
        public ItemName Name { get; }
        public string Description { get; }
        public ItemType Type { get;  }

        public int Price { get;}
        public Item(ItemName n, string desc, ItemType type)
        {
            Name = n;
            Description = desc;
            Type = type;
            Price = 100;
        }

        public bool Equals(Item other)
        {
            if (other != null)
            return this.Name == other.Name;

            return false;
        }

        public override bool Equals(Object o)
        {
            return this.Equals(o as Item);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name.ToString();
        }

    }

    
}