#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# Installs or updates the Unitas webshop on the web server (172.20.66.40).
# See deploy/README.md.
#
#   sudo bash deploy-webserver.sh <folder with the published website>
#
# The connection string must already be in /etc/unitas/unitas.env (root, 600).
# ---------------------------------------------------------------------------
set -euo pipefail

RELEASE_DIR="${1:?Geef de map met de gepubliceerde website op}"
SERVER_IP="${SERVER_IP:-172.20.66.40}"
HERE="$(cd "$(dirname "$0")" && pwd)"
APP_DIR=/opt/unitas

[ "$(id -u)" -eq 0 ] || { echo "Start dit script als root (sudo)."; exit 1; }
[ -f "$RELEASE_DIR/Project Unitas.dll" ] || { echo "'Project Unitas.dll' niet gevonden in $RELEASE_DIR"; exit 1; }
[ -f /etc/unitas/unitas.env ] || { echo "/etc/unitas/unitas.env ontbreekt (zie deploy/README.md)"; exit 1; }

echo "== 1/5 ASP.NET Core runtime"
if ! dpkg -s aspnetcore-runtime-10.0 >/dev/null 2>&1; then
    apt-get update -qq
    DEBIAN_FRONTEND=noninteractive apt-get install -y -qq aspnetcore-runtime-10.0 >/dev/null
fi
dotnet --list-runtimes | grep AspNetCore

echo "== 2/5 Service-account 'unitas'"
id unitas >/dev/null 2>&1 || useradd --system --no-create-home --shell /usr/sbin/nologin unitas
chown root:root /etc/unitas/unitas.env
chmod 600 /etc/unitas/unitas.env

echo "== 3/5 Website installeren in $APP_DIR"
systemctl stop unitas 2>/dev/null || true
rm -rf "$APP_DIR.new"
cp -r "$RELEASE_DIR" "$APP_DIR.new"
rm -f "$APP_DIR.new/appsettings.Development.json" "$APP_DIR.new/web.config"
# Owned by root and read-only for the app: if the app is ever hacked it can't change itself.
chown -R root:root "$APP_DIR.new"
find "$APP_DIR.new" -type d -exec chmod 755 {} +
find "$APP_DIR.new" -type f -exec chmod 644 {} +
rm -rf "$APP_DIR.old"
[ -d "$APP_DIR" ] && mv "$APP_DIR" "$APP_DIR.old"
mv "$APP_DIR.new" "$APP_DIR"

echo "== 4/5 systemd-service"
install -m 644 "$HERE/unitas.service" /etc/systemd/system/unitas.service
systemctl daemon-reload
systemctl enable --now unitas >/dev/null
sleep 3
systemctl is-active unitas

echo "== 5/5 Apache als reverse proxy met HTTPS"
if [ ! -f /etc/ssl/unitas/unitas.crt ]; then
    # Self-signed certificate for the server's IP address (valid 1 year).
    # Browsers warn until the certificate is trusted: see deploy/README.md.
    install -d -m 755 /etc/ssl/unitas
    openssl req -x509 -newkey rsa:3072 -sha256 -days 365 -nodes \
        -keyout /etc/ssl/unitas/unitas.key -out /etc/ssl/unitas/unitas.crt \
        -subj "/O=Unitas Bakery Systems BV/CN=$SERVER_IP" \
        -addext "subjectAltName=IP:$SERVER_IP" \
        -addext "keyUsage=critical,digitalSignature,keyEncipherment" \
        -addext "extendedKeyUsage=serverAuth" 2>/dev/null
    chmod 600 /etc/ssl/unitas/unitas.key
    chmod 644 /etc/ssl/unitas/unitas.crt
    echo "   Certificaat aangemaakt: /etc/ssl/unitas/unitas.crt"
fi
a2enmod -q proxy proxy_http headers ssl >/dev/null
install -m 644 "$HERE/apache-unitas.conf" /etc/apache2/sites-available/unitas.conf
install -m 644 "$HERE/apache-hardening.conf" /etc/apache2/conf-available/unitas-hardening.conf
a2enconf -q unitas-hardening >/dev/null
a2dissite -q 000-default >/dev/null 2>&1 || true
a2ensite -q unitas >/dev/null
apachectl configtest
systemctl restart apache2   # restart (not reload): a newly enabled ssl module needs it

echo
echo "Klaar. De webshop draait op https://$SERVER_IP/"
echo "Logs: journalctl -u unitas -f   |   /var/log/apache2/unitas-*.log"
