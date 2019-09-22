# JellyfinJav
Mostly ready for production use, but don't expect perfection.

# Metadata Providers
* R18 (Videos)
* AsianScreens (Actresses)

# Instructions
### Installation
Either download the latest prebuilt release and drop the .dll into Jellyfin's plugins directory, or build the .dll with the following:
        
    dotnet publish --configuration Release
    cp bin/Release/netstandard2.0/JellyfinJav.dll $JELLYFIN_DIR/plugins/

### Usage
When adding the media library, make sure to select "Content type: movies". And "Show advanced settings" to be able to select the metadata downloaders you want. Currently only R18 is supported for movie metadata.

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
