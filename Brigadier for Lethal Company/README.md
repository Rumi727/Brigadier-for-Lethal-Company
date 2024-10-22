# Brigadier for Lethal Company

Language available in README: [[한국어 (대한민국)](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/README-KO.md)] [[**English (US)**]](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/README.md)

[Brigadier.NET]: https://github.com/AtomicBlom/Brigadier.NET

This is a mod that ports [Brigadier.NET] to Lethal Company.

## Explanation

It has really basic commands and execute commands built in.\
So you can use Vector3 arguments, and of course you can use relative coordinates like ~ ~ ~

You can also use entity selectors like Minecraft's @e[type=enemy] @s

### Selector Description

@a : All Players\
@e : All Entities (Player, Enemy, Item) (Please note that items in your inventory are also considered items.)\
@s : Oneself\
@p : Closest player other than yourself\
@n : Closest entity other than itself\
@r : One random player among all players
\
\
\
[type=] Entity Type (Can also be used as a negation by adding '!' in front)\

Example :\
``@e[type=!player,type=!item]`` (Can be used like ``@e[type=!player|!item]``)\
``@e[type=player|enemy]`` (Brings only players and enemies)

Entity Type : player, enemy, anomaly, item
\
\
\
[name=] Entity Name (Can also be used as a negation by adding '!' in front)\
Example : ``@e[name=Rumi727|jester|"S!p?e!c?i!a?l characters and spaces can be enclosed in \""]``\
(Retrieve only entities with names ``Rumi727`` or ``jester`` or ``S!p?e!c?i!a?l characters and spaces can be enclosed in "``)
\
\
\
[distance=] Distance\
Example : ``@e[type=!player,distance=..10]`` Gets all entities within a 10m radius, excluding players.
\
\
\
[limit=] Limit\
Example : ``@e[limit=1]`` Get the first entity among all entities
\
\
\
[death=false] Select only undead entities\
[death=true] Select only dead entities
\
\
\
[sort=near] Sort by closest\
[sort=far] Sort by furthest\
[sort=random] Random sort

## Add custom commands

If you inherit [ServerCommand](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/API/ServerCommand.cs)\
An instance is created at the [BFLCNetworkHandler.OnNetworkSpawn](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/Networking/BFLCNetworkHandler.cs) stage and the Register method is automatically called.

You can register the node to ServerCommand.dispatcher in the Register method.

The argument type can be accessed through the [RuniArguments](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/API/Arguments/RuniArguments.cs) class.

If you want to get the current command's position or rotation value,\
you can refer to the values ​​stored in [Command Source](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/API/ServerCommandSource.cs).

For detailed usage, please refer to the [Brigadier.NET] document.
Although it's Java, the [Fabric](https://fabricmc.net/wiki/tutorial:commands) documentation might also help...lol

The ThunderStore release also includes XML documentation, so you may find that helpful as well.

### Caution!

Due to limitations in my coding skills, mods that add server-side custom commands require the mod to be installed on the client as well for the added commands to appear in the list!