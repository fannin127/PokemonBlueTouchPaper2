using System;
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
    public class Parser
    {
        public ItemName parseItemName(string s)
        {
            if (s.ToLower() == "potion")
            {
                return ItemName.Potion;
            }
            else if (s.ToLower() == "pokeball")
            {
                return ItemName.PokeBall;
            }
            else if (s.ToLower() == "greatball")
            {
                return ItemName.GreatBall;
            }
            else if (s.ToLower() == "ultraball")
            {
                return ItemName.UltraBall;
            }
            else if (s.ToLower() == "masterball")
            {
                return ItemName.MasterBall;
            }
            else if (s.ToLower() == "oldrod")
            {
                return ItemName.OldRod;
            }
            else if (s.ToLower() == "nurse")
            {
                return ItemName.Nurse;
            }
            else if (s.ToLower() == "water-stone")
            {
                return ItemName.WaterStone;
            }
            else if (s.ToLower() == "thunder-stone")
            {
                return ItemName.ThunderStone;
            }
            else if (s.ToLower() == "fire-stone")
            {
                return ItemName.FireStone;
            }
            else if (s.ToLower() == "leaf-stone")
            {
                return ItemName.LeafStone;
            }
            else if (s.ToLower() == "moon-stone")
            {
                return ItemName.MoonStone;
            }
            else
            {
                return ItemName.Null;
            }

        }

        public int tryParseInteger(string s, int elseReturn)
        {
            try
            {
                return int.Parse(s);
            }
            catch
            {
                return elseReturn;
            }
        }

        public StatusType parseStatus(string s)
        {
            switch (s.ToLower())
            {
                case "burn":
                    return StatusType.Burn;
                case "freeze":
                    return StatusType.Frozen;
                case "heal-block":
                    return StatusType.HealBlock;
                case "infatuation":
                    return StatusType.Infatuation;
                case "ingrain":
                    return StatusType.Ingrain;
                case "leech-seed":
                    return StatusType.LeechSeed;
                case "nightmare":
                    return StatusType.NightMare;
                case "embargo":
                    return StatusType.NoHoldItem;
                case "no-type-immunity":
                    return StatusType.NoImmunity;
                case "paralysis":
                    return StatusType.Paralyze;
                case "perish-song":
                    return StatusType.PerishSong;
                case "poison":
                    return StatusType.Poison;
                case "sleep":
                    return StatusType.Sleep;
                case "unknown":
                    return StatusType.Telekenesis;
                case "torment":
                    return StatusType.Torment;
                case "confusion":
                    return StatusType.Confusion;
            }
            return StatusType.Null;
        }

        public bool parseToFoe(string s)
        {
            //TODO
            if (s == "user")
            {
                return false;
            }
            else if (s == "opponents-field")
            {
                return true;
            } else if (s == "users-field")
            {
                return false;
            } else
            {
                return true;
            }
        }

        public Pokemon.Stat parsePokemonStat(string s)
        {
            switch (s.ToLower())
            {
                case "hp":
                    return Pokemon.Stat.HP;
                case "attack":
                    return Pokemon.Stat.Attack;
                case "defense":
                    return Pokemon.Stat.Defense;
                case "special-attack":
                    return Pokemon.Stat.SpAtk;
                case "special-defense":
                    return Pokemon.Stat.SpDef;
                case "speed":
                    return Pokemon.Stat.Speed;
                case "accuracy":
                    return Pokemon.Stat.Accuracy;
                default:
                    return Pokemon.Stat.Evasion;
            }
        }

        public MoveStore.Speciality parseSpeciality(string s)
        {
            switch (s.ToLower())
            {
                case "physical":
                    return MoveStore.Speciality.Physical;
                case "special":
                    return MoveStore.Speciality.Special;
                default:
                    return MoveStore.Speciality.Null;
            }
        }

        public Pokemon.PokeType parsePokeType(string s)
        {
            switch (s.ToLower())
            {
                case "grass":
                    return Pokemon.PokeType.Grass;
                case "fire":
                    return Pokemon.PokeType.Fire;
                case "water":
                    return Pokemon.PokeType.Water;
                case "poison":
                    return Pokemon.PokeType.Poison;
                case "normal":
                    return Pokemon.PokeType.Normal;
                case "null":
                    return Pokemon.PokeType.Null;
                case "ground":
                    return Pokemon.PokeType.Ground;
                case "rock":
                    return Pokemon.PokeType.Rock;
                case "electric":
                    return Pokemon.PokeType.Electric;
                case "dragon":
                    return Pokemon.PokeType.Dragon;
                case "dark":
                    return Pokemon.PokeType.Dark;
                case "ghost":
                    return Pokemon.PokeType.Ghost;
                case "psychic":
                    return Pokemon.PokeType.Psychic;
                case "ice":
                    return Pokemon.PokeType.Ice;
                case "flying":
                    return Pokemon.PokeType.Flying;
                case "fairy":
                    return Pokemon.PokeType.Fairy;
                case "bug":
                    return Pokemon.PokeType.Bug;
                case "steel":
                    return Pokemon.PokeType.Steel;
                case "fighting":
                    return Pokemon.PokeType.Fighting;
            }

            throw new TypeNotParseException();
        }

        public MoveCategory parseMoveCategory(string s)
        {
            switch (s.ToLower())
            {
                default:
                    return MoveCategory.Unique;
                case "damage":
                    return MoveCategory.Attacking;
                case "damage+ailment":
                    return MoveCategory.DamageAndAilement;
                case "ohko":
                    return MoveCategory.KO;
                case "ailment":
                    return MoveCategory.Status;
                case "force-switch":
                    return MoveCategory.Switch;
                case "net-good-stats":
                    return MoveCategory.StatChange;
                case "damage+lower":
                    return MoveCategory.DamageAndLower;
                case "field-effect":
                    return MoveCategory.SideField;
                case "damage+heal":
                    return MoveCategory.Absorbing;
                case "whole-field-effect":
                    return MoveCategory.Field;
                case "swagger":
                    return MoveCategory.StatusAndRaiseStats;
                case "heal":
                    return MoveCategory.Healing;
                case "damage+raise":
                    return MoveCategory.DamageAndRaise;

            }
        }

        public RouteName parseRouteName(string rn)
        {
            if (rn == "OakLab")
            {
                return RouteName.OakLab;
            }
            else if (rn == "PokeCentreR2")
            {
                return RouteName.PokeCentreR2;
            }
            else if (rn == "Route1")
            {
                return RouteName.Route1;
            }
            else if (rn == "Route2")
            {
                return RouteName.Route2;
            }
            else if (rn == "TurnTown")
            {
                return RouteName.TurnTown;
            }
            else if (rn == "PokeCentreTurnTown")
            {
                return RouteName.PokeCentreTurnTown;
            }
            else if (rn == "PokeMartTurnTown")
            {
                return RouteName.PokeMartTurnTown;
            }
            else if (rn == "GymTurnTown")
            {
                return RouteName.GymTurnTown;
            }
            else
            {
                throw new TypeNotParseException();
            }
        }
    }
}
