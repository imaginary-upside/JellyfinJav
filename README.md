# JellyfinJav
Definitely not ready for production use yet, so use at your own risk.
I'm not a C# developer, and am not really a big fan of the language, so the code isn't the greatest.

# Metadata Providers
* R18 (Videos)
* AsianScreens (Actresses)

# Instructions
When adding the media library, make sure to select "Content type: movies". And "Show advanced settings" to be able to select the metadata downloaders you want. Currently only R18 is supported for movie metadata.

Actress metadata is only downloaded on demand when the actress is first accessed. This is a limitation of Jellyfin, but I'll probably be able to work around this in the future, although the workaround will probably be pretty hacky.

# Development
### Requirements
* Docker
* Docker Compose
* .NET Core v2.2

### Building
        ./build.sh

### General
JAV files for testing purposes are stored in ./videos

# License
Currently licensed under GPLv3, but I may be relicensing under AGPLv3 in the future.