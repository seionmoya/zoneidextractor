# zoneidextractor

Extract zones from dumps from player log

## Requirements

- .NET 8.0
- `QuestExtender` inside SPT's bepinex

## Build

- Debug: `dotnet build`
- Release: `dotnet publish`

## Usage

`Player.log` can be found in `C:\Users\User\AppData\LocalLow\Battlestate Games\EscapeFromTarkov\`.

1. Put `Player.log` next to `zoneidextractor.exe`
2. Run `zoneidextractor`
3. A file named `questpoints.jsonc` is produced
