using Brigadier.NET;
using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class SummonCommand : ServerCommand
    {
        public const string resultText = "Summoned new {name}";

        SummonCommand() { }

        public override void Register()
        {
            //summon enemy <EnemyType:entity>
            //summon enemy <EnemyType:entity> <Vector3:location>
            //summon anomaly <AnomalyType:entity>
            //summon anomaly <AnomalyType:entity> <Vector3:location>
            //summon item <Item:item>
            //summon item <Item:item> <int:price>
            //summon item <Item:item> <Vector3:location>
            //summon item <Item:item> <Vector3:location> <int:price>
            dispatcher.Register(x =>
                x.Literal("summon")
                    .Then(x =>
                        x.Literal("enemy")
                            .Then(x =>
                                x.Argument("entity", LethalArguments.EnemyType())
                                    .Executes(x =>
                                    {
                                        EnemyType entity = LethalArguments.GetEnemyType(x, "entity");
                                        SummonEntity(entity, x.Source.position);

                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.enemyName));

                                        return 1;
                                    })
                                    .Then(x =>
                                        x.Argument("location", LethalArguments.Vector3())
                                            .Executes(x =>
                                            {
                                                EnemyType entity = LethalArguments.GetEnemyType(x, "entity");
                                                Vector3 position = LethalArguments.GetVector3(x, "location");

                                                SummonEntity(entity, position);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.enemyName));

                                                return 1;
                                            })
                                    )
                            )
                    )
                    .Then(x =>
                        x.Literal("anomaly")
                            .Then(x =>
                                x.Argument("entity", LethalArguments.AnomalyType())
                                    .Executes(x =>
                                    {
                                        AnomalyType entity = LethalArguments.GetAnomalyType(x, "entity");
                                        SummonEntity(entity, x.Source.position);

                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.anomalyName));

                                        return 1;
                                    })
                                    .Then(x =>
                                        x.Argument("location", LethalArguments.Vector3())
                                            .Executes(x =>
                                            {
                                                AnomalyType entity = LethalArguments.GetAnomalyType(x, "entity");
                                                Vector3 position = LethalArguments.GetVector3(x, "location");

                                                SummonEntity(entity, position);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.anomalyName));

                                                return 1;
                                            })
                                    )
                            )
                    )
                    .Then(x =>
                        x.Literal("item")
                            .Then(x =>
                                x.Argument("item", LethalArguments.ItemType())
                                    .Executes(x =>
                                    {
                                        Item entity = LethalArguments.GetItemType(x, "item");
                                        SummonEntity(entity, x.Source.position);

                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                        return 1;
                                    })
                                    .Then(
                                        x.Argument("price", Arguments.Integer())
                                            .Executes(x =>
                                            {
                                                Item entity = LethalArguments.GetItemType(x, "item");
                                                int price = Arguments.GetInteger(x, "price");

                                                SummonEntity(entity, x.Source.position, price);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                                return 1;
                                            })
                                    )
                                    .Then(x =>
                                        x.Argument("location", LethalArguments.Vector3())
                                            .Executes(x =>
                                            {
                                                Item entity = LethalArguments.GetItemType(x, "item");
                                                Vector3 position = LethalArguments.GetVector3(x, "location");

                                                SummonEntity(entity, position);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                                return 1;
                                            })
                                            .Then(
                                                x.Argument("price", Arguments.Integer())
                                                    .Executes(x =>
                                                    {
                                                        Item entity = LethalArguments.GetItemType(x, "item");
                                                        Vector3 position = LethalArguments.GetVector3(x, "location");
                                                        int price = Arguments.GetInteger(x, "price");

                                                        SummonEntity(entity, position, price);
                                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                                        return 1;
                                                    })
                                            )
                                    )
                            )
                    )
            );
        }

        public static void SummonEntity(EnemyType entityType, Vector3 position)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (!RoundManager.Instance.SpawnEnemyGameObject(position, 0, 0, entityType).TryGet(out var entity))
                return;

            entityType.numberSpawned++;
            entityType.hasSpawnedAtLeastOne = true;

            bool isOutside = entity.transform.position.y >= -100;

            EnemyAI enemyAI = entity.GetComponent<EnemyAI>();
            enemyAI.StartCoroutine(SummonEntityCoroutine(enemyAI, isOutside));

            InternalSummonEnemyClientRpc(enemyAI, isOutside);
        }

        [ClientRpc]
        static void InternalSummonEnemyClientRpc(NetworkBehaviourReference entityRef, bool isOutside)
        {
            if (!entityRef.TryGet(out EnemyAI enemyAI))
                return;

            enemyAI.StartCoroutine(SummonEntityCoroutine(enemyAI, isOutside));
        }

        static IEnumerator SummonEntityCoroutine(EnemyAI enemyAI, bool isOutside)
        {
            yield return null;
            enemyAI.SetEnemyOutside(isOutside);
        }

        public static void SummonEntity(AnomalyType entityType, Vector3 position)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            GameObject gameObject = Object.Instantiate(entityType.anomalyPrefab, position, Quaternion.identity);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            Anomaly anomaly = gameObject.GetComponent<Anomaly>();
            RoundManager.Instance.SpawnedAnomalies.Add(anomaly);
        }

        public static void SummonEntity(Item entityType, Vector3 position, int price = 0)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            GameObject gameObject = Object.Instantiate(entityType.spawnPrefab, position, Quaternion.Euler(entityType.restingRotation));

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn(true);

            InternalSummonItemClientRpc(networkObject, price);
        }

        [ClientRpc]
        static void InternalSummonItemClientRpc(NetworkObjectReference entityRef, int price)
        {
            if (!entityRef.TryGet(out var entity))
                return;

            GrabbableObject grabbableObject = entity.gameObject.GetComponent<GrabbableObject>();
            if (grabbableObject.GetComponentInChildren<ScanNodeProperties>() != null)
                grabbableObject.SetScrapValue(price);
        }
    }
}
