﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using StringEnums;
using RxSockets;

namespace InterReact
{
    public sealed class RequestMessage
    {
        private readonly List<string> Strings = new();
        private readonly IRxSocketClient RxSocket;
        private readonly Limiter Limiter;

        internal RequestMessage(IRxSocketClient rxSocket, Limiter limiter)
        {
            RxSocket = rxSocket;
            Limiter = limiter;
        }

        // V100Plus format: 4 byte message length prefix plus payload of null-terminated strings.
        internal byte[] Get() => Strings.ToByteArray().ToByteArrayWithLengthPrefix();
        internal void Send() => Limiter.Limit(() => RxSocket.Send(Get()));

        /////////////////////////////////////////////////////

        internal RequestMessage Write(params object?[]? objs)
        {
            if (objs == null)
                Strings.Add(string.Empty);
            else if (objs.Length == 0)
                throw new ArgumentException("invalid length", nameof(objs));
            else foreach (object? o in objs)
                    Strings.Add(GetString(o));
            return this;
        }

        private static string GetString(object? o)
        {
            if (o == null)
                return "";

            switch (o)
            {
                case string s:
                    return s;
                case bool bo:
                    return bo ? "1" : "0";
                case Enum e:
                    Type ut = Enum.GetUnderlyingType(e.GetType());
                    return Convert.ChangeType(e, ut).ToString() ?? "";
            }

            Type type = o.GetType();

            if (type.IsStringEnum())
                return o.ToString() ?? "";

            Type? utype = Nullable.GetUnderlyingType(type);
            if (utype != null)
            {
                if (utype != typeof(int) && utype != typeof(long) && utype != typeof(double))
                    throw new InvalidDataException($"Nullable '{utype.Name}' is not supported.");
                o = Convert.ChangeType(o, utype);
            }

            return o switch
            {
                int i => i.ToString(NumberFormatInfo.InvariantInfo),
                long l => l.ToString(NumberFormatInfo.InvariantInfo),
                double d => d.ToString(NumberFormatInfo.InvariantInfo),
                _ => throw new InvalidDataException($"RequestMessage: unsupported data type = {o.GetType().Name}."),
            };
        }

        internal RequestMessage WriteContract(Contract c) =>
            Write(c.ContractId, c.Symbol, c.SecurityType, c.LastTradeDateOrContractMonth, c.Strike, c.Right, c.Multiplier,
                c.Exchange, c.PrimaryExchange, c.Currency, c.LocalSymbol, c.TradingClass, c.IncludeExpired);
    }

}
