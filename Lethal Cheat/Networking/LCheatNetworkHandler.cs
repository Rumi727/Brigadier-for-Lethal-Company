using GameNetcodeStuff;
using Rumi.LCNetworks.API;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.Networking
{
    /// <summary>
    /// LCheat 모드의 네트워크를 담당하며, 몇가지 API가 마련되어있습니다
    /// </summary>
    public sealed class LCheatNetworkHandler : LCNHNetworkBehaviour<LCheatNetworkHandler>
    {
        public override string name => "LCheat Network Handler";
        public override uint globalIdHash => 591605829;



        #region Teleport
        /// <summary>
        /// 선택한 엔티티를 특정 좌표로 텔레포트합니다
        /// </summary>
        /// <param name="entity">텔레포트할 <see cref="NetworkBehaviour"/> 오브젝트</param>
        /// <param name="position">텔레포트할 좌표</param>
        public static void TeleportEntity(NetworkBehaviour entity, Vector3 position)
        {
            if (instance == null || !instance.IsServer)
                return;

            if (entity is EnemyAI enemy)
                enemy.agent.Warp(position);

            entity.transform.position = position;
            instance.InternalTeleportEntityClientRpc(entity, position);
        }

        [ClientRpc]
        void InternalTeleportEntityClientRpc(NetworkBehaviourReference entityRef, Vector3 position)
        {
            if (entityRef.TryGet(out var entity))
                entity.transform.position = position;
        }
        #endregion



        #region Kill Entity
        /// <summary>
        /// 선택한 엔티티를 죽입니다 (죽일 수 없을 경우, 파괴합니다)
        /// </summary>
        /// <param name="entity">죽일 <see cref="NetworkBehaviour"/> 오브젝트</param>
        public static void KillEntity(NetworkBehaviour entity)
        {
            if (instance == null || !instance.IsServer)
                return;

            if (entity is PlayerControllerB or EnemyAI)
                instance.InternalKillEntityClientRpc(entity);
        }

        [ClientRpc]
        void InternalKillEntityClientRpc(NetworkBehaviourReference entityRef)
        {
            if (!entityRef.TryGet(out var entity))
                return;
            
            if (entity is PlayerControllerB player)
                player.KillPlayer(Vector3.up * 5);
            else if (entity is EnemyAI enemy)
                enemy.KillEnemy();
        }
        #endregion



        #region Destroy Entity
        /// <summary>
        /// 선택한 엔티티를 파괴합니다 (플레이어는 파괴할 수 없습니다)
        /// </summary>
        /// <param name="entity">파괴할 <see cref="NetworkObject"/> 스크립트가 붙어 있는 <see cref="NetworkBehaviour"/> 오브젝트</param>
        public static void DestroyEntity(NetworkBehaviour entity)
        {
            if (instance == null || !instance.IsServer)
                return;

            if (entity is not PlayerControllerB && entity.TryGetComponent(out NetworkObject networkObject))
                networkObject.Despawn();
        }
        #endregion



        #region Damage Entity
        /// <summary>
        /// 선택한 엔티티에 데미지를 줍니다
        /// </summary>
        /// <param name="entity">데미지를 줄 <see cref="NetworkBehaviour"/> 오브젝트</param>
        public static void DamageEntity(NetworkBehaviour entity, int damage)
        {
            if (instance == null || !instance.IsServer)
                return;

            if (entity is PlayerControllerB or EnemyAI)
                instance.InternalDamageEntityClientRpc(entity, damage);
        }

        [ClientRpc]
        void InternalDamageEntityClientRpc(NetworkBehaviourReference entityRef, int damage)
        {
            if (!entityRef.TryGet(out var entity))
                return;

            if (entity is PlayerControllerB player)
                player.DamagePlayer(damage);
            else if (entity is EnemyAI enemy)
                enemy.HitEnemy(damage, null, true);
        }
        #endregion



        #region Summon Entity
        public static void SummonEntity(EnemyType entity, Vector3 position)
        {
            if (instance == null || !instance.IsServer)
                return;

            GameObject gameObject = Instantiate(entity.enemyPrefab, position, Quaternion.identity);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            EnemyAI enemyAI = gameObject.GetComponent<EnemyAI>();
            RoundManager.Instance.SpawnedEnemies.Add(enemyAI);
        }

        public static void SummonEntity(AnomalyType entity, Vector3 position)
        {
            if (instance == null || !instance.IsServer)
                return;

            GameObject gameObject = Instantiate(entity.anomalyPrefab, position, Quaternion.identity);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            Anomaly anomaly = gameObject.GetComponent<Anomaly>();
            RoundManager.Instance.SpawnedAnomalies.Add(anomaly);
        }

        public static void SummonEntity(Item entity, Vector3 position, int price = 0)
        {
            if (instance == null || !instance.IsServer)
                return;

            GameObject gameObject = Instantiate(entity.spawnPrefab, position, Quaternion.Euler(entity.restingRotation));

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn(true);

            instance.InternalSummonEntityClientRpc(networkObject, price);
        }

        [ClientRpc]
        void InternalSummonEntityClientRpc(NetworkObjectReference entityRef, int price)
        {
            if (!entityRef.TryGet(out var entity))
                return;

            GrabbableObject grabbableObject = entity.gameObject.GetComponent<GrabbableObject>();
            if (grabbableObject.GetComponentInChildren<ScanNodeProperties>() != null)
                grabbableObject.SetScrapValue(price);
        }
        #endregion



        #region Time
        public static float GetTimeHour()
        {
            if (instance == null || !instance.IsServer)
                return 0;

            var timeInstance = TimeOfDay.Instance;
            var level = timeInstance.currentLevel;

            return (timeInstance.globalTime * level.DaySpeedMultiplier / timeInstance.lengthOfHours) + 6;
        }

        public static void SetTimeHour(float time)
        {
            if (instance == null || !instance.IsServer)
                return;

            instance.InternalSetTimeHourClientRpc(time);
        }

        [ClientRpc]
        void InternalSetTimeHourClientRpc(float time)
        {
            var timeInstance = TimeOfDay.Instance;
            var level = timeInstance.currentLevel;

            float globalTime = (time - 6) / level.DaySpeedMultiplier * timeInstance.lengthOfHours;

            timeInstance.globalTime = globalTime;
            timeInstance.currentDayTime = timeInstance.CalculatePlanetTime(level);
            timeInstance.hour = (int)(timeInstance.currentDayTime / timeInstance.lengthOfHours);
            timeInstance.previousHour = timeInstance.hour;
            timeInstance.globalTimeAtEndOfDay = globalTime + (timeInstance.totalTime - timeInstance.currentDayTime) / level.DaySpeedMultiplier;
            timeInstance.normalizedTimeOfDay = timeInstance.currentDayTime / timeInstance.totalTime;

            timeInstance.RefreshClockUI();
        }
        #endregion



        #region Time Speed
        public static float GetTimeSpeed() => TimeOfDay.Instance.globalTimeSpeedMultiplier;

        public static void SetTimeSpeed(float speed)
        {
            if (instance == null || !instance.IsServer)
                return;

            instance.InternalSetTimeSpeedClientRpc(speed);
        }

        [ClientRpc]
        void InternalSetTimeSpeedClientRpc(float speed) => TimeOfDay.Instance.globalTimeSpeedMultiplier = speed;
        #endregion



        #region Credit
        public static int GetCredit()
        {
            if (instance == null || !instance.IsServer)
                return 0;

            return FindAnyObjectByType<Terminal>().groupCredits;
        }

        public static void SetCredit(int credit)
        {
            if (instance == null || !instance.IsServer)
                return;

            Terminal terminal = FindAnyObjectByType<Terminal>();

            terminal.useCreditsCooldown = true;
            terminal.groupCredits = credit;
            terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
        }
        #endregion
    }
}
