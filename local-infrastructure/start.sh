#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd -- "$SCRIPT_DIR/.." && pwd)"
ENV_FILE="$PROJECT_ROOT/.env"
COMPOSE_FILE="$PROJECT_ROOT/deploy/docker-compose.yml"
PROJECT_NAME="local-godelivery-core"

if [[ ! -f "$ENV_FILE" ]]; then
  cp "$PROJECT_ROOT/.env.example" "$ENV_FILE"
  echo "Arquivo .env criado. Preencha as senhas e os parâmetros JWT antes de iniciar."
  exit 1
fi

docker compose --project-name "$PROJECT_NAME" --env-file "$ENV_FILE" -f "$COMPOSE_FILE" --project-directory "$PROJECT_ROOT" up -d --build --remove-orphans
