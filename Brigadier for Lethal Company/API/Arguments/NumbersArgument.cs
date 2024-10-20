using Brigadier.NET;
using Brigadier.NET.Exceptions;
using System;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class NumbersArgument<T> : RuniArgumentType<T[]> where T : struct, IComparable<T>
    {
        public virtual T? minimum { get; }
        public virtual T? maximum { get; }

        readonly Func<IStringReader, T> readFunc;

        public virtual int argumentCount { get; }

        protected internal NumbersArgument(T? minimum, T? maximum, Func<IStringReader, T> readFunc, int argumentCount)
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
        }

        public override T[] Parse(IStringReader reader)
        {
            if (!reader.CanRead())
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);

            T[] results = new T[argumentCount];

            int firstCursor = reader.Cursor;

            for (int i = 0; i < argumentCount; i++)
            {
                if (i > 0)
                    reader.Cursor++;

                T value = readFunc.Invoke(reader);
                MinMaxThrow(reader, reader.Cursor, value);

                if (i < argumentCount - 1)
                    NextCanReadThrow(reader, firstCursor);

                results[i] = value;
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

        public override bool Equals(object? o) => o is NumbersArgument<T> arg && Equals(minimum, arg.minimum) && Equals(maximum, arg.maximum);

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
