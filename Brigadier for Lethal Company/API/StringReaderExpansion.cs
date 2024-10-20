using Brigadier.NET;
using Brigadier.NET.Exceptions;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using System;

namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>
    /// This is an extension of the <see cref="IStringReader"/> implementation.
    /// <br/><br/>
    /// <see cref="IStringReader"/> 구현의 확장입니다
    /// </summary>
    public static class StringReaderExpansion
    {
        /// <summary>
        /// Reads and returns a swizzle string at the current location
        /// <br/><br/>
        /// 현재 위치에서 스위즐 문자열을 읽고, 반환합니다
        /// </summary>
        public static PosSwizzleEnum ReadPosSwizzle(this IStringReader reader)
        {
            PosSwizzleEnum posSwizzle = PosSwizzleEnum.none;
            while (reader.CanRead() && !char.IsWhiteSpace(reader.Peek()))
            {
                if (reader.Peek() == 'x')
                {
                    reader.Cursor++;

                    if (!posSwizzle.HasFlag(PosSwizzleEnum.x))
                    {
                        if (!posSwizzle.HasFlag(PosSwizzleEnum.none))
                            posSwizzle |= PosSwizzleEnum.x;
                        else
                            posSwizzle = PosSwizzleEnum.x;
                    }
                    else
                        throw CommandSyntaxException.BuiltInExceptions.InvalidPosSwizzle().CreateWithContext(reader);
                }
                else if (reader.Peek() == 'y')
                {
                    reader.Cursor++;

                    if (!posSwizzle.HasFlag(PosSwizzleEnum.y))
                    {
                        if (!posSwizzle.HasFlag(PosSwizzleEnum.none))
                            posSwizzle |= PosSwizzleEnum.y;
                        else
                            posSwizzle = PosSwizzleEnum.y;
                    }
                    else
                        throw CommandSyntaxException.BuiltInExceptions.InvalidPosSwizzle().CreateWithContext(reader);
                }
                else if (reader.Peek() == 'z')
                {
                    reader.Cursor++;

                    if (!posSwizzle.HasFlag(PosSwizzleEnum.z))
                    {
                        if (!posSwizzle.HasFlag(PosSwizzleEnum.none))
                            posSwizzle |= PosSwizzleEnum.z;
                        else
                            posSwizzle = PosSwizzleEnum.z;
                    }
                    else
                        throw CommandSyntaxException.BuiltInExceptions.InvalidPosSwizzle().CreateWithContext(reader);
                }
                else
                    throw CommandSyntaxException.BuiltInExceptions.InvalidPosSwizzle().CreateWithContext(reader);
            }

            return posSwizzle;
        }

        /// <summary>
        /// Reads and returns a string <see cref="Range"/> from the current location
        /// <br/><br/>
        /// 현재 위치에서 <see cref="Range"/> 문자열을 읽고, 반환합니다
        /// </summary>
        public static Range ReadRange(this IStringReader reader)
        {
            int cursor = reader.Cursor;

            string text = ReadNumberString(reader);
            string[] range = text.Split("..");

            if (range.Length == 2)
            {
                float? min = null;
                string minText = range[0];

                if (!string.IsNullOrEmpty(minText))
                {
                    try
                    {
                        min = float.Parse(minText);
                    }
                    catch (FormatException)
                    {
                        reader.Cursor = cursor;
                        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat().CreateWithContext(reader, minText);
                    }
                }

                float? max = null;
                string maxText = range[1];

                if (!string.IsNullOrEmpty(maxText))
                {
                    try
                    {
                        max = float.Parse(maxText);
                    }
                    catch (FormatException)
                    {
                        reader.Cursor = cursor;
                        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat().CreateWithContext(reader, maxText);
                    }
                }

                if (min == null && max == null)
                    throw CommandSyntaxException.BuiltInExceptions.RangeEmpty().CreateWithContext(reader);

                return new Range(min, max);
            }
            else if (range.Length == 1)
            {
                try
                {
                    return new Range(float.Parse(text));
                }
                catch (FormatException)
                {
                    reader.Cursor = cursor;
                    throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat().CreateWithContext(reader, text);
                }
            }

            throw CommandSyntaxException.BuiltInExceptions.RangeEmpty().CreateWithContext(reader);
        }

        /// <summary>
        /// Reads and returns a string <see cref="RangeInt"/> at the current location
        /// <br/><br/>
        /// 현재 위치에서 <see cref="RangeInt"/> 문자열을 읽고, 반환합니다
        /// </summary>
        public static RangeInt ReadRangeInt(this IStringReader reader)
        {
            int cursor = reader.Cursor;

            string text = ReadNumberString(reader);
            string[] range = text.Split("..");

            if (range.Length == 2)
            {
                int? min;
                try
                {
                    min = int.Parse(range[0]);
                }
                catch (FormatException)
                {
                    reader.Cursor = cursor;
                    throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidInt().CreateWithContext(reader, range[0]);
                }

                int? max;
                try
                {
                    max = int.Parse(range[1]);
                }
                catch (FormatException)
                {
                    reader.Cursor = cursor;
                    throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidInt().CreateWithContext(reader, range[1]);
                }

                if (min == null && max == null)
                    throw CommandSyntaxException.BuiltInExceptions.RangeEmpty().CreateWithContext(reader);

                return new RangeInt(min, max);
            }
            else if (range.Length == 1)
            {
                try
                {
                    return new RangeInt(int.Parse(text));
                }
                catch (FormatException)
                {
                    reader.Cursor = cursor;
                    throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidInt().CreateWithContext(reader, text);
                }
            }

            throw CommandSyntaxException.BuiltInExceptions.RangeEmpty().CreateWithContext(reader);
        }

        /// <summary>
        /// Reads and returns a string corresponding to a number
        /// <br/><br/>
        /// 숫자에 해당하는 문자열을 읽고 반환합니다
        /// </summary>
        public static string ReadNumberString(this IStringReader reader)
        {
            int cursor = reader.Cursor;
            while (reader.CanRead() && IsAllowedNumber(reader.Peek()))
                reader.Skip();

            return reader.String.Substring(cursor, reader.Cursor - cursor);
        }

        /// <summary>
        /// Returns true if the character is a number
        /// <br/><br/>
        /// 문자가 숫자라면 true를 반환합니다
        /// </summary>
        public static bool IsAllowedNumber(char c) => (c >= '0' && c <= '9') || c == '-' || c == '.';
    }
}
