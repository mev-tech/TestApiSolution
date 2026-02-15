#!/usr/bin/env bash
set -e

STACK="${1:-seq}"

echo "Restarting local environment ($STACK)..."
docker compose --profile seq --profile elk --profile apm down
./start-local.sh "$STACK"
