#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.Netcode;

namespace Rumi.BrigadierForLethalCompany
{
    public static class RPCParamsUtility
    {
        public static bool TryGetPlayer(this ServerRpcParams rpcParams, [NotNullWhen(true)] out PlayerControllerB? player)
        {
            player = BFLCUtility.GetPlayers().FirstOrDefault(x => x.OwnerClientId == rpcParams.Receive.SenderClientId);
            return player != null;
        }

        public static ClientRpcParams ToRpc(this PlayerControllerB? player)
        {
            if (player == null)
                return default;

            return new ClientRpcParams() { Send = new() { TargetClientIds = [player.OwnerClientId] } };
        }

        public static ClientRpcParams ToRpc(this IEnumerable<PlayerControllerB?> players) => new ClientRpcParams() 
        { 
            Send = new() 
            { 
                TargetClientIds = [.. players.WhereNotNull().Select(x => x.OwnerClientId)] 
            } 
        };

        public static ClientRpcParams PlayerToRpc(params IEnumerable<PlayerControllerB?> players) => new ClientRpcParams()
        {
            Send = new()
            {
                TargetClientIds = [.. players.WhereNotNull().Select(x => x.OwnerClientId)]
            }
        };
    }
}
