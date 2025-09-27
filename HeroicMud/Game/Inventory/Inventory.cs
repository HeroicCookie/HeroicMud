using System.Runtime.InteropServices;

namespace HeroicMud.Game.Inventory;

public class Inventory
{
    public readonly int MaxItems = 20;

    private readonly List<Item> _items = new([]);

}

public abstract class Item
{
    public virtual string Name { get => "Unnamed"; }
    public virtual int StackLimit { get => 1; }
    public virtual ItemCategory Category { get => ItemCategory.Misc; }
}

public enum ItemCategory
{
    Misc,
    Armor,
    Weapon,
    Treasure,
    Drink,
    Food,
    Ammo,
    Tool
}
