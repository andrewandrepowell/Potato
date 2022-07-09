# Potato

Soon...

## NuGet Packages

```
dotnet add package MonoGame.Framework.DesktopGL --version 3.8.0.1641
dotnet add package MonoGame.Extended
```

## Rough Summary

This document shall contain the plan for the game Potato. In summary, the goal is to build a 2D platformer where the player controls the protagonist and has them traverse several levels. The main goal of the game is to have the protagonist reach and defeat the boss of each level, doing so starts the next level until the game completion state is reached. The time it takes to reach the game completion state is recorded, giving the player the secondary goal of minimizing the time it takes to have the protagonist reach the game completion state. Each level shall consist of several rooms where the protagonist must defeat a minimum number of spawned enemies. Once the required number of enemies of a room is defeated, the player can choose to either continue fighting more enemies with the protagonist, or progress to the next room. If the player decides to continue fighting more enemies, the future spawns will progressively become stronger until either the gameover state is reached or the player chooses to move on to the next room. Upon entering a new room, the player is given a choice to spend a currency to empower their character, or start the fight against the next wave of enemies. All choices by the player are selected by moving the protagonist to the appropriate location. Enemies and projectiles can inflict damage to the protagonist. The punishment to the protagonist when damaged by an enemy or projectile consists of immediately losing a heart, getting temporarily stunned, and getting pushed in the direction opposite to the direction of the enemy/projectile. However, the protagonist is given temporary invulnerability to subsequent attacks. If the protagonist’s last heart is removed, then the game over state is reached and the protagonist must start the current level at its beginning. There is no other punishment. As for the narrative, the protagonist is a farmer who one day witnessed a racoon steal a potato grown by the protagonist. The protagonist sees this and decides to chase down the critter, starting off the events described in this game’s rough summary.

