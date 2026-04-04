using Brigadier.NET;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Tree;
using GameNetcodeStuff;
using StaticNetcodeLib;
using System;
using Unity.Netcode;

namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>
    /// Classes that inherit this class are instantiated during the <see cref="NetworkManager.OnInstantiated"/> phase, and their <see cref="Register"/> method is automatically called.
    /// <br/><br/>
    /// 이 클래스를 상속한 클래스는 <see cref="NetworkManager.OnInstantiated"/> 단계에서 인스턴스가 생성되며, <see cref="Register"/> 메소드가 자동으로 호출됩니다.
    /// </summary>
    [StaticNetcode]
    public abstract class ServerCommand
    {
        /// <summary>
        /// Root of <see cref="dispatcher"/>
        /// <br/><br/>
        /// <see cref="dispatcher"/>의 루트
        /// </summary>
        public static RootCommandNode<ServerCommandSource> rootNode
        {
            get => dispatcher.Root;
            set => dispatcher = new CommandDispatcher<ServerCommandSource>(value);
        }

        /// <summary>
        /// Server-side dispatcher for this client
        /// <br/><br/>
        /// 이 클라이언트의 서버 측 디스패쳐
        /// </summary>
        public static CommandDispatcher<ServerCommandSource> dispatcher { get; private set; } = new CommandDispatcher<ServerCommandSource>();

        /// <summary>
        /// Methods automatically called during registration phase
        /// <br/><br/>
        /// 등록 단계에서 자동으로 호출되는 메소드
        /// </summary>
        public abstract void Register();

        /// <summary>
        /// Resets the client's server-side dispatcher.
        /// <br/><br/>
        /// 클라이언트의 서버 측 디스패처를 리셋합니다
        /// </summary>
        public static void Reset() => dispatcher = new CommandDispatcher<ServerCommandSource>();

        /// <summary>
        /// Register all commands with the client's server-side dispatcher.
        /// <br/><br/>
        /// 모든 커맨드를 클라이언트의 서버 측 디스패처에 등록합니다
        /// </summary>
        public static void AllRegister()
        {
            Debug.Log("Command Registering...");

            for (int i = 0; i < ReflectionManager.types.Count; i++)
            {
                Type type = ReflectionManager.types[i];
                if (type.IsAbstract || !typeof(ServerCommand).IsAssignableFrom(type))
                    continue;

                try
                {
                    ((ServerCommand)Activator.CreateInstance(type, true)).Register();
                    Debug.Log($"Registered command: {type.Name}");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            Debug.Log("Command Registered!");
        }

        /// <summary>
        /// Execute commands from client to server
        /// <br/><br/>
        /// 클라이언트에서 서버로 명령어를 실행합니다
        /// </summary>
        /// <param name="command">실행할 명령어</param>
        public static void ExecuteCommand(string command)
        {
            Debug.Log("Sending commands to the server side : " + command);
            InternalExecuteCommandServerRpc(command, GameNetworkManager.Instance.localPlayerController);
        }

        [ServerRpc]
        static void InternalExecuteCommandServerRpc(string command, NetworkBehaviourReference entityRef)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (!entityRef.TryGet(out var entity) || entity is not PlayerControllerB player)
                return;

            Debug.Log($"Received command from {player.playerUsername} : {command}");

            ServerCommandSource source = new ServerCommandSource(player);
            try
            {
                dispatcher.Execute(command, source);
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

        static Action<NetworkIntelliSenseArray>? requestedIntelliSense;

        /// <summary>
        /// 서버로 인탤리센스를 요청합니다.
        /// </summary>
        public static void RequestIntelliSense(string command, int cursor, Action<NetworkIntelliSenseArray> action)
        {
            Debug.LogD($"Requesting intelli sense to the server side : {cursor} : {command}");

            InternalRequestIntelliSenseServerRpc(command, cursor, GameNetworkManager.Instance.localPlayerController);
            requestedIntelliSense = action;
        }

        /// <summary>
        /// 인탤리센스 요청을 취소합니다.
        /// </summary>
        public static void CancelRequestIntelliSense() => requestedIntelliSense = null;

        [ServerRpc]
        static async void InternalRequestIntelliSenseServerRpc(string command, int cursor, NetworkBehaviourReference requester)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (!requester.TryGet(out PlayerControllerB player))
                return;

            Debug.LogD($"Received request intelli sense from {player.playerUsername} : {cursor} : {command}");

            NetworkIntelliSenseArray intelliSenseArray = await BFLCUtility.GetIntelliSenseText(dispatcher, command, new ServerCommandSource(player), cursor);
            Debug.LogD($"Sending intelli sense to {player.playerUsername} : {intelliSenseArray}");

            SendIntelliSenseClientRpc(intelliSenseArray, requester);
        }

        [ClientRpc]
        static void SendIntelliSenseClientRpc(NetworkIntelliSenseArray intelliSenseArray, NetworkBehaviourReference requester)
        {
            if (!requester.TryGet(out PlayerControllerB player) || player != GameNetworkManager.Instance.localPlayerController)
                return;

            Debug.LogD($"Received intelli sense from server side : {intelliSenseArray}");
            requestedIntelliSense?.Invoke(intelliSenseArray);
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public struct NetworkStringRanges(int start, int end) : INetworkSerializable
        {
            public int start = start;
            public int end = end;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref start);
                serializer.SerializeValue(ref end);
            }
        }
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    }
}
