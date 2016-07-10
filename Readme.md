Alpha WoW
=============

A project that I've worked on and off over the last 8 months, the original idea was to create a simple sandbox with basic networking for the original Alpha client (0.5.3) however, as per usual, it evolved and I've implemented the base of a few other systems.

A quick disclaimer: this was never intended to be a commercial project and therefore it is the definition of spaghetti code! I wouldn't recommend trying to use this outside of being a reference. If I were to redo this I'd use one of the vanilla cores as a base and work back from that.

Current status of features:
-	Chat and emotes
  -	Chat channels not implemented
-	Items
-	Trading
  -	No enchanting through the chat window
-	Groups
  -	Group loot not implemented
-	Quests
  -	No ScriptDev equivalent so scripted quests won't work
  -	Not all of the correct quest filters are applied
  -	Issue with displaying quest objective count
-	Instance portals
  -	Instancing itself is not implemented
-	Gameobjects
  -	Only spawning (chests are animated though!)
-	NPCs
  -	Vendors, talent trainers, bankers and skill trainers are working to some degree
  -	Basic faction implementation
-	Friend/Ignore List
-	Talent "tree"
  -	Spell effects not implemented
-	Spell casting
  -	Spell effects not implemented but cast animations and pre cast checks are
-	Creatures
  -	Can melee combat
  -	Looting works, couldn't figure out how looting worked in terms that tagging mobs wasn't a feature, should everyone have access to the dead mob's loot?
  -	Movement isn't correct
  -	AI not implemented
  -	Basic faction implementation
-	Levelling
  -	Correct talent and skill points being applied

Some things to note:
-	I couldn't find any reliable source to say if guilds even existed at this stage. The packets are there and the code is in the client but whether it was released I'm not sure so haven't implemented it
-	All settings are in the Globals.cs file including level cap
-	I've used a Mangos 1.12.1 database as the backend so a lot of items/creatures/stats are wrong
-	GM Commands include
  -	.additem [item number]
  -	.addskill [skillid]
  -	.setskill [skillid] [amount] [max amount]
  -	.kill
  -	.level [level]
  -	.money [copper amount]
  -	.setpower [amount]
-	Saving is something I started working on but never really implemented
-	The saving and database mechanic works around a custom ORM which is heavily reflection reliant

Requirements:
-	.Net 4.6.1
-	MySQL

Installation:
-	Extract the DBC files to a folder named "dbc" under the root directory 
  -	MPQEdit works for this 
-	Unzip and run the SQL file in the Database.zip archive
-	Inside the App.Config file is a MySQL connection string, change this to point to your database
-	Create a shortcut to WoWClient.exe and add –uptodate to the end
  -	i.e. "E:\World of Warcraft Alpha 0.5.3\WoWClient.exe" –uptodate
