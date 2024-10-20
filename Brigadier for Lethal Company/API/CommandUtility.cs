using Brigadier.NET;
using Brigadier.NET.Context;
using Brigadier.NET.Suggestion;
using Brigadier.NET.Tree;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>
    /// 커맨드에서 유용한 유틸리티 클래스입니다
    /// </summary>
    public static class CommandUtility
    {
        /// <summary>
        /// 명령어 입력 값에 해당하는 디스패처의 인틀리샌스 텍스트를 가져옵니다
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dispatcher">가져올 디스패처</param>
        /// <param name="input">입력 값</param>
        /// <param name="source">현재 소스</param>
        /// <param name="cursor">현재 입력 커서</param>
        /// <returns></returns>
        public static async Task<string> GetIntelliSenseText<TSource>(this CommandDispatcher<TSource> dispatcher, string input, TSource source, int cursor)
        {
            if (cursor < 0 || cursor > input.Length)
                return string.Empty;

            StringReader stringReader = new StringReader(input);
            ParseResults<TSource> parseResults = dispatcher.Parse(stringReader, source);

            // 자동 완성
            {
                List<Suggestion> suggestions = (await dispatcher.GetCompletionSuggestions(parseResults, cursor)).List;
                if (suggestions.Count > 0)
                    return string.Join("\n", suggestions.Select(x => x.Text));
            }

            // 예외
            {
                var exceptions = parseResults.Exceptions;
                if (exceptions.Count > 0)
                    return string.Join("\n", exceptions.Select(x => x.Value.Message));
            }

            // 설명
            {
                SuggestionContext<TSource> context = parseResults.Context.FindSuggestionContext(cursor);
                if (context.Parent.Children.Any(x => x is ArgumentCommandNode<TSource>))
                {
                    ICollection<string> usages = dispatcher.GetSmartUsage(context.Parent, source).Values;
                    if (usages.Count > 0)
                        return string.Join("\n", usages);
                }
            }

            return string.Empty;
        }
    }
}
