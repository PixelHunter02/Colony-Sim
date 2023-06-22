using System;


[System.Serializable]
public sealed class Resource : IEquatable<Resource>

{
    public PickUpItemSO itemSO;
    public int amount;

    public override bool Equals(object other)
    {
        return Equals(other as Resource);
    }

    public bool Equals(Resource other)
    {
        if (other == null)
        {
            return false;
        }

        return other.itemSO == itemSO && other.amount == amount;
    }
    
    public override int GetHashCode()
    {
        int hash = 19;
        hash = hash * 31 + (itemSO == null ? 0 : itemSO.GetHashCode());
        hash = hash * 31 + amount.GetHashCode();
        return hash;
    }
}
