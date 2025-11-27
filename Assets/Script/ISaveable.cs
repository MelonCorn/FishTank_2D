using UnityEngine;

public interface ISaveable
{
    public void Save(SaveData saveData);
    public void Load(SaveData saveData);
}
