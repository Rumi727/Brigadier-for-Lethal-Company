using Brigadier.NET;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class CoordinatesArgument<T> : RuniArgumentType<CoordinatesArgumentValue<T>[]> where T : struct, IComparable<T>
    {
        public virtual T? minimum { get; }
        public virtual T? maximum { get; }

        readonly Func<IStringReader, T> readFunc;

        public virtual int argumentCount { get; }
        public virtual int localArgumentCount { get; }

        protected internal CoordinatesArgument(T? minimum, T? maximum, Func<IStringReader, T> readFunc, int argumentCount, int localArgumentCount = 0)
        {
            if (minimum == null || maximum == null || minimum.Value.CompareTo(maximum.Value) < 0)
            {
                this.minimum = minimum;
                this.maximum = maximum;
            }
            else
            {
                this.minimum = maximum;
                this.maximum = minimum;
            }

            this.readFunc = readFunc;

            this.argumentCount = argumentCount;
            this.localArgumentCount = localArgumentCount;
        }

        public T Read(IStringReader reader, ref CoordinateType numberBaseType)
        {
            if (!reader.CanRead())
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);

            if (reader.Peek() == '~')
            {
                if (numberBaseType == CoordinateType.local)
                    throw CommandSyntaxException.BuiltInExceptions.WorldLocalPosMixed().CreateWithContext(reader);

                reader.Skip();
                numberBaseType = CoordinateType.offset;

                if (!reader.CanRead() || char.IsWhiteSpace(reader.Peek()))
                    return default;
                else
                    return readFunc.Invoke(reader);
            }
            else if (localArgumentCount > 0 && reader.Peek() == '^')
            {
                if (numberBaseType != CoordinateType.none && numberBaseType != CoordinateType.local)
                    throw CommandSyntaxException.BuiltInExceptions.WorldLocalPosMixed().CreateWithContext(reader);

                reader.Skip();
                numberBaseType = CoordinateType.local;

                if (!reader.CanRead() || char.IsWhiteSpace(reader.Peek()))
                    return default;
                else
                    return readFunc.Invoke(reader);
            }
            else
            {
                if (numberBaseType == CoordinateType.local)
                    throw CommandSyntaxException.BuiltInExceptions.WorldLocalPosMixed().CreateWithContext(reader);

                numberBaseType = CoordinateType.world;
                return readFunc.Invoke(reader);
            }
        }

        public override CoordinatesArgumentValue<T>[] Parse(IStringReader reader)
        {
            CoordinatesArgumentValue<T>[] results = new CoordinatesArgumentValue<T>[argumentCount];

            int firstCursor = reader.Cursor;
            CoordinateType coordinatesType = CoordinateType.none;

            for (int i = 0; i < (coordinatesType == CoordinateType.local ? localArgumentCount : argumentCount); i++)
            {
                if (i > 0)
                    reader.Skip();

                T value = Read(reader, ref coordinatesType);
                MinMaxThrow(reader, reader.Cursor, value);

                if (i < argumentCount - 1)
                    NextCanReadThrow(reader, firstCursor);

                results[i] = new CoordinatesArgumentValue<T>(value, coordinatesType);
            }

            return results;
        }

        void MinMaxThrow(IStringReader reader, int cursor, T value)
        {
            if (minimum != null && value.CompareTo(minimum.Value) < 0)
            {
                reader.Cursor = cursor;
                throw CommandSyntaxException.BuiltInExceptions.FloatTooLow().CreateWithContext(reader, value, minimum);
            }

            if (maximum != null && value.CompareTo(maximum.Value) > 0)
            {
                reader.Cursor = cursor;
                throw CommandSyntaxException.BuiltInExceptions.FloatTooHigh().CreateWithContext(reader, value, maximum);
            }
        }

        void NextCanReadThrow(IStringReader reader, int firstCursor)
        {
            if (!reader.CanRead(2))
            {
                reader.Cursor = firstCursor;
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
            }
        }

        public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder)
        {
            string input = builder.Remaining.Trim();
            if (input.StartsWith('~'))
                builder.Suggest("~ ~ ~");
            else if (input.StartsWith('^'))
                builder.Suggest("^ ^ ^");

            return builder.BuildFuture();
        }

        public override bool Equals(object? o) => o is CoordinatesArgument<T> arg && Equals(minimum, arg.minimum) && Equals(maximum, arg.maximum);

        public override int GetHashCode()
        {
            unchecked
            {
                return GetType().GetHashCode() * minimum.GetHashCode() * maximum.GetHashCode();
            }
        }

        public override string ToString() => $"{typeof(T)}({(minimum != null ? minimum : string.Empty)}~{(maximum != null ? maximum : string.Empty)})";
    }
}
