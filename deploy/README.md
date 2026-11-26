# Deployment – webserver (172.20.66.40)

De webshop draait op **https://172.20.66.40** (hostname `webserver`, Ubuntu 26.04).
De database staat op 172.20.66.35. Zie [`db/README.md`](../db/README.md).

```
bezoeker ──HTTPS 443──▶ Apache (reverse proxy) ──▶ website op 127.0.0.1:5000 ──TLS──▶ MySQL op .35:3306
          (HTTP 80 stuurt door naar HTTPS)          (systemd-service "unitas")
```

## Wat draait er?

| Onderdeel | Waar | Beveiliging |
|---|---|---|
| Website (ASP.NET Core 10) | `/opt/unitas`, service `unitas` | Draait als eigen account `unitas` zonder login; bestanden alleen-lezen; alleen bereikbaar via localhost; systemd-hardening |
| Connection string | `/etc/unitas/unitas.env` | Alleen leesbaar voor root; niet in git |
| Apache | site `unitas` | HTTP → HTTPS, alleen TLS 1.2/1.3, security headers, geen versie-informatie |
| Certificaat | `/etc/ssl/unitas/` | Self-signed voor 172.20.66.40, 1 jaar geldig (tot nov. 2027) |
| Firewall (ufw) | webserver én databaseserver | Zie hieronder |

## Firewall (`firewall.sh`)

| Server | Toegestaan | Al het andere |
|---|---|---|
| webserver (.40) | 80 en 443 voor iedereen; SSH (22) alleen vanuit MGMT 172.20.66.0/24 | geblokkeerd en gelogd |
| database (.35) | MySQL (3306) alleen vanaf .40; SSH (22) alleen vanuit MGMT 172.20.66.0/24 | geblokkeerd en gelogd |

Instellen of opnieuw toepassen: `sudo bash firewall.sh webserver` of `sudo bash firewall.sh database`.

## Website bijwerken (nieuwe versie)

Op je eigen computer, vanuit de hoofdmap van de repository:

```bash
dotnet publish "Project Unitas/Project Unitas.csproj" -c Release -o publish
scp -r publish deploy <gebruiker>@172.20.66.40:~
ssh <gebruiker>@172.20.66.40
sudo bash ~/deploy/deploy-webserver.sh ~/publish
rm -rf ~/publish ~/deploy
```

Het script installeert de runtime als die ontbreekt, zet de nieuwe versie in `/opt/unitas` (de vorige blijft als `/opt/unitas.old`), herstart de service en controleert de Apache-configuratie.

**Database gewijzigd (nieuwe migration)?** Voer eerst het SQL-script uit op de databaseserver (zie `db/README.md`), daarna pas de nieuwe website.

## Handig

| Wat | Commando (op .40) |
|---|---|
| Status | `systemctl status unitas` |
| Logs van de website (o.a. mislukte logins) | `journalctl -u unitas -f` |
| Logs van Apache | `/var/log/apache2/unitas-access.log`, `unitas-error.log` |
| Geblokkeerde verbindingen | `/var/log/ufw.log` |
| Herstarten | `sudo systemctl restart unitas` |
| Terug naar vorige versie | `sudo systemctl stop unitas && sudo mv /opt/unitas /opt/unitas.bad && sudo mv /opt/unitas.old /opt/unitas && sudo systemctl start unitas` |

## Certificaatwaarschuwing in de browser

Het certificaat is self-signed. Browsers waarschuwen dus totdat het vertrouwd wordt. Twee opties:
- **Netjes (aanrader):** verspreid `/etc/ssl/unitas/unitas.crt` via **Group Policy** op de AD-server (.30) naar *Trusted Root Certification Authorities* van alle domein-pc's.
- **Voor één pc:** download het certificaat en importeer het in de certificaatopslag van Windows of de browser.

Het certificaat verloopt in **november 2027**. Daarna verwijder je `/etc/ssl/unitas/` en draai je `deploy-webserver.sh` opnieuw: dan wordt een nieuw certificaat gemaakt.

## Nog te doen
- SSH met sleutels in plaats van wachtwoorden, en wachtwoord-login voor SSH uitzetten.
- Logs centraal verzamelen (syslog/monitoring-server).
- Back-ups van de database ook buiten .35 bewaren.
