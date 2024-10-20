using Brigadier.NET;
using Brigadier.NET.Tree;
using System;

namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>
    /// Classes that inherit this class are instantiated during the <see cref="Networking.BFLCNetworkHandler.OnNetworkSpawn"/> phase, and their <see cref="Register"/> method is automatically called.
    /// <br/><br/>
    /// 이 클래스를 상속한 클래스는 <see cref="Networking.BFLCNetworkHandler.OnNetworkSpawn"/> 단계에서 인스턴스가 생성되며, <see cref="Register"/> 메소드가 자동으로 호출됩니다.
    /// </summary>
    public abstract class ServerCommand
    {
        /// <summary>
        /// Root of <see cref="dispatcher"/>
        /// <br/><br/>
        /// <see cref="dispatcher"/>의 루트
        /// </summary>
        public static RootCommandNode<ServerCommandSource> chatRoot => dispatcher.GetRoot();

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
                    ((ServerCommand?)Activator.CreateInstance(type, true))?.Register();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            Debug.Log("Command Registered!");
        }
    }
}
