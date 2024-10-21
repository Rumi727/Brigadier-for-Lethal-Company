using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Summon : ServerCommand
    {
        public const string resultText = "Summoned new {name}";

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
            dispatcher.Register(static x =>
                x.Literal("summon")
                    .Then(static x =>
                        x.Literal("enemy")
                            .Then(static x =>
                                x.Argument("entity", RuniArguments.EnemyType())
                                    .Executes(static x =>
                                    {
                                        EnemyType entity = RuniArguments.GetEnemyType(x, "entity");
                                        LCheatNetworkHandler.SummonEntity(entity, x.Source.position);

                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.enemyName));

                                        return 1;
                                    })
                                    .Then(static x =>
                                        x.Argument("location", RuniArguments.Vector3())
                                            .Executes(static x =>
                                            {
                                                EnemyType entity = RuniArguments.GetEnemyType(x, "entity");
                                                Vector3 position = RuniArguments.GetVector3(x, "location");

                                                LCheatNetworkHandler.SummonEntity(entity, position);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.enemyName));

                                                return 1;
                                            })
                                    )
                            )
                    )
                    .Then(static x =>
                        x.Literal("anomaly")
                            .Then(static x =>
                                x.Argument("entity", RuniArguments.AnomalyType())
                                    .Executes(static x =>
                                    {
                                        AnomalyType entity = RuniArguments.GetAnomalyType(x, "entity");
                                        LCheatNetworkHandler.SummonEntity(entity, x.Source.position);

                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.anomalyName));

                                        return 1;
                                    })
                                    .Then(static x =>
                                        x.Argument("location", RuniArguments.Vector3())
                                            .Executes(static x =>
                                            {
                                                AnomalyType entity = RuniArguments.GetAnomalyType(x, "entity");
                                                Vector3 position = RuniArguments.GetVector3(x, "location");

                                                LCheatNetworkHandler.SummonEntity(entity, position);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.anomalyName));

                                                return 1;
                                            })
                                    )
                            )
                    )
                    .Then(static x =>
                        x.Literal("item")
                            .Then(static x =>
                                x.Argument("item", RuniArguments.ItemType())
                                    .Executes(static x =>
                                    {
                                        Item entity = RuniArguments.GetItemType(x, "item");
                                        LCheatNetworkHandler.SummonEntity(entity, x.Source.position);

                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                        return 1;
                                    })
                                    .Then(
                                        x.Argument("price", RuniArguments.Integer())
                                            .Executes(static x =>
                                            {
                                                Item entity = RuniArguments.GetItemType(x, "item");
                                                int price = RuniArguments.GetInteger(x, "price");

                                                LCheatNetworkHandler.SummonEntity(entity, x.Source.position, price);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                                return 1;
                                            })
                                    )
                                    .Then(static x =>
                                        x.Argument("location", RuniArguments.Vector3())
                                            .Executes(static x =>
                                            {
                                                Item entity = RuniArguments.GetItemType(x, "item");
                                                Vector3 position = RuniArguments.GetVector3(x, "location");

                                                LCheatNetworkHandler.SummonEntity(entity, position);
                                                x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

                                                return 1;
                                            })
                                            .Then(
                                                x.Argument("price", RuniArguments.Integer())
                                                    .Executes(static x =>
                                                    {
                                                        Item entity = RuniArguments.GetItemType(x, "item");
                                                        Vector3 position = RuniArguments.GetVector3(x, "location");
                                                        int price = RuniArguments.GetInteger(x, "price");

                                                        LCheatNetworkHandler.SummonEntity(entity, position, price);
                                                        x.Source.SendCommandResult(resultText.Replace("{name}", entity.itemName));

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
