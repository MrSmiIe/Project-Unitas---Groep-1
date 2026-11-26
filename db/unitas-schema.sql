CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `AspNetRoles` (
    `Id` varchar(255) NOT NULL,
    `Name` varchar(256) NULL,
    `NormalizedName` varchar(256) NULL,
    `ConcurrencyStamp` longtext NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetUsers` (
    `Id` varchar(255) NOT NULL,
    `FullName` varchar(100) NOT NULL,
    `CompanyName` varchar(100) NOT NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `UserName` varchar(256) NULL,
    `NormalizedUserName` varchar(256) NULL,
    `Email` varchar(256) NULL,
    `NormalizedEmail` varchar(256) NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext NULL,
    `SecurityStamp` longtext NULL,
    `ConcurrencyStamp` longtext NULL,
    `PhoneNumber` longtext NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `LockoutEnd` datetime NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `ContactMessages` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NOT NULL,
    `Company` varchar(100) NULL,
    `Email` varchar(254) NOT NULL,
    `Subject` varchar(50) NOT NULL,
    `Message` varchar(2000) NOT NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `Handled` tinyint(1) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Machines` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NOT NULL,
    `Category` varchar(50) NOT NULL,
    `Summary` varchar(200) NOT NULL,
    `Description` varchar(2000) NOT NULL,
    `Price` decimal(10,2) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` varchar(255) NOT NULL,
    `ClaimType` longtext NULL,
    `ClaimValue` longtext NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` varchar(255) NOT NULL,
    `ClaimType` longtext NULL,
    `ClaimValue` longtext NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(255) NOT NULL,
    `ProviderKey` varchar(255) NOT NULL,
    `ProviderDisplayName` longtext NULL,
    `UserId` varchar(255) NOT NULL,
    PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserRoles` (
    `UserId` varchar(255) NOT NULL,
    `RoleId` varchar(255) NOT NULL,
    PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserTokens` (
    `UserId` varchar(255) NOT NULL,
    `LoginProvider` varchar(255) NOT NULL,
    `Name` varchar(255) NOT NULL,
    `Value` longtext NULL,
    PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `MachineSpecs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `MachineId` int NOT NULL,
    `Label` varchar(60) NOT NULL,
    `Value` varchar(100) NOT NULL,
    `SortOrder` int NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MachineSpecs_Machines_MachineId` FOREIGN KEY (`MachineId`) REFERENCES `Machines` (`Id`) ON DELETE CASCADE
);

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (1, 'Deegbereiding', 'De SK-80 kneedt tot 80 kg deeg per keer met twee snelheden en een omkeerbare kuip. Geschikt voor brood-, pizza- en zoetdeeg.', 'Spiraalkneder SK-80', 18950.0, 'Krachtige spiraalkneder voor grote hoeveelheden deeg.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (2, 'Deegverwerking', 'De DV-30 verdeelt een deegklomp in 30 gelijke stukken per cyclus. Het gewicht is traploos instelbaar.', 'Deegverdeler DV-30', 12400.0, 'Verdeelt deeg in gelijke stukken van 50 tot 1000 gram.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (3, 'Deegverwerking', 'De OB-2000 maakt van verdeelde deegstukken mooie, gelijkmatige bollen. Ideaal in combinatie met de DV-30.', 'Opboller OB-2000', 9750.0, 'Bolt tot 2000 deegstukken per uur rond op.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (4, 'Deegverwerking', 'De LM-600 rolt en vormt deeg tot stokbroden, puntjes en tijgerbroden. De walsafstand is per product in te stellen.', 'Langmaker LM-600', 8300.0, 'Rolt deegbollen uit tot langwerpige broden.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (5, 'Rijzen', 'De RK-20 biedt plaats aan 20 bakplaten. Temperatuur en luchtvochtigheid worden automatisch op peil gehouden.', 'Rijskast RK-20', 6900.0, 'Rijskast met regeling van temperatuur en luchtvochtigheid.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (6, 'Bakken', 'De DO-4 heeft vier onafhankelijk regelbare dekken met stenen vloer, voor een krokante korst. Inclusief stoominstallatie.', 'Dekoven DO-4', 34500.0, 'Dekoven met vier stenen bakvloeren.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (7, 'Bakken', 'In de RO-18 draait een volledig rek met 18 platen rond, zodat alles even bruin wordt. Gas- en elektrische uitvoering.', 'Rekoven RO-18', 27800.0, 'Roterende rekoven voor een gelijkmatig bakresultaat.');
SELECT ROW_COUNT();

INSERT INTO `Machines` (`Id`, `Category`, `Description`, `Name`, `Price`, `Summary`)
VALUES (8, 'Afwerking', 'De BS-11 snijdt tot 600 broden per uur. Veilig door een afgeschermd mes en noodstop.', 'Broodsnijmachine BS-11', 5450.0, 'Snijdt broden in gelijke plakken van 11 mm.');
SELECT ROW_COUNT();


INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (1, 'Capaciteit', 1, 0, '80 kg deeg');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (2, 'Snelheden', 1, 1, '2 + omkeerbaar');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (3, 'Vermogen', 1, 2, '7,5 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (4, 'Afmetingen (b × d × h)', 1, 3, '75 × 125 × 140 cm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (5, 'Stukken per cyclus', 2, 0, '30');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (6, 'Gewicht per stuk', 2, 1, '50 – 1000 g');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (7, 'Vermogen', 2, 2, '1,5 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (8, 'Afmetingen (b × d × h)', 2, 3, '60 × 70 × 130 cm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (9, 'Capaciteit', 3, 0, '2000 stuks per uur');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (10, 'Gewicht per stuk', 3, 1, '30 – 1200 g');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (11, 'Vermogen', 3, 2, '0,75 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (12, 'Afmetingen (b × d × h)', 3, 3, '65 × 65 × 120 cm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (13, 'Walsbreedte', 4, 0, '600 mm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (14, 'Capaciteit', 4, 1, '1500 stuks per uur');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (15, 'Vermogen', 4, 2, '0,55 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (16, 'Afmetingen (b × d × h)', 4, 3, '90 × 80 × 125 cm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (17, 'Capaciteit', 5, 0, '20 platen (60 × 40 cm)');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (18, 'Temperatuur', 5, 1, '20 – 45 °C');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (19, 'Vermogen', 5, 2, '3 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (20, 'Afmetingen (b × d × h)', 5, 3, '80 × 95 × 205 cm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (21, 'Dekken', 6, 0, '4');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (22, 'Bakoppervlak', 6, 1, '9,6 m²');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (23, 'Max. temperatuur', 6, 2, '300 °C');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (24, 'Vermogen', 6, 3, '36 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (25, 'Capaciteit', 7, 0, '18 platen (60 × 80 cm)');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (26, 'Max. temperatuur', 7, 1, '280 °C');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (27, 'Vermogen', 7, 2, '45 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (28, 'Afmetingen (b × d × h)', 7, 3, '140 × 200 × 240 cm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (29, 'Snijdikte', 8, 0, '11 mm');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (30, 'Capaciteit', 8, 1, '600 broden per uur');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (31, 'Vermogen', 8, 2, '0,37 kW');
SELECT ROW_COUNT();

INSERT INTO `MachineSpecs` (`Id`, `Label`, `MachineId`, `SortOrder`, `Value`)
VALUES (32, 'Afmetingen (b × d × h)', 8, 3, '70 × 60 × 85 cm');
SELECT ROW_COUNT();


CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE INDEX `IX_ContactMessages_CreatedAtUtc` ON `ContactMessages` (`CreatedAtUtc`);

CREATE INDEX `IX_Machines_Category` ON `Machines` (`Category`);

CREATE INDEX `IX_MachineSpecs_MachineId` ON `MachineSpecs` (`MachineId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261126120226_InitialCreate', '10.0.14');

COMMIT;

