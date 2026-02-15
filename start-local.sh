#!/usr/bin/env bash
set -e

STACK="${1:-seq}"

down() {
  echo "Stopping all containers..."
  docker compose --profile seq --profile elk --profile apm down
}

case "$STACK" in
  seq)
    export LOGGING_SINK=seq
    echo "Starting: db + api + Seq + Portainer"
    echo "  API:       http://localhost:8000"
    echo "  Seq:       http://localhost:8081"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile seq up --build -d
    ;;
  elk)
    export LOGGING_SINK=elk
    echo "Starting: db + api + Elasticsearch + Kibana + Portainer"
    echo "  API:       http://localhost:8000"
    echo "  Kibana:    http://localhost:5601"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile elk up --build -d
    ;;
  apm)
    export LOGGING_SINK=elk
    echo "Starting: db + api + Elasticsearch + Kibana + APM Server + Portainer"
    echo "  API:       http://localhost:8000"
    echo "  Kibana:    http://localhost:5601"
    echo "  APM:       http://localhost:8200"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile elk --profile apm up --build -d
    ;;
  all)
    export LOGGING_SINK=seq
    echo "Starting: everything"
    echo "  API:       http://localhost:8000"
    echo "  Seq:       http://localhost:8081"
    echo "  Kibana:    http://localhost:5601"
    echo "  APM:       http://localhost:8200"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile seq --profile elk --profile apm up --build -d
    ;;
  down)
    down
    exit 0
    ;;
  logs)
    docker compose logs -f
    ;;
  *)
    echo "Usage: ./start-local.sh [seq|elk|apm|all|down|logs]"
    echo ""
    echo "  seq    db + api + Seq (default, lightweight)"
    echo "  elk    db + api + Elasticsearch + Kibana"
    echo "  apm    db + api + Elasticsearch + Kibana + APM Server"
    echo "  all    everything"
    echo "  down   stop all containers"
    echo "  logs   follow logs of running containers"
    echo ""
    echo "Portainer (http://localhost:9000) is always included."
    exit 1
    ;;
esac

echo ""
echo "Done. Use './start-local.sh logs' to follow logs, './start-local.sh down' to stop."
