﻿namespace InterReact;

public sealed class NewsBulletin
{
    public int MessageId { get; }

    public NewsBulletinType Type { get; } = NewsBulletinType.Undefined;

    public string Message { get; } = "";

    /// <summary>
    /// The exchange from which this message originated.
    /// </summary>
    public string Origin { get; } = "";

    internal NewsBulletin() { }

    internal NewsBulletin(ResponseReader r)
    {
        r.IgnoreVersion();
        MessageId = r.ReadInt();
        Type = r.ReadEnum<NewsBulletinType>();
        Message = r.ReadString();
        Origin = r.ReadString();
    }
}
