﻿namespace InterReact;

public sealed class WshMetaData : IHasRequestId
{
    public int RequestId { get; }
    public string Data { get; } = "";
    public WshMetaData() { }
    internal WshMetaData(ResponseReader reader)
    {
        RequestId = reader.ReadInt();
        Data = reader.ReadString();
    }
}

public sealed class WshEventData : IHasRequestId
{
    public int RequestId { get; }
    public string Data { get; } = "";
    public WshEventData() { }
    internal WshEventData(ResponseReader reader)
    {
        RequestId = reader.ReadInt();
        Data = reader.ReadString();
    }
}
