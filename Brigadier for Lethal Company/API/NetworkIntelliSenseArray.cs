using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>
    /// 네트워크로 전송 가능한 인텔리센스 배열입니다.
    /// </summary>
    public struct NetworkIntelliSenseArray(NetworkIntelliSenseArray.Type type, NetworkIntelliSenseArray.Suggestion[] texts) : INetworkSerializable, IEnumerable<NetworkIntelliSenseArray.Suggestion>
    {
        public Type type = type;

        Suggestion[]? texts = texts;

        public readonly int length => texts?.Length ?? 0;

        public enum Type
        {
            suggestion,
            usage,
            exception
        }

        public readonly Suggestion this[int index]
        {
            get => texts?[index] ?? Array.Empty<Suggestion>()[index];
            set
            {
                if (texts != null)
                    texts[index] = value;
                else
                    Array.Empty<Suggestion>()[index] = value;
            }
        }

        public override readonly string ToString() => $"NetworkIntelliSenseArray{{isSuggestion={type}, length={length}}}";

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref type);

            int length = texts != null ? texts.Length : 0;
            serializer.SerializeValue(ref length);

            if (serializer.IsReader)
                texts = new Suggestion[length];

            for (int n = 0; n < length; ++n)
                serializer.SerializeValue(ref texts![n]);
        }

        public readonly IEnumerator<Suggestion> GetEnumerator() => (texts as IEnumerable<Suggestion>)?.GetEnumerator() ?? Enumerable.Empty<Suggestion>().GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Suggestion(StringRange range, string text) : INetworkSerializable
        {
            public StringRange range = range;
            public string text = text;

            public static implicit operator Suggestion(string text) => new Suggestion(new StringRange(), text);
            public static implicit operator Suggestion(Brigadier.NET.Suggestion.Suggestion suggestion) => new Suggestion(suggestion.Range, suggestion.Text);

            void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
            {
                serializer.SerializeValue(ref text);
                serializer.SerializeValue(ref range);
            }

            public override readonly string ToString() => $"NetworkIntelliSenseArray.Suggestion{{range={range}, text={text}}}";
        }

        public struct StringRange(int start, int end) : INetworkSerializable
        {
            public int start = start;
            public int end = end;

            public readonly string Get(string source) => source.Substring(start, end - start);

            public static implicit operator StringRange(Brigadier.NET.Context.StringRange range) => new StringRange(range.Start, range.End);

            void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
            {
                serializer.SerializeValue(ref start);
                serializer.SerializeValue(ref end);
            }

            public override readonly string ToString() => $"NetworkIntelliSenseArray.StringRange{{start={start}, end={end}}}";
        }
    }
}
