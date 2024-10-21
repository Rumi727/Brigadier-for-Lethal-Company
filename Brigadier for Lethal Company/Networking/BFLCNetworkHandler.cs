using Brigadier.NET.Exceptions;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.LCNetworks.API;
using System;
using Unity.Netcode;

namespace Rumi.BrigadierForLethalCompany.Networking
{
    /// <summary>
    /// It is responsible for the network in BFLC mode and has several APIs.
    /// <br/><br/>
    /// BFLC 모드의 네트워크를 담당하며, 몇가지 API가 마련되어있습니다
    /// </summary>
    public sealed class BFLCNetworkHandler : LCNetworkBehaviour<BFLCNetworkHandler>
    {
        /// <summary>
        /// The name of the handler.
        /// <br/><br/>
        /// 핸들러의 이름입니다.
        /// </summary>
        public override string name => "BFLC Network Handler";

        /// <summary>
        /// This is the hash value of this class, and it must be **unconditionally** the same across the global network.<br/>
        /// So, set it to a random constant so that it does not overlap.
        /// <br/><br/>
        /// 이 클래스의 해시 값이며, 글로벌 네트워크 전반에서 **무조건적으로** 동일해야 합니다.<br/>
        /// 따라서 겹치지 않도록 무작위 상수로 설정하세요.
        /// </summary>
        public override uint globalIdHash => 133444399;



        /// <summary>
        /// A method that is automatically called when the handler is spawned.<br/>
        /// If you don't know exactly what you are doing, don't call this method.
        /// <br/><br/>
        /// 핸들러가 스폰될 때 자동 호출되는 메소드입니다.<br/>
        /// 자신이 무엇을 하고 있는지 정확하게 모른다면, 이 메소드를 호출하지 마세요.
        /// </summary>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            ServerCommand.Reset();
            ServerCommand.AllRegister();

            /*if (IsServer)
                ServerCommand.AllRegister();
            else
                RequestServerCommandNode();*/
        }



        /*
        /// <summary>
        /// The client requests a command node from the server.
        /// <br/><br/>
        /// 클라이언트가 서버에게 명령어 노드를 요청합니다.
        /// </summary>
        public static void RequestServerCommandNode()
        {
            if (instance == null || instance.IsServer)
                return;

            Debug.Log("Sends a server command node request to the server");
            instance.InternalRequestServerCommandNodeServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        void InternalRequestServerCommandNodeServerRpc()
        {
            if (!IsServer)
                return;

            SendServerCommandNode();
        }



        /// <summary>
        /// Send command nodes from the server to the client
        /// <br/><br/>
        /// 서버에서 클라이언트로 명령어 노드를 보냅니다
        /// </summary>
        public static void SendServerCommandNode()
        {
            if (instance == null || !instance.IsServer)
                return;

            Debug.Log($"Sending server command node to client");

            try
            {
                var serializedRootNode = SerializationUtility.SerializeValue(ServerCommand.rootNode, DataFormat.Binary);
                Debug.Log(serializedRootNode.Length);
                instance.InternalSendServerCommandNodeClientRpc(serializedRootNode);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Serialization failed!");
            }
        }

        [ClientRpc]
        void InternalSendServerCommandNodeClientRpc(byte[] serializedRootNode)
        {
            if (IsServer)
                return;

            Debug.Log("Received a command node from the server");

            try
            {
                Debug.Log(serializedRootNode);
                Debug.Log(serializedRootNode.Length);
                ServerCommand.rootNode = SerializationUtility.DeserializeValue<RootCommandNode<ServerCommandSource>>(serializedRootNode, DataFormat.Binary);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Deserialization failed!");
            }
        }*/



        /// <summary>
        /// Execute commands from client to server
        /// <br/><br/>
        /// 클라이언트에서 서버로 명령어를 실행합니다
        /// </summary>
        /// <param name="command">실행할 명령어</param>
        public static void ExecuteCommand(string command)
        {
            if (instance == null)
                return;

            Debug.Log("Sending commands to the server side : " + command);
            instance.InternalExecuteCommandServerRpc(command, GameNetworkManager.Instance.localPlayerController.GetComponent<NetworkObject>());
        }

        [ServerRpc(RequireOwnership = false)]
        void InternalExecuteCommandServerRpc(string command, NetworkObjectReference networkObjectReference)
        {
            if (!IsServer)
                return;

            if (networkObjectReference.TryGet(out NetworkObject networkObject) && networkObject.TryGetComponent(out PlayerControllerB player))
            {
                Debug.Log($"Received command from {player.playerUsername} : {command}");

                ServerCommandSource source = new ServerCommandSource(player);
                try
                {
                    ServerCommand.dispatcher.Execute(command, source);
                }
                catch (CommandSyntaxException e)
                {
                    source.SendCommandResult(e);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }



        /// <summary>
        /// 서버 측에서 특정 클라이언트를 제외한 모든 클라이언트로 채팅을 전송합니다
        /// </summary>
        public static void AddGlobalChat(string text, PlayerControllerB? targetPlayer)
        {
            if (instance == null || !instance.IsServer)
                return;

            instance.InternalAddGlobalChatClientRpc(text, targetPlayer != null ? targetPlayer.GetComponent<NetworkObject>() : new NetworkObjectReference());
        }

        /// <summary>
        /// 서버 측에서 특정 클라이언트로 채팅을 전송합니다
        /// </summary>
        public static void AddChat(string text, PlayerControllerB? targetPlayer)
        {
            if (instance == null || !instance.IsServer || targetPlayer == null)
                return;

            NetworkObject? networkObject = targetPlayer.GetComponent<NetworkObject>();
            if (networkObject != null)
                instance.InternalAddChatClientRpc(text, networkObject);
        }

        [ClientRpc]
        void InternalAddGlobalChatClientRpc(string text, NetworkObjectReference networkObjectReference)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject) && networkObject.TryGetComponent(out PlayerControllerB player) && player == GameNetworkManager.Instance.localPlayerController)
                return;

            BFLCUtility.AddChatClient(text);
        }

        [ClientRpc]
        void InternalAddChatClientRpc(string text, NetworkObjectReference networkObjectReference)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject) && networkObject.TryGetComponent(out PlayerControllerB player) && player == GameNetworkManager.Instance.localPlayerController)
                BFLCUtility.AddChatClient(text);
        }
    }
}
