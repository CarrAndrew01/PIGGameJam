using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundArrays : IEnumerable
{
    [SerializeField]
    SoundType type;
    public Sound[] sounds;

    public IEnumerator<Sound> GetEnumerator()
    {
        for (int i = 0; i < sounds.Length; ++i)
        {
            yield return sounds[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum SoundType
{
    MUSIC,
    AMBIENT,
    GENERIC,
    BLACKJACK
}
