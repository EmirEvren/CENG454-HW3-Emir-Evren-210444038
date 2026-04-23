# ColorGuardian

A 3D defend-the-objective action prototype developed for CENG 454.

## Overview
The player protects a central treasure chest from waves of color-coded enemies.

Each enemy can only be damaged by bullets of the matching color:
- Red
- Blue
- Green
- Yellow

The player must gather ammo before each stage, switch between colors, and defend the chest through all stages.

## Core Gameplay Loop
- Collect colored ammo during the pre-stage loot phase
- Defend the treasure chest from incoming enemies
- Use the correct ammo color against matching enemies
- Survive all stages to win
- Lose if the chest health reaches zero

## Technical Goals
This project is designed around:
- Observer
- Strategy
- Object Pool
- Decorator
- Interface-based architecture

## Author
Emir Evren - 210444038
CENG 454 - Game Programming
Spring 2026