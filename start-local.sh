#!/usr/bin/env bash
set -e

STACK="${1:-seq}"

case "$STACK" in
  seq)
    echo "Starting: db + api + Seq + Portainer"
    echo "  API:       http://localhost:8080"
    echo "  Seq:       http://localhost:8081"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile seq up --build
    ;;
  elk)
    echo "Starting: db + api + Elasticsearch + Kibana + Portainer"
    echo "  API:       http://localhost:8080"
    echo "  Kibana:    http://localhost:5601"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile elk up --build
    ;;
  apm)
    echo "Starting: db + api + Elasticsearch + Kibana + APM Server + Portainer"
    echo "  API:       http://localhost:8080"
    echo "  Kibana:    http://localhost:5601"
    echo "  APM:       http://localhost:8200"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile elk --profile apm up --build
    ;;
  all)
    echo "Starting: everything"
    echo "  API:       http://localhost:8080"
    echo "  Seq:       http://localhost:8081"
    echo "  Kibana:    http://localhost:5601"
    echo "  APM:       http://localhost:8200"
    echo "  Portainer: http://localhost:9000"
    docker compose --profile seq --profile elk --profile apm up --build
    ;;
  *)
    echo "Usage: ./start-local.sh [seq|elk|apm|all]"
    echo ""
    echo "  seq   db + api + Seq (default, lightweight)"
    echo "  elk   db + api + Elasticsearch + Kibana"
    echo "  apm   db + api + Elasticsearch + Kibana + APM Server"
    echo "  all   everything"
    echo ""
    echo "Portainer (http://localhost:9000) is always included."
    exit 1
    ;;
esac
