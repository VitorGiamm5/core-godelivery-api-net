#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd -- "$SCRIPT_DIR/.." && pwd)"
ENV_FILE="$PROJECT_ROOT/.env"
COMPOSE_FILE="$PROJECT_ROOT/deploy/docker-compose.yml"
PROJECT_NAME="local-godelivery-core"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Arquivo .env não encontrado. Execute local-infrastructure/start.sh e preencha as configurações."
  exit 1
fi

docker compose --project-name "$PROJECT_NAME" --env-file "$ENV_FILE" -f "$COMPOSE_FILE" --project-directory "$PROJECT_ROOT" down
docker compose --project-name "$PROJECT_NAME" --env-file "$ENV_FILE" -f "$COMPOSE_FILE" --project-directory "$PROJECT_ROOT" up -d --build --remove-orphans
