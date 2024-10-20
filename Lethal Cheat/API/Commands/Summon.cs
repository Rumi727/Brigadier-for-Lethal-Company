using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Summon : ServerCommand
    {
        Summon() { }

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
                                x.Argument("entity", RuniArguments.EnemyType())
                                    .Executes(x =>
                                    {
                                        EnemyType entity = RuniArguments.GetEnemyType(x, "entity");
                                        LCheatNetworkHandler.SummonEntity(entity, x.Source.position);

                                        return 1;
                                    })
                                    .Then(x =>
                                        x.Argument("location", RuniArguments.Vector3())
                                            .Executes(x =>
                                            {
                                                EnemyType entity = RuniArguments.GetEnemyType(x, "entity");
                                                Vector3 position = RuniArguments.GetVector3(x, "location");

                                                LCheatNetworkHandler.SummonEntity(entity, position);

                                                return 1;
                                            })
                                    )
                            )
                    )
                    .Then(x =>
                        x.Literal("anomaly")
                            .Then(x =>
                                x.Argument("entity", RuniArguments.AnomalyType())
                                    .Executes(x =>
                                    {
                                        AnomalyType entity = RuniArguments.GetAnomalyType(x, "entity");
                                        LCheatNetworkHandler.SummonEntity(entity, x.Source.position);

                                        return 1;
                                    })
                                    .Then(x =>
                                        x.Argument("location", RuniArguments.Vector3())
                                            .Executes(x =>
                                            {
                                                AnomalyType entity = RuniArguments.GetAnomalyType(x, "entity");
                                                Vector3 position = RuniArguments.GetVector3(x, "location");

                                                LCheatNetworkHandler.SummonEntity(entity, position);
                                                return 1;
                                            })
                                    )
                            )
                    )
                    .Then(x =>
                        x.Literal("item")
                            .Then(x =>
                                x.Argument("item", RuniArguments.ItemType())
                                    .Executes(x =>
                                    {
                                        Item entity = RuniArguments.GetItemType(x, "item");
                                        LCheatNetworkHandler.SummonEntity(entity, x.Source.position);

                                        return 1;
                                    })
                                    .Then(
                                        x.Argument("price", RuniArguments.Integer())
                                            .Executes(x =>
                                            {
                                                Item entity = RuniArguments.GetItemType(x, "item");
                                                int price = RuniArguments.GetInteger(x, "price");

                                                LCheatNetworkHandler.SummonEntity(entity, x.Source.position, price);
                                                return 1;
                                            })
                                    )
                                    .Then(x =>
                                        x.Argument("location", RuniArguments.Vector3())
                                            .Executes(x =>
                                            {
                                                Item entity = RuniArguments.GetItemType(x, "item");
                                                Vector3 position = RuniArguments.GetVector3(x, "location");

                                                LCheatNetworkHandler.SummonEntity(entity, position);
                                                return 1;
                                            })
                                            .Then(
                                                x.Argument("price", RuniArguments.Integer())
                                                    .Executes(x =>
                                                    {
                                                        Item entity = RuniArguments.GetItemType(x, "item");
                                                        Vector3 position = RuniArguments.GetVector3(x, "location");
                                                        int price = RuniArguments.GetInteger(x, "price");

                                                        LCheatNetworkHandler.SummonEntity(entity, position, price);
                                                        return 1;
                                                    })
                                            )
                                    )
                            )
                    )
            );
        }
    }
}
