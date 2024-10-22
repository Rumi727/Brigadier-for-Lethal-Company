#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace Rumi.BrigadierForLethalCompany
{
    public static class BFLCUtility
    {
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
    }
}
