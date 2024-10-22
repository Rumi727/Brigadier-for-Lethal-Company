using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.Networking;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>
    /// The source to use when executing the command.
    /// <br/><br/>
    /// 커맨드를 실행할 때 사용할 소스입니다
    /// </summary>
    public class ServerCommandSource : ICloneable
    {
        /// <summary>현재 이름</summary>
        public string name { get; private set; }

        /// <summary>실행한 개체</summary>
        public NetworkBehaviour? sender { get; private set; } = null;
        /// <summary>실행한 플레이어</summary>
        public PlayerControllerB? player { get; private set; } = null;

        /// <summary>처음 실행한 플레이어</summary>
        public PlayerControllerB? firstPlayer { get; }

        /// <summary>실행한 위치</summary>
        public Vector3 position { get; private set; } = Vector3.zero;

        /// <summary>현재 회전값</summary>
        public Vector3 rotation { get; private set; } = Vector3.zero;

        /// <summary>현재 상태를 문자열로 변환합니다</summary>
        public override string ToString() => $"{nameof(name)}:{name}, {nameof(sender)}:{sender}, {nameof(player)}:{player}, {nameof(position)}:{position}, {nameof(rotation)}:{rotation}";

        /// <summary>실행한 개체를 지정합니다</summary>
        public ServerCommandSource SetSender(NetworkBehaviour sender)
        {
            name = sender.name;

            this.sender = sender;
            player = sender as PlayerControllerB;

            return this;
        }

        /// <summary>실행한 위치를 지정합니다</summary>
        public ServerCommandSource SetPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        /// <summary>현재 회전 값을 지정합니다</summary>
        public ServerCommandSource SetRotation(Vector3 rotation)
        {
            this.rotation = rotation;
            return this;
        }

        /// <summary>커맨드 결과를 채팅에 전송합니다</summary>
        public void SendCommandResult(string text, bool sendGlobal = true)
        {
            if (sender == null)
                return;

            BFLCNetworkHandler.AddChat($"<color=white><size=10>{text}</size></color>", player);

            if (sendGlobal)
                BFLCNetworkHandler.AddGlobalChat($"<color=#aaaaaa><size=10><i>[{sender.GetEntityName()}] {text}</i></size></color>", player);
        }

        /// <summary>커맨드 결과를 채팅에 전송합니다</summary>
        public void SendCommandResult(Exception e)
        {
            if (firstPlayer == null)
                return;

            BFLCNetworkHandler.AddChat($"<color=#ff5555><size=10>{e.Message}</size></color>", firstPlayer);
        }

        /// <summary>
        /// 커맨드 소스를 복제합니다
        /// </summary>
        public ServerCommandSource Clone() => new ServerCommandSource(this);
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// 커맨드 소스를 생성합니다
        /// </summary>
        /// <param name="name">개체 이름</param>
        public ServerCommandSource(string name = "IL") : this(name, Vector3.zero, Vector3.zero) { }

        /// <summary>
        /// 커맨드 소스를 생성합니다
        /// </summary>
        /// <param name="position">실행한 좌표</param>
        /// <param name="rotation">회전 값</param>
        public ServerCommandSource(Vector3 position, Vector3 rotation) : this("IL", position, rotation) { }

        /// <summary>
        /// 커맨드 소스를 생성합니다
        /// </summary>
        /// <param name="entity">실행한 엔티티</param>
        public ServerCommandSource(NetworkBehaviour entity) : this(entity, entity.transform.position, entity.transform.eulerAngles) => sender = entity;

        /// <summary>
        /// 커맨드 소스를 생성합니다
        /// </summary>
        /// <param name="entity">실행한 엔티티</param>
        /// <param name="position">실행한 좌표</param>
        /// <param name="rotation">회전 값</param>
        public ServerCommandSource(NetworkBehaviour entity, Vector3 position, Vector3 rotation) : this(entity.name, position, rotation)
        {
            sender = entity;
            player = sender as PlayerControllerB;
            firstPlayer = player;
        }

        /// <summary>
        /// 커맨드 소스를 생성합니다
        /// </summary>
        /// <param name="name">개체 이름</param>
        /// <param name="position">실행한 좌표</param>
        /// <param name="rotation">회전 값</param>
        public ServerCommandSource(string name, Vector3 position, Vector3 rotation)
        {
            this.name = name;

            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 커맨드 소스를 복제합니다
        /// </summary>
        /// <param name="source">복제할 커맨드 소스</param>
        public ServerCommandSource(ServerCommandSource source)
        {
            name = source.name;

            sender = source.sender;
            player = source.player;

            firstPlayer = player;

            position = source.position;
            rotation = source.rotation;
        }
    }
}
