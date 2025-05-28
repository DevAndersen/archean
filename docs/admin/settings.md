# Settings

Settings are defined in the `appsettings.json` file, using the [JSON](https://en.wikipedia.org/wiki/JSON) data format. This file should be located in the same directory as the server executable.

Each of the sections described below can be configured by defining them as a top-level node in the `appsettings.json` file.

Here is example of a simple `appsettings.json` file:

```json
{
    "Server":
    {
        "Name": "My server",
        "Motd": "Welcome to my server",
        "MaxPlayers": 10,
        "Port": 25565
    },
    "Chat":
    {
        "ChatFormat": "<&b{1}&f> {0}",
        "ServerChatFormat": "&e[Server]&f {0}"
    }
}
```

Additionally, settings can be defined using environment variables.

These must follow the same naming pattern as seen in the `appsettings.json`, with each level separated by two underscores (`__`). These will override the values defined in the `appsettings.json` file.

For example, to use an environment variable to set the server's name, the name of the corresponding environment variable would be `Server__Name`.

## Server

These settings let you configure the base server setup.

`appsettings.json` path: `Server`

<table>
    <thead>
        <tr>
            <th>Key</th>
            <th>Description</th>
            <th>Data type</th>
            <th>Remarks</th>
            <th>Fallback value</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Name</td>
            <td>The name of the server.</td>
            <td>String</td>
            <td></td>
            <td>
                <code>Unnamed server</code>
            </td>
        </tr>
        <tr>
            <td>Motd</td>
            <td>The MOTD (Message Of The Day), displayed beneath the server name when joining the server.</td>
            <td>String</td>
            <td></td>
            <td>
                <code>Server MOTD</code>
            </td>
        </tr>
        <tr>
            <td>MaxPlayers</td>
            <td>The maximum number of players that can be connected to the server at the same time.</td>
            <td>Number (1-127)</td>
            <td></td>
            <td>
                <code>127</code>
            </td>
        </tr>
        <tr>
            <td>Port</td>
            <td>The networking port that the server will listen for connections on.</td>
            <td>Number (ushort)</td>
            <td></td>
            <td>
                <code>25565</code>
            </td>
        </tr>
        <tr>
            <td>WorldLoadingMotd</td>
            <td>The MOTD message shown when joining a world.</td>
            <td>String</td>
            <td></td>
            <td>
                <code>Loading world, {0}% done</code>
            </td>
        </tr>
        <tr>
            <td>ServerFullMessage</td>
            <td>The message displayed when a player's attempt to join the server is prevented due to the server already being full.</td>
            <td>String</td>
            <td></td>
            <td>
                <code>The server is full</code>
            </td>
        </tr>
        <tr>
            <td>Backlog</td>
            <td>The number of connections that can be in waiting position.</td>
            <td>Number</td>
            <td>This is for configuring the underlying socket server itself.</td>
            <td>
                <code>3</code>
            </td>
        </tr>
    </tbody>
</table>

## Chat

These settings let you configure chat messages and formatting.

`appsettings.json` path: `Chat`

<table>
    <thead>
        <tr>
            <th>Key</th>
            <th>Description</th>
            <th>Data type</th>
            <th>Remarks</th>
            <th>Fallback value</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>ChatFormat</td>
            <td>The format used by player chat messages.</td>
            <td>String</td>
            <td>
                <p>The string will apply the following formatting:</p>
                <ul>
                    <li><code>{0}</code> will be replaced by the chat message.</li>
                    <li><code>{1}</code> will be replaced by the player name.</li>
                </ul>
            </td>
            <td>
                <code><{1}> {0}</code>
            </td>
        </tr>
        <tr>
            <td>ServerChatFormat</td>
            <td>The format used by system chat messages.</td>
            <td>String</td>
            <td>
                <p>The string will apply the following formatting:</p>
                <ul>
                    <li><code>{0}</code> will be replaced by the chat message.</li>
                </ul>
            </td>
            <td>
                <code>[Server] {0}</code>
            </td>
        </tr>
    </tbody>
</table>

## Block aliases

These settings let you configure aliases for blocks. These are used by commands that require specifying a type of block.

`appsettings.json` path: `Aliases`

<table>
    <thead>
        <tr>
            <th>Key</th>
            <th>Description</th>
            <th>Data type</th>
            <th>Remarks</th>
            <th>Fallback value</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>RegisterDefaultNameAliases</td>
            <td>Automatically registers the default block names as aliases, for example <code>"stone"</code> for the Stone block type.</td>
            <td>Boolean</td>
            <td></td>
            <td>
                <code>true</code>
            </td>
        </tr>
        <tr>
            <td>RegisterDefaultIdAliases</td>
            <td>Automatically registers the default block names as aliases, for example <code>"1"</code> for the Stone block type.</td>
            <td>Boolean</td>
            <td></td>
            <td>
                <code>true</code>
            </td>
        </tr>
        <tr>
            <td>CustomAliases</td>
            <td>Allows you to define custom aliases for block types</td>
            <td>Object</td>
            <td>See below for further information.</td>
            <td></td>
        </tr>
    </tbody>
</table>

### Custom aliases

You can use this setting to define custom aliases for block types.

You can either use the name of the block type or the block ID as the key.

Custom aliases are defined in an array, allowing you to define multiple aliases for each block type.

Here is an example which defines "Soil" and "Earth" as aliases for Dirt, and "Cobble" as an alias for Cobblestone (block ID 4).

```json
"Aliases":
{
    "CustomAliases":
    {
        "Dirt":
        [
            "Soil",
            "Earth"
        ],
        "4":
        [
            "Cobble"
        ]
    }
}
```

Do note:

- Block aliases are case-insensitive.
- You cannot reuse an alias that has already been defined.
- You cannot define aliases for blocks that are not known to Archean (from Air to Obsidian).

## Logging

These settings are used to configure logging.

`appsettings.json` path: `Logging`

This is also where logging levels can be configured. For more information, see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging#configure-logging
