# Potato

Soon...

## NuGet Packages

```
dotnet add package MonoGame.Framework.DesktopGL --version 3.8.0.1641
dotnet add package MonoGame.Extended
dotnet add package MonoGame.Extended.Content.Pipeline
dotnet add package MonoGame.Extended.Input
```

## Rough Gameplay Summary 
This document shall contain the gameplay plan for the game Potato. In summary, the goal is to build a 2D platformer where the player controls the protagonist and has them traverse several levels. The main goal of the game is to have the protagonist reach and defeat the boss of each level, doing so starts the next level until the game completion state is reached. The time it takes to reach the game completion state is recorded, giving the player the secondary goal of minimizing the time it takes to have the protagonist reach the game completion state. Each level shall consist of several sections where the protagonist must defeat a minimum number of spawned enemies. Once the required number of enemies of a section is defeated, the player can choose to either continue fighting more enemies with the protagonist, or progress to the next section. If the player decides to continue fighting more enemies, the future spawns will progressively become stronger until either the gameover state is reached or the player chooses to move on to the next section. Upon entering a new section, the player is given a choice to spend experience to empower their character, or start the fight against the next wave of enemies. All choices by the player are selected by moving the protagonist to the appropriate location. Enemies and projectiles can inflict damage to the protagonist. The punishment to the protagonist when damaged by an enemy or projectile consists of immediately losing a heart, getting temporarily stunned, and getting pushed in the direction opposite to the direction of the enemy/projectile. However, the protagonist is given temporary invulnerability to subsequent attacks. If the protagonist’s last heart is removed, then the game over state is reached and the protagonist must start the current level at its beginning. There is no other punishment. The protagonist and projectiles can inflict damage to enemies. The same effects that can apply to the protagonist are also applied to enemies when they take damage. As for the narrative, the protagonist is a farmer who one day witnessed a racoon steal a potato grown by the protagonist. The protagonist sees this and decides to chase down the critter, starting off the events described in this game’s rough summary.

## Rough User Interface Structure Summary
The user interface will consist of a mixture of menus and a hud. A menu can consist of a block of text, selectable options, sliders, and/or a typing-field. A special case of a menu is a dialog box, where only a block of text is present. The hud will consist of the protagonist’s hearts, spendable experience, and total time in game.

## Changeable Options
- Music Volume
- Sound Effects Volume
- Master Volume
- Key Bindings 
- Reset to Defaults
- Fullscreen Toggle
- Preferred Controller 

## Rough Protagonist Control Summary

## Game States
- Semi-Paused
- Full-Paused
- Game Over
- Game Completion

## Game Rooms
- Level 
- Title 
- Segue 
- Game Over 
- Game Completion 
- Debug Level Editor 


