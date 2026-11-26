#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# Sets up the Unitas MySQL database server (172.20.66.35) — see db/README.md.
#
# Run on a fresh Ubuntu 24.04 container/VM, as root, with unitas-schema.sql
# in the same folder:
#     sudo bash setup-mysql-server.sh
#
# What it does (security by design):
#  - installs MySQL (8.0 or 8.4) and listens ONLY on the server's own address
#  - requires TLS-encrypted connections, disables LOAD DATA LOCAL
#  - creates database "unitas" with user "unitas_web" that:
#      * may only log in FROM the web server (172.20.66.40)
#      * may only read/change data (no DROP/CREATE/ALTER: least privilege)
#  - loads the tables and starting data
#  - sets up a daily back-up (mysqldump) with 14 days retention
# ---------------------------------------------------------------------------
set -euo pipefail

DB_SERVER_IP="${DB_SERVER_IP:-172.20.66.35}"
WEB_SERVER_IP="${WEB_SERVER_IP:-172.20.66.40}"
DB_NAME=unitas
DB_USER=unitas_web
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SCHEMA="$SCRIPT_DIR/unitas-schema.sql"

[ "$(id -u)" -eq 0 ] || { echo "Start dit script als root (sudo bash $0)."; exit 1; }
[ -f "$SCHEMA" ] || { echo "unitas-schema.sql niet gevonden naast dit script."; exit 1; }

echo "== 1/5 MySQL installeren"
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get install -y -qq mysql-server >/dev/null

echo "== 2/5 MySQL beveiligen"
cat > /etc/mysql/mysql.conf.d/unitas.cnf <<EOF
[mysqld]
# Only listen on the server's own network address (not on every interface).
bind-address             = $DB_SERVER_IP
# Refuse connections that are not encrypted with TLS.
require_secure_transport = ON
# Don't allow clients to read files from the server via LOAD DATA LOCAL.
local_infile             = OFF
EOF
systemctl restart mysql

# Same clean-up as mysql_secure_installation (root keeps logging in via the local socket only).
mysql <<'SQL'
DELETE FROM mysql.user WHERE User = '';
DELETE FROM mysql.user WHERE User = 'root' AND Host NOT IN ('localhost', '127.0.0.1', '::1');
DROP DATABASE IF EXISTS test;
FLUSH PRIVILEGES;
SQL

echo "== 3/5 Database en gebruiker aanmaken"
NEW_PASSWORD=""
mysql -e "CREATE DATABASE IF NOT EXISTS \`$DB_NAME\` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;"
if [ "$(mysql -N -e "SELECT COUNT(*) FROM mysql.user WHERE User='$DB_USER' AND Host='$WEB_SERVER_IP'")" = "0" ]; then
    # 192 random bits, with upper case, lower case, digits and "-" so it also passes
    # MySQL 8.4's validate_password policy. Only characters that are safe in a connection string.
    NEW_PASSWORD="Ux$(openssl rand -hex 12)-$(openssl rand -hex 12 | tr 'a-f' 'A-F')9"
    mysql <<SQL
CREATE USER '$DB_USER'@'$WEB_SERVER_IP' IDENTIFIED BY '$NEW_PASSWORD' REQUIRE SSL;
GRANT SELECT, INSERT, UPDATE, DELETE ON \`$DB_NAME\`.* TO '$DB_USER'@'$WEB_SERVER_IP';
SQL
else
    echo "   Gebruiker $DB_USER@$WEB_SERVER_IP bestaat al; wachtwoord blijft ongewijzigd."
fi

echo "== 4/5 Tabellen en startgegevens laden"
if [ "$(mysql -N -e "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='$DB_NAME' AND table_name='__EFMigrationsHistory'")" = "0" ]; then
    mysql "$DB_NAME" < "$SCHEMA" >/dev/null
    echo "   Klaar: $(mysql -N -e "SELECT COUNT(*) FROM \`$DB_NAME\`.Machines") machines geladen."
else
    echo "   Tabellen bestaan al; overgeslagen."
fi

echo "== 5/5 Dagelijkse back-up instellen"
install -d -m 700 /var/backups/mysql
cat > /etc/cron.daily/unitas-mysql-backup <<EOF
#!/bin/sh
# Daily back-up of the Unitas database; keeps 14 days.
# TODO (team): also copy /var/backups/mysql to storage OUTSIDE this server.
set -e
mysqldump --single-transaction --routines --triggers $DB_NAME | gzip > /var/backups/mysql/$DB_NAME-\$(date +%F).sql.gz
chmod 600 /var/backups/mysql/*.sql.gz
find /var/backups/mysql -name '*.sql.gz' -mtime +14 -delete
EOF
chmod 700 /etc/cron.daily/unitas-mysql-backup
/etc/cron.daily/unitas-mysql-backup
echo "   Eerste back-up: $(ls /var/backups/mysql | tail -1)"

echo
echo "================================================================"
echo " MySQL is klaar op $DB_SERVER_IP:3306"
if [ -n "$NEW_PASSWORD" ]; then
    echo
    echo " Zet deze connection string op de WEBSERVER ($WEB_SERVER_IP) als"
    echo " environment variable ConnectionStrings__Unitas (zie db/README.md):"
    echo
    echo "   Server=$DB_SERVER_IP;Database=$DB_NAME;User=$DB_USER;Password=$NEW_PASSWORD;SslMode=Required"
    echo
    echo " Het wachtwoord wordt nergens anders opgeslagen. Bewaar het in jullie"
    echo " wachtwoordkluis en zet het NOOIT in git."
fi
echo "================================================================"
