# JellyfinJav
Mostly ready for production use, but don't expect perfection.

# Screenshots
![Grid Example](screenshots/example-grid.jpg)
![Video Example](screenshots/example-video.jpg)
![Actress Example](screenshots/example-actress.jpg)

# Metadata Providers
* R18 (Videos) - English
* JAVLibrary (Videos) - English
* JavBus (Videos, Actresses) - Chinese
* AsianScreens (Actresses) - English

# Instructions
### Installation
Either download the latest prebuilt release and drop the .dll into Jellyfin's plugins directory, or build the .dll with the following:
        
    dotnet publish --configuration Release
    cp bin/Release/netstandard2.0/JellyfinJav.dll $JELLYFIN_DIR/plugins/

### Usage
When adding the media library, make sure to select "Content type: movies". And "Show advanced settings" to be able to select the metadata downloaders you want.

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
Licensed under AGPL-3.0-only
