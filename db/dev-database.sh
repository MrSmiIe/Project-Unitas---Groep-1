#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# Local test database for developers (Linux/macOS with Docker).
#
# Starts MySQL 8.4 (same as the server) in Docker (only reachable from this computer), creates the
# "unitas" database and the restricted "unitas_web" user (same rights as on the
# real server), loads db/unitas-schema.sql and stores the connection string in
# dotnet user-secrets — so it never ends up in git.
#
# Usage (from the repository root):   bash db/dev-database.sh
# Start again after a reboot:          docker start unitas-mysql
# Throw everything away:               docker rm -f unitas-mysql && docker volume rm unitas-mysql-data
# ---------------------------------------------------------------------------
set -euo pipefail

CONTAINER=unitas-mysql
VOLUME=unitas-mysql-data
PORT=3306
REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT="$REPO_ROOT/Project Unitas"

if docker ps -a --format '{{.Names}}' | grep -qx "$CONTAINER"; then
    echo "Container '$CONTAINER' bestaat al. Starten..."
    docker start "$CONTAINER" >/dev/null
    echo "Klaar. (Opnieuw beginnen? docker rm -f $CONTAINER && docker volume rm $VOLUME)"
    exit 0
fi

ROOT_PASSWORD="$(openssl rand -hex 24)"
WEB_PASSWORD="$(openssl rand -hex 24)"

echo "MySQL starten in Docker (alleen bereikbaar via 127.0.0.1:$PORT)..."
docker run -d --name "$CONTAINER" \
    -e MYSQL_ROOT_PASSWORD="$ROOT_PASSWORD" \
    -p 127.0.0.1:$PORT:3306 \
    -v "$VOLUME":/var/lib/mysql \
    mysql:8.4 >/dev/null

echo -n "Wachten tot MySQL klaar is"
until docker exec -e MYSQL_PWD="$ROOT_PASSWORD" "$CONTAINER" mysql -uroot -e "SELECT 1" >/dev/null 2>&1; do
    echo -n "."; sleep 2
done
echo

run_sql() { docker exec -i -e MYSQL_PWD="$ROOT_PASSWORD" "$CONTAINER" mysql -uroot "$@"; }

echo "Database en gebruiker aanmaken..."
run_sql <<SQL
CREATE DATABASE unitas CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Least privilege: the website may only read and change data, not the table structure.
CREATE USER 'unitas_web'@'%' IDENTIFIED BY '$WEB_PASSWORD' REQUIRE SSL;
GRANT SELECT, INSERT, UPDATE, DELETE ON unitas.* TO 'unitas_web'@'%';
SQL

echo "Tabellen en startgegevens laden (db/unitas-schema.sql)..."
run_sql unitas < "$REPO_ROOT/db/unitas-schema.sql" >/dev/null

echo "Connection string opslaan in dotnet user-secrets (niet in git)..."
dotnet user-secrets set "ConnectionStrings:Unitas" \
    "Server=127.0.0.1;Port=$PORT;Database=unitas;User=unitas_web;Password=$WEB_PASSWORD;SslMode=Required" \
    --project "$PROJECT" >/dev/null

echo "Klaar! Start de website met:  cd \"Project Unitas\" && dotnet run"
