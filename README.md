# CENG454 HW3 - Core Breach: Pattern-Driven Systems Prototype

## Student Information

**Full Name:** Emir Evren  
**Student ID:** 210444038  
**Course:** CENG 454 - Game Programming  
**Project Name:** ColorGuardian / Core Breach Prototype  

## Repository

This repository contains the Unity project for CENG454 HW3.

## Project Summary

This project is a short single-player defend-the-core prototype. The player protects a central chest/core from enemy waves, collects color-coded ammunition during loot phases, and uses matching bullet colors to damage enemies.

The gameplay loop includes:

- Loot phase
- Combat phase
- Enemy waves
- Player and chest health
- Win condition after all stages are cleared
- Lose condition when the player dies or the chest is destroyed

## Required Patterns Used

### Observer

Used for health, ammo, and stage events.

Main examples:

- `StageManager`
- `WinHandler`
- `GameLoseHandler`
- `ChestHealth`
- `PlayerHealth`
- `AmmoInventory`
- UI scripts

### Strategy

Used for enemy target selection.

Main examples:

- `IEnemyTargetStrategy`
- `EnemyTargetStrategy`
- `PlayerPriorityTargetStrategy`
- `CoreRushTargetStrategy`
- `EnemyController`

### Object Pool

Used for projectile reuse.

Main examples:

- `BulletPool`
- `Bullet`
- `IPoolable`
- `PlayerShooter`

### Decorator

Used for weapon damage modifiers.

Main examples:

- `IWeaponDamageModifier`
- `BaseWeaponDamageModifier`
- `DamageModifierDecorator`
- `ColorMatchDamageModifier`
- `BonusDamageModifier`
- `Bullet`

### Interfaces

Main custom interfaces:

- `IDamageable`
- `IPoolable`
- `IEnemyTargetStrategy`
- `IWeaponDamageModifier`

## Report

The final PDF report is included in the `Reports` folder.

## Unity Version

Unity 6.3 LTS / Unity 6000.3.13f1