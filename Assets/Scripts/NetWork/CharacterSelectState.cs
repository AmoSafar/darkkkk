using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong clientID;
    public int characterID;

    public CharacterSelectState(ulong clientID, int characterID = -1){
        this.clientID = clientID;
        this.characterID = characterID;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref characterID);
    }

    public bool Equals(CharacterSelectState other)
    {
        return clientID == other.clientID && characterID == other.characterID;
    }
}