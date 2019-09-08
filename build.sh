#!/bin/sh
set -e

dotnet build
mkdir -p ./docker-config/plugins
docker-compose kill
docker-compose up -d