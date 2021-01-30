# JellyfinJav
Don't expect perfection.

# Metadata Providers
* R18 (Videos)
* JAVLibrary (Videos)
* AsianScreens (Actresses)
* Warashi Asian Pornstars (Actresses)

# Instructions
### Installation
Within your Jellyfin admin panel, go to Plugins -> Repositories, and add
https://raw.githubusercontent.com/imaginary-upside/JellyfinJav/master/manifest.json.
Then switch over to the Catalog tab and install the latest version.

### Usage
When adding the media library, make sure to select "Content type: movies".

### Example File Names
* abp200.mkv
* ABP200.mkv
* ABP-200.mkv
* some random text abp-200 more random text.mkv

# Development
### Requirements
* Docker
* Docker Compose
* .NET 5.0

### Building
    ./build.sh

### General
JAV files for testing purposes are stored in ./videos

# Screenshots
![Grid Example](screenshots/example-grid.jpg)
![Video Example](screenshots/example-video.jpg)
![Actress Example](screenshots/example-actress.jpg)

# License
Licensed under AGPL-3.0-only
