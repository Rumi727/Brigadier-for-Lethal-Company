# Lethal Cheat

This is a lethal cheat mode.\
You can access it by typing / in the chat.

Unlike most cheat modes, if it's installed on the server, you can use it even if you're not the room manager.\
In other words, if it's not installed on the server, you can't use it.

I intended to use this mod for debugging and playing with friends.

There are more commands to add, but I'm not sure when I'll add them.

## List of commands
```
damage <int:amount>
damage <Entity:destination> <int:amount>

destroy
destroy <Entity:destination>

kill
kill <Entity:targets>

summon enemy <EnemyType:entity>
summon enemy <EnemyType:entity> <Vector3:location>
summon anomaly <AnomalyType:entity>
summon anomaly <AnomalyType:entity> <Vector3:location>
summon item <Item:item>
summon item <Item:item> <int:price>
summon item <Item:item> <Vector3:location>
summon item <Item:item> <Vector3:location> <int:price>

tp -> teleport

teleport <Vector3:location>
teleport <Entity:destination>
teleport <Entity[]:targets> <Vector3:location>
teleport <Entity[]:targets> <Entity:destination>

time set <float:time>
time speed <float:speed>
```