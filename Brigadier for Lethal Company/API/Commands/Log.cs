using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands
{
    public sealed class Log : ServerCommand
    {
        Log() { }

        public override void Register()
        {
            //log <string:message>
            //log source
            //log types
            //log entitys
            //log anomalys
            //log items
            dispatcher.Register(x =>
                x.Literal("log")
                    .Then(x =>
                        x.Argument("message", RuniArguments.String())
                            .Executes(x =>
                            {
                                string text = RuniArguments.GetString(x, "message");
                                Debug.Log(text);
                                return text.Length;
                            })
                    )
                    .Then(x =>
                        x.Literal("source")
                            .Executes(x =>
                            {
                                string text = x.Source.ToString();
                                Debug.Log(text);
                                return text.Length;
                            })
                    )
                    .Then(x =>
                        x.Literal("types")
                            .Executes(x =>
                            {
                                Debug.Log(SelectorOptionType.playerType);
                                Debug.Log(SelectorOptionType.enemyType);
                                Debug.Log(SelectorOptionType.itemType);

                                return 4;
                            })
                    )
                    .Then(x =>
                        x.Literal("entitys")
                            .Executes(x =>
                            {
                                int count = 0;
                                foreach ((string enemyName, _) in EnemyTypeArgumentType.GetEnemyTypes())
                                {
                                    Debug.Log(enemyName);
                                    count++;
                                }

                                return count;
                            })
                    )
                    .Then(x =>
                        x.Literal("anomalys")
                            .Executes(x =>
                            {
                                int count = 0;
                                foreach ((string enemyName, _) in AnomalyTypeArgumentType.GetAnomalyTypes())
                                {
                                    Debug.Log(enemyName);
                                    count++;
                                }

                                return count;
                            })
                    )
                    .Then(x =>
                        x.Literal("items")
                            .Executes(x =>
                            {
                                int count = 0;
                                foreach ((string itemName, _) in ItemTypeArgumentType.GetItemTypes())
                                {
                                    Debug.Log(itemName);
                                    count++;
                                }

                                return count;
                            })
                    )
            );
        }
    }
}