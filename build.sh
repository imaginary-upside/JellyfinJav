#!/bin/sh
set -e

(cd JellyfinJav; dotnet build)
mkdir -p ./docker-config/plugins
docker-compose kill
docker-compose up -d