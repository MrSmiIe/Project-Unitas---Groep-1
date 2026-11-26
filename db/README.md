# Database – Unitas webshop

De website slaat zijn gegevens op in **MySQL** op de databaseserver **172.20.66.35** (hostname `database`, Ubuntu 26.04, MySQL 8.4) (zie de netwerkschets).

## Wat staat er in de database?

| Tabel | Inhoud |
|---|---|
| `Machines`, `MachineSpecs` | De machines in de webshop met hun specificaties (8 machines als startdata) |
| `ContactMessages` | Berichten van het contactformulier |
| `AspNetUsers` | Klantaccounts: naam, bedrijf, e-mail, **wachtwoord-hash** (nooit het wachtwoord zelf), MFA aan/uit, blokkering |
| `AspNetUserTokens` | MFA-sleutel en herstelcodes per klant |
| Overige `AspNet…` tabellen | Rollen en logins van ASP.NET Core Identity (nu nog niet gebruikt) |

De tabellen worden gemaakt door [`unitas-schema.sql`](unitas-schema.sql). Dat bestand wordt gegenereerd uit de C#-code (zie *Database wijzigen*), dus pas het niet met de hand aan.

---

## 1. Databaseserver opzetten (172.20.66.35)

### a. Container maken in Proxmox
In de Proxmox-webinterface (https://172.20.66.20:8006): **Create CT**

| Instelling | Waarde |
|---|---|
| Hostname | `unitas-db` |
| Unprivileged container | aan |
| Template | Ubuntu 24.04 of nieuwer (getest op 24.04 en 26.04) |
| Disk | 8 GB |
| CPU / geheugen | 1 core / 1024 MB |
| Netwerk | IPv4 static `172.20.66.35/24`, gateway `172.20.66.1` |

### b. Firewall in Proxmox (CT → Firewall)
Zet de firewall van de container **aan** met als standaard *DROP* voor binnenkomend verkeer, en sta alleen dit toe:

| Richting | Bron | Poort | Waarom |
|---|---|---|---|
| IN | `172.20.66.40` (webserver) | TCP 3306 | De website praat met MySQL |
| IN | beheer-pc's (MGMT) | TCP 22 | Beheer via SSH |

### c. Script uitvoeren
Kopieer twee bestanden naar de server en voer het script uit:

```bash
scp db/setup-mysql-server.sh db/unitas-schema.sql <gebruiker>@172.20.66.35:~
ssh <gebruiker>@172.20.66.35
sudo bash setup-mysql-server.sh
```

Het script installeert MySQL, beveiligt het, maakt de database en de gebruiker `unitas_web`, laadt de tabellen en zet een dagelijkse back-up aan. Aan het eind toont het **één keer** de connection string met het wachtwoord. Bewaar die in jullie wachtwoordkluis.

---

## 2. Website op de webserver (172.20.66.40) koppelen

De website leest de connection string uit de environment variable **`ConnectionStrings__Unitas`** (twee underscores). Zet hem **niet** in `appsettings.json` of in git.

Voorbeeld met systemd: maak `/etc/unitas/unitas.env` aan (alleen leesbaar voor root):

```bash
sudo install -d -m 700 /etc/unitas
sudo sh -c 'echo "ConnectionStrings__Unitas=Server=172.20.66.35;Database=unitas;User=unitas_web;Password=<WACHTWOORD>;SslMode=Required" > /etc/unitas/unitas.env'
sudo chmod 600 /etc/unitas/unitas.env
```

En in de service-unit van de website: `EnvironmentFile=/etc/unitas/unitas.env`

Test vanaf de webserver: `nc -zv 172.20.66.35 3306` moet *succeeded* geven.

---

## 3. Lokaal ontwikkelen (op je eigen laptop)

De website start niet zonder database. Gebruik lokaal een test-database in Docker.

**Linux / macOS** (met Docker), vanuit de hoofdmap van de repository:
```bash
bash db/dev-database.sh
cd "Project Unitas" && dotnet run
```

**Windows** (met Docker Desktop), in PowerShell vanuit de hoofdmap:
```powershell
docker run -d --name unitas-mysql -e MYSQL_ROOT_PASSWORD=<kies-een-wachtwoord> -p 127.0.0.1:3306:3306 -v unitas-mysql-data:/var/lib/mysql mysql:8.4
# even wachten tot MySQL draait (~20 sec), dan:
docker exec -i unitas-mysql mysql -uroot -p<root-wachtwoord> -e "CREATE DATABASE unitas; CREATE USER 'unitas_web'@'%' IDENTIFIED BY '<web-wachtwoord>' REQUIRE SSL; GRANT SELECT, INSERT, UPDATE, DELETE ON unitas.* TO 'unitas_web'@'%';"
Get-Content db/unitas-schema.sql | docker exec -i unitas-mysql mysql -uroot -p<root-wachtwoord> unitas
dotnet user-secrets set "ConnectionStrings:Unitas" "Server=127.0.0.1;Database=unitas;User=unitas_web;Password=<web-wachtwoord>;SslMode=Required" --project "Project Unitas"
```

`dotnet user-secrets` bewaart het wachtwoord in je gebruikersprofiel, buiten de repository, dus het komt nooit in git.

---

## 4. Database wijzigen (migrations)

1. Pas de C#-modellen aan (map `Project Unitas/Models` of `Data/UnitasDbContext.cs`).
2. Maak een migration (vanuit `Project Unitas/`):
   ```bash
   dotnet tool restore
   dotnet ef migrations add <NaamVanDeWijziging> --output-dir Data/Migrations
   ```
3. Maak een SQL-script van **alleen** de nieuwe wijziging en voer het uit op de server:
   ```bash
   dotnet ef migrations script <VorigeMigration> <NaamVanDeWijziging> -o ../db/<NaamVanDeWijziging>.sql
   # op de server:  sudo mysql unitas < <NaamVanDeWijziging>.sql
   ```
   Werk ook `unitas-schema.sql` bij (`dotnet ef migrations script -o ../db/unitas-schema.sql`, en verwijder de onzichtbare BOM aan het begin), zodat een nieuwe server meteen alles krijgt.

> De website zelf mag de tabelstructuur **niet** wijzigen (least privilege). Migrations voer je dus uit als beheerder op de server, niet vanuit de website.

---

## Security-maatregelen (voor het securityrapport)

| Maatregel | Hoe |
|---|---|
| Least privilege | `unitas_web` mag alleen SELECT/INSERT/UPDATE/DELETE op `unitas`; geen DROP/CREATE/ALTER |
| Toegang beperkt | `unitas_web` mag alleen inloggen vanaf 172.20.66.40; MySQL luistert alleen op .35; Proxmox-firewall laat alleen .40 toe op 3306 |
| Versleuteling | `require_secure_transport = ON`: verbindingen zonder TLS worden geweigerd |
| Wachtwoorden | Klantwachtwoorden als hash (PBKDF2, ASP.NET Core Identity); database-wachtwoord niet in git (user-secrets / environment variable) |
| Sterk wachtwoordbeleid | Min. 12 tekens, hoofdletter, kleine letter, cijfer, speciaal teken |
| MFA | Verplicht voor klantaccounts (authenticator-app + 10 herstelcodes) |
| Brute force | Account 15 minuten geblokkeerd na 5 mislukte pogingen |
| Logging | Mislukte logins, blokkeringen, MFA-gebruik en nieuwe accounts worden gelogd |
| Back-up | Dagelijkse `mysqldump` op de server, 14 dagen bewaard |

**Nog te doen:**
- Back-ups ook **buiten** de databaseserver bewaren (en een keer terugzetten testen).
- De logs van de website centraal verzamelen (syslog/monitoring).
- Let op: de MFA-sleutels en herstelcodes staan (standaard bij ASP.NET Identity) leesbaar in `AspNetUserTokens`. Bescherm daarom de database en de back-ups goed: back-ups zijn net zo gevoelig als de database zelf.
