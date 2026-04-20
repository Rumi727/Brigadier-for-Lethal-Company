#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
using Brigadier.NET;
using Brigadier.NET.Context;
using Brigadier.NET.Suggestion;
using Brigadier.NET.Tree;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API;
using StaticNetcodeLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.BrigadierForLethalCompany
{
    [StaticNetcode]
    public static class BFLCUtility
    {
        /// <summary>
        /// 서버 측에서 특정 클라이언트로 커맨드 결과를 전송합니다
        /// </summary>
        public static void SendChat(string text, PlayerControllerB? targetPlayer)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            InternalChatClientRpc(text, targetPlayer.ToRpc());
        }

        /// <summary>
        /// 서버 측에서 특정 클라이언트로 커맨드 결과를 전송합니다
        /// </summary>
        public static void SendChat(string text, params IEnumerable<PlayerControllerB?> targetPlayers)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (targetPlayers.Any())
                InternalChatClientRpc(text, targetPlayers.ToRpc());
        }

        [ClientRpc]
        static void InternalChatClientRpc(string text, ClientRpcParams rpcParams = default) => AddChatClient(text);

        /// <summary>
        /// 클라이언트에 "하얀색" 채팅을 추가합니다
        /// </summary>
        public static void AddChatClient(string text)
        {
            HUDManager x = HUDManager.Instance;

            if (x.ChatMessageHistory.Count >= 4)
                x.ChatMessageHistory.Remove(x.ChatMessageHistory[0]);

            x.ChatMessageHistory.Add("<color=white>" + text + "</color>");

            x.chatText.text = string.Empty;
            for (int i = 0; i < x.ChatMessageHistory.Count; i++)
                x.chatText.text += "\n" + x.ChatMessageHistory[i];
        }

        public static string GetEntityName(this NetworkBehaviour entity) => Enumerable.Repeat(entity, 1).GetEntityName(1);

        public static string GetEntityName(this IEnumerable<NetworkBehaviour> entitys, int count = -1)
        {
            if (entitys.CountIsOne())
            {
                NetworkBehaviour entity = entitys.First();
                if (entity is PlayerControllerB player)
                    return player.playerUsername;
                else if (entity is EnemyAI enemy)
                    return enemy.enemyType.enemyName;
                else if (entity is Anomaly anomaly)
                    return anomaly.anomalyType.anomalyName;
                else if (entity is GrabbableObject item)
                    return item.itemProperties.itemName;

                return entity.name;
            }

            if (count < 0)
                return $"{entitys.Count()} entities";
            else
                return $"{count} entities";
        }

        public static string GetPlayerName(this IEnumerable<PlayerControllerB> entitys, int count = -1)
        {
            if (entitys.CountIsOne())
            {
                NetworkBehaviour entity = entitys.First();
                if (entity is PlayerControllerB player)
                    return player.playerUsername;

                return entity.name;
            }

            if (count < 0)
                return $"{entitys.Count()} players";
            else
                return $"{count} players";
        }

        /// <summary>
        /// 명령어 입력 값에 해당하는 디스패처의 인틀리샌스 텍스트를 가져옵니다
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dispatcher">가져올 디스패처</param>
        /// <param name="input">입력 값</param>
        /// <param name="source">현재 소스</param>
        /// <param name="cursor">현재 입력 커서</param>
        /// <returns></returns>
        public static async Task<NetworkIntelliSenseArray> GetIntelliSenseText<TSource>(this CommandDispatcher<TSource> dispatcher, string input, TSource source, int cursor)
        {
            if (cursor < 0 || cursor > input.Length)
                return new NetworkIntelliSenseArray();

            StringReader stringReader = new StringReader(input);
            ParseResults<TSource> parseResults = dispatcher.Parse(stringReader, source);

            // 자동 완성
            {
                var completionResult = await dispatcher.GetCompletionSuggestions(parseResults, cursor);
                List<Suggestion> suggestions = completionResult.List;

                SuggestionContext<TSource> suggestionContext = parseResults.Context.FindSuggestionContext(cursor);
                if (suggestionContext.Parent == dispatcher.Root)
                {
                    suggestions.RemoveAll(suggestion =>
                    {
                        var childNode = suggestionContext.Parent.Children
                            .OfType<LiteralCommandNode<TSource>>()
                            .FirstOrDefault(c => c.Name == suggestion.Text);

                        return childNode != null && !childNode.CanUse(source);
                    });
                }

                if (suggestions.Count > 0)
                    return new NetworkIntelliSenseArray(NetworkIntelliSenseArray.Type.suggestion, [.. suggestions.Select(x => (NetworkIntelliSenseArray.Suggestion)x)]);
            }

            // 예외
            {
                var exceptions = parseResults.Exceptions.Values;
                if (exceptions.Count > 0)
                    return new NetworkIntelliSenseArray(NetworkIntelliSenseArray.Type.exception, [exceptions.Last().Message]);
            }

            // 설명
            {
                SuggestionContext<TSource> context = parseResults.Context.FindSuggestionContext(cursor);
                if (context.Parent.Children.Any(x => x is ArgumentCommandNode<TSource>))
                {
                    ICollection<string> usages = dispatcher.GetSmartUsage(context.Parent, source).Values;
                    if (usages.Count > 0)
                        return new NetworkIntelliSenseArray(NetworkIntelliSenseArray.Type.usage, [.. usages.Select(x => (NetworkIntelliSenseArray.Suggestion)x)]);
                }
            }

            return new NetworkIntelliSenseArray();
        }

        public static IEnumerable<PlayerControllerB> GetPlayers() => StartOfRound.Instance.ClientPlayerList.Values.Select(static x => StartOfRound.Instance.allPlayerScripts[x]);
        public static IEnumerable<NetworkBehaviour> GetEntitys() => GetPlayers().OfType<NetworkBehaviour>().Concat(UnityEngine.Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None)).Concat(UnityEngine.Object.FindObjectsByType<Anomaly>(FindObjectsSortMode.None)).Concat(UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None));
    }
}
