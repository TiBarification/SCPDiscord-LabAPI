# Bot commands

## Bot commands

| Command                              | Description                                                                    |
|--------------------------------------|--------------------------------------------------------------------------------|
| `/ban <steamid> <duration> (reason)` | Bans a player from the server.                                                 |
| `/help`                              | Shows a message with useful information.                                       |
| `/kick <steamid> (reason)`           | Kicks a player from the server.                                                |
| `/kickall (reason)`                  | Kicks all players from the server with a message, useful for server shutdowns. |
| `/list`                              | Lists all online players.                                                      |
| `/ra <command>`                      | Runs a remote admin command.                                                   |
| `/server <command>`                  | Runs a server command.                                                         |
| `/syncid <steamid>`                  | Syncs your Discord role to the game.                                           |
| `/syncip <ip>`                       | Syncs your Discord role to the game. (For servers not using steam)             |
| `/unban <steamid/ip>`                | Unbans a player from the server.                                               |
| `/unsync`                            | Unsyncs your Discord account.                                                  |
| `/unsyncplayer`                      | Removes the steam account from being synced with your discord account.         |

## Plugin console commands

| Command                                                    | Permission (Waiting for API implementation) | Description                                                                                 |
|------------------------------------------------------------|---------------------------------------------|---------------------------------------------------------------------------------------------|
| `scpd_debug`                                               | `scpdiscord.debug`                          | Toggles debug console messages.                                                             |
| `scpd_grantreservedslot/scpd_grs <steamid>`                | `scpdiscord.removereservedslot`             | Gives a player a reserved slot on the server.                                               |
| `scpd_grantvanillarank/scpd_gvr <steamid/playerid> <rank>` | `scpdiscord.grantreservedslot`              | Gives a player a vanilla rank for their current session.                                    |
| `scpd_reconnect, scpd_rc`                                  | `scpdiscord.reconnect`                      | Reconnects to the bot.                                                                      |
| `scpd_reload`                                              | `scpdiscord.reload`                         | Reloads the plugin, all configs and files and reconnects.                                   |
| `scpd_removereservedslot/scpd_rrs <steamid>`               | `scpdiscord.grantvanillarank`               | Removes a reserved slot from a player.                                                      |
| `scpd_setnickname <player id/steamid> <nickname>`          | `scpdiscord.setnickname`                    | Sets a player's nickname, useful for the rolesync system if you want to sync discord names. |
| `scpd_unsync <discordid>`                                  | `scpdiscord.unsync`                         | Manually remove a player from being synced to discord.                                      |
| `scpd_validate`                                            | `scpdiscord.validate`                       | Creates a config and language validation report in the console.                             |
| `scpd_verbose`                                             | `scpdiscord.verbose`                        | Toggles verbose console messages.                                                           |

## Time

Time is expressed in the format `NumberUnit` where unit is a unit of time and number is the amount of that time unit, for example `6M` represents six months.

Valid time units:

|  Letter   |   Unit    |
|:---------:|:---------:|
|    `s`    |  Seconds  |
|    `m`    |  Minutes  |
|    `h`    |   Hours   |
|    `d`    |   Days    |
|    `w`    |   Weeks   |
|    `M`    |  Months   |
|    `y`    |   Years   |