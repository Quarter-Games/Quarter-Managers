# Description
This is a list of Quarter games managers, that are developed to provide more easy-to-use API to start small unity projects.
They are divided into packages and can be found at npm package registry with scope 
``` com.quarter-games ```
For more information about installing a packages, please refer to 
[Unity documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/upm-scoped.html)
[Me at discord](https://discord.com/channels/157206617417449473)

Unity cannot handle package dependencies if dependecy is stored in another registry.
It wouldn't install the package if required dependency isn't install already.
If package uses another registry it is listed down below

## Manager
```
com.quarter-games.manager
```
This is an base class for singleton manager that is used in all other packages and Game Manager that should be placed in loading sceen and will init all other managers

## Basic Save

```
com.quarter-games.save.basic
```
This is base class for save manager, that saves individual values. Best solution for saving settings or basic meta game data as player level, amount of xp etc.

## Prefs Save

```
com.quarter-games.save.basic.prefs
```
Unity Player Prefs implementation of Basic Save package

## File Save
```
com.quarter-games.save.file
```
**Experimental** save system for saving data into files

## JSON File Save
```
com.quarter-games.save.file.json
```
JSON local implementation of File Save system
** IMPORTANT **
Requires installation of unity registry package: 
`com.unity.nuget.newtonsoft-json`

## Economy
```
com.quarter-games.qconomy
```
API and scriptable objects for setting up economy in a game. Have functionality of transactions and currencies. Saving the data is up to implementing class

## Basic Economy
```
com.quarter-games.economy
```
Implementation of economy system based on Basic Save Manager

## Analytics
```
com.quarter-games.analytics
```
Base class for analytics mnagers. Requires player consent to start collecting the data.
Uses Basic Save system to save player consent status.
For further information about player consent, refer to 
[Legal info about GDPR]([url](https://gdpr-info.eu/issues/consent/))
[Unity Page about privacy policy in their package](https://docs.unity.com/ugs/en-us/manual/analytics/manual/privacy-overview)
** IMPORTANT **
Requires installation of unity registry package: 
`com.unity.services.analytics`

## Unity Analytics
```
com.quarter-games.analytics.unity
```
Implementation of analytics package with unity analytics. 

## State Manager
```
com.quarter-games.state
```
Time and State manager. Has API for 4 types of timers and different game states that will change the unity time scale/internal time scale

## Sound
```
com.quarter-games.sound
```
Basic API for managing different mixers and sound effects. Requires Basic Save manager for saving the sound settings.
