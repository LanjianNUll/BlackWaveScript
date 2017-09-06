using System;

namespace Network.Serializer
{
    interface INetStreamPickler
    {
        object PickFromNetStreamObject(INetStreamObject value);
        INetStreamObject UnpackToNetStreamObject(object dict);
    }

    interface INetStreamObject
    { }
}
