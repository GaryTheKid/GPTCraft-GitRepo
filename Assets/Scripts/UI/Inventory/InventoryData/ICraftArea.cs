using System;

public interface ICraftArea
{
    void WithdrawAllCraftItems(Func<ItemData, int, bool> WithdrawItemFunc);
    void RemoveOneElementForEachSlot();
    public abstract string GetCraftHashString();
}
