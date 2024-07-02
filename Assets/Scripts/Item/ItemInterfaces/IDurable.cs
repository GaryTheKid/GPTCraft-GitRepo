public interface IDurable
{
    public abstract short GetMaxDurability();
    public abstract bool UpdateDurability(ItemData itemData, short changeAmount);
}
