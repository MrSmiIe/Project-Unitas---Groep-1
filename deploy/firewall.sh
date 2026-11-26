#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# Host firewall (ufw) for the Unitas servers: block everything incoming except
# what the server is for. See deploy/README.md.
#
#   sudo bash firewall.sh webserver     (on 172.20.66.40)
#   sudo bash firewall.sh database      (on 172.20.66.35)
#
# SSH stays open for the management network (VLAN 66), so you can't lock yourself
# out from there. Safe to run more than once.
# ---------------------------------------------------------------------------
set -euo pipefail

ROLE="${1:?Gebruik: firewall.sh webserver|database}"
ADMIN_NET="${ADMIN_NET:-172.20.66.0/24}"      # MGMT VLAN 66: beheer via SSH
WEB_SERVER_IP="${WEB_SERVER_IP:-172.20.66.40}"

[ "$(id -u)" -eq 0 ] || { echo "Start dit script als root (sudo)."; exit 1; }
command -v ufw >/dev/null || { apt-get update -qq && apt-get install -y -qq ufw >/dev/null; }

ufw default deny incoming >/dev/null
ufw default allow outgoing >/dev/null

# SSH only from the management network (added first, so the current SSH session keeps working).
ufw allow from "$ADMIN_NET" to any port 22 proto tcp comment 'SSH beheer (MGMT)' >/dev/null
# Remove broader SSH rules ("from anywhere") that would undo the line above.
ufw delete allow 22/tcp >/dev/null 2>&1 || true
ufw delete allow OpenSSH >/dev/null 2>&1 || true

case "$ROLE" in
    webserver)
        ufw allow 80/tcp  comment 'HTTP (redirect naar HTTPS)' >/dev/null
        ufw allow 443/tcp comment 'HTTPS webshop' >/dev/null
        ;;
    database)
        ufw allow from "$WEB_SERVER_IP" to any port 3306 proto tcp comment 'MySQL alleen voor de webserver' >/dev/null
        ;;
    *)
        echo "Onbekende rol: $ROLE (gebruik webserver of database)"; exit 1 ;;
esac

ufw logging low >/dev/null      # log blocked connection attempts (/var/log/ufw.log)
ufw --force enable >/dev/null
ufw status verbose
