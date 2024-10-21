using Brigadier.NET;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors
{
    public class SelectorArgument : RuniArgumentType<SelectorArgumentValue>
    {
        readonly bool onlyPlayer = false;
        readonly bool limit = false;

        protected internal SelectorArgument(bool onlyPlayer = false, bool limit = false)
        {
            this.onlyPlayer = onlyPlayer;
            this.limit = limit;
        }

        public override SelectorArgumentValue Parse(IStringReader reader)
        {
            if (!reader.CanRead())
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);

            if (reader.Peek() == '@')
            {
                reader.Skip();
                if (!reader.CanRead())
                    throw CommandSyntaxException.BuiltInExceptions.SelectorUnkown().CreateWithContext(reader, string.Empty);

                IEnumerable<NetworkBehaviour> entitys = GetEntitys();

                char peek = reader.Peek();
                reader.Skip();

                if (peek == 'a')
                    return new SelectorArgumentValue(GetPlayers(), SelectorType.none, GetOptions(reader));
                else if (peek == 'p')
                    return new SelectorArgumentValue(GetPlayers(), SelectorType.nearby, GetOptions(reader, false, true));
                else if (peek == 'r')
                    return new SelectorArgumentValue(GetPlayers(), SelectorType.random, GetOptions(reader, false, true));
                else if (peek == 'e')
                    return new SelectorArgumentValue(GetEntitys(), SelectorType.none, GetOptions(reader, true, false));
                else if (peek == 'n')
                    return new SelectorArgumentValue(GetEntitys(), SelectorType.nearby, GetOptions(reader, true, true));
                else if (peek == 's')
                    return new SelectorArgumentValue(Enumerable.Empty<NetworkBehaviour>(), SelectorType.sender, GetOptions(reader, true, true));
                else
                    throw CommandSyntaxException.BuiltInExceptions.SelectorUnkown().CreateWithContext(reader, peek);
            }
            else
            {
                string name = reader.ReadString();
                return new SelectorArgumentValue(GetPlayers().Where(x => x.playerUsername == name), SelectorType.none);
            }
        }

        /// <summary>현재 리더기의 커서 부분부터의 선택자 옵션을 구합니다</summary>
        /// <param name="reader">현재 리더기</param>
        /// <param name="existEntity">엔티티가 포함된 선택자 옵션을 가져오려면 true</param>
        /// <param name="isLimitedType">리미트 된 선택자라서 개수 검사를 할 필요가 없다면 true</param>
        public SelectorOption[] GetOptions(IStringReader reader, bool existEntity = false, bool isLimitedType = false)
        {
            SelectorOption[] result = Array.Empty<SelectorOption>();

            int? parsedMinLimit = null;
            bool parsedExistEntity = false;

            if (reader.CanRead() && !char.IsWhiteSpace(reader.Peek()))
            {
                reader.Expect('[');

                List<SelectorOption> options = new List<SelectorOption>();

                reader.Cursor--;

                do
                {
                    reader.Skip();

                    int nameCursor = reader.Cursor;
                    string name = reader.ReadString();

                    bool exists = false;
                    for (int i = 0; i < SelectorOption.selectorOptions.Count; i++)
                    {
                        (Type optionType, string optionName, bool optionOnlyEntity) = SelectorOption.selectorOptions[i];
                        if (name != optionName || (!existEntity && optionOnlyEntity))
                            continue;

                        reader.Expect('=');

                        SelectorOption option = (SelectorOption)Activator.CreateInstance(optionType, true);
                        option.Parse(reader);

                        // 구문 분석된 모든 타입 옵션에서 엔티티가 포함되어있을 경우를 감지합니다
                        if (existEntity && !parsedExistEntity && option is SelectorOptionType optionTypeInstance)
                            parsedExistEntity |= optionTypeInstance.types.Any(static x => x != SelectorOptionType.playerType);
                        else if (option is SelectorOptionLimit optionLimit && optionLimit.limit != null) // 구문 분석된 모든 리미트 옵션에서의 최소 리미트를 가져옵니다
                        {
                            if (parsedMinLimit != null)
                                parsedMinLimit = Mathf.Min(parsedMinLimit.Value, optionLimit.limit.Value);
                            else
                                parsedMinLimit = optionLimit.limit;
                        }

                        options.Add(option);
                        exists = true;
                    }

                    if (!exists)
                    {
                        reader.Cursor = nameCursor;
                        throw CommandSyntaxException.BuiltInExceptions.SelectorOptionsUnknown().CreateWithContext(reader, name);
                    }
                }
                while (reader.CanRead() && reader.Peek() == ',');

                reader.Expect(']');

                result = options.OrderBy(static x => x.sort).ToArray();
            }

            if (existEntity && parsedExistEntity && onlyPlayer)
                throw CommandSyntaxException.BuiltInExceptions.OnlyPlayer().CreateWithContext(reader);
            else if (!isLimitedType && limit && (parsedMinLimit == null || parsedMinLimit.Value > 1))
            {
                if (onlyPlayer)
                    throw CommandSyntaxException.BuiltInExceptions.PlayerToMany().CreateWithContext(reader);
                else
                    throw CommandSyntaxException.BuiltInExceptions.EntityToMany().CreateWithContext(reader);
            }

            return result;
        }

        public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder)
        {
            if ("@a".StartsWith(builder.Remaining))
                builder.Suggest("@a");
            if ("@p".StartsWith(builder.Remaining))
                builder.Suggest("@p");
            if ("@e".StartsWith(builder.Remaining))
                builder.Suggest("@e");
            if ("@s".StartsWith(builder.Remaining))
                builder.Suggest("@s");
            if ("@r".StartsWith(builder.Remaining))
                builder.Suggest("@r");
            if ("@n".StartsWith(builder.Remaining))
                builder.Suggest("@n");

            foreach (var item in GetPlayers())
            {
                bool singleQuote = true;
                bool unquotedString = true;

                for (int i = 0; i < item.playerUsername.Length; i++)
                {
                    char c = item.playerUsername[i];
                    if (!StringReader.IsAllowedInUnquotedString(c))
                        unquotedString = false;
                    if (c == '\'')
                        singleQuote = false;
                }

                string suggest;
                if (unquotedString)
                    suggest = item.playerUsername;
                else if (singleQuote)
                    suggest = "'" + item.playerUsername + "'";
                else
                    suggest = '"' + item.playerUsername.Replace("\"", "\\\"") + '"';

                if (suggest.StartsWith(builder.Remaining))
                    builder.Suggest(suggest);
            }

            return builder.BuildFuture();
        }

        static IEnumerable<PlayerControllerB> GetPlayers() => StartOfRound.Instance.ClientPlayerList.Values.Select(static x => StartOfRound.Instance.allPlayerScripts[x]);
        static IEnumerable<NetworkBehaviour> GetEntitys() => GetPlayers().OfType<NetworkBehaviour>().Concat(UnityEngine.Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None)).Concat(UnityEngine.Object.FindObjectsByType<Anomaly>(FindObjectsSortMode.None)).Concat(UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None));
    }
}
