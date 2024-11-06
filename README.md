# Zone Extractor

Extract quest zones info while running SPT.

## Requirements

- .NET 8.0

## Build

`dotnet build`

## Usage

1. Put `ZoneExtractor.dll` into `BepInEx/Plugins/`
2. Start the game
3. Do a raid on each location, wait for 1 minute after loading in before exiting the raid
4. Close the game
5. A file named `questpoints.jsonc` is produced in the game's root directory

## License

- The source in this repo is covered under MIT
- `spt-reflection.dll` and `hollowed.dll` are covered under NCSA
