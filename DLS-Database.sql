-- Opretter databasen DLS med alle tabeller.
-- Kør hele scriptet i Visual Studio (SQL Server Object Explorer) eller SSMS mod (localdb)\MSSQLLocalDB.
-- OBS: Tabellerne slettes og laves forfra, så alle data forsvinder.

IF DB_ID('DLS') IS NULL
    CREATE DATABASE DLS;
GO

USE DLS;
GO

DROP TABLE IF EXISTS Tilmelding;
DROP TABLE IF EXISTS AktivitetTegn;
DROP TABLE IF EXISTS AktivitetLeder;
DROP TABLE IF EXISTS SpejderTegn;
DROP TABLE IF EXISTS Aktivitet;
DROP TABLE IF EXISTS Spejder;
DROP TABLE IF EXISTS Leder;
DROP TABLE IF EXISTS Tegn;
GO

CREATE TABLE Tegn
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Navn NVARCHAR(100) NOT NULL
);

CREATE TABLE Leder
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Navn NVARCHAR(100) NOT NULL,
    BrugerNavn NVARCHAR(50) NOT NULL UNIQUE,
    Adgangskode NVARCHAR(100) NOT NULL
);

CREATE TABLE Spejder
(
    SpejderId INT IDENTITY(1,1) PRIMARY KEY,
    Navn NVARCHAR(100) NOT NULL,
    BrugerNavn NVARCHAR(50) NOT NULL UNIQUE,
    Adgangskode NVARCHAR(100) NOT NULL,
    Foeselsdag DATE NOT NULL
);

CREATE TABLE Aktivitet
(
    AktivitetsId INT IDENTITY(1,1) PRIMARY KEY,
    Navn NVARCHAR(100) NOT NULL,
    Tidspunkt DATETIME2 NOT NULL,
    MaksDeltagere INT NOT NULL,
    MinAlder INT NOT NULL,
    Sted NVARCHAR(100) NOT NULL DEFAULT '',
    Beskrivelse NVARCHAR(1000) NOT NULL DEFAULT ''
);

-- Hvilke tegn en spejder har
CREATE TABLE SpejderTegn
(
    SpejderId INT NOT NULL REFERENCES Spejder(SpejderId) ON DELETE CASCADE,
    TegnId INT NOT NULL REFERENCES Tegn(Id) ON DELETE CASCADE,
    PRIMARY KEY (SpejderId, TegnId)
);

-- Hvilke ledere der står for en aktivitet
CREATE TABLE AktivitetLeder
(
    AktivitetsId INT NOT NULL REFERENCES Aktivitet(AktivitetsId) ON DELETE CASCADE,
    LederId INT NOT NULL REFERENCES Leder(Id) ON DELETE CASCADE,
    PRIMARY KEY (AktivitetsId, LederId)
);

-- Hvilke tegn man skal have for at deltage i en aktivitet
CREATE TABLE AktivitetTegn
(
    AktivitetsId INT NOT NULL REFERENCES Aktivitet(AktivitetsId) ON DELETE CASCADE,
    TegnId INT NOT NULL REFERENCES Tegn(Id) ON DELETE CASCADE,
    PRIMARY KEY (AktivitetsId, TegnId)
);

CREATE TABLE Tilmelding
(
    TilmeldingId INT IDENTITY(1,1) PRIMARY KEY,
    AktivitetsId INT NOT NULL REFERENCES Aktivitet(AktivitetsId) ON DELETE CASCADE,
    SpejderId INT NOT NULL REFERENCES Spejder(SpejderId) ON DELETE CASCADE,
    Tilmeldingsdato DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    CONSTRAINT UQ_Tilmelding UNIQUE (AktivitetsId, SpejderId)
);
GO

-- ===== Testdata =====
-- Alle brugere har adgangskoden 1234

INSERT INTO Tegn (Navn) VALUES
(N'Førstehjælp'),   -- 1
(N'Bålmester'),     -- 2
(N'Svømmer'),       -- 3
(N'Knob og reb'),   -- 4
(N'Kanosejler');    -- 5

INSERT INTO Leder (Navn, BrugerNavn, Adgangskode) VALUES
(N'Mette Hansen', 'mette', '1234'),  -- 1
(N'Jonas Berg', 'jonas', '1234'),    -- 2
(N'Aisha Khan', 'aisha', '1234');    -- 3

INSERT INTO Spejder (Navn, BrugerNavn, Adgangskode, Foeselsdag) VALUES
(N'Ida Larsen', 'ida', '1234', '2014-03-12'),        -- 1
(N'Oliver Nielsen', 'oliver', '1234', '2017-08-02'), -- 2
(N'Noah Pedersen', 'noah', '1234', '2011-01-20'),    -- 3
(N'Freja Madsen', 'freja', '1234', '2013-11-05');    -- 4

INSERT INTO SpejderTegn (SpejderId, TegnId) VALUES
(1, 2), (1, 3),
(3, 1), (3, 3), (3, 5),
(4, 4);

-- Lejren er i uge 27 2027
INSERT INTO Aktivitet (Navn, Tidspunkt, MaksDeltagere, MinAlder, Sted, Beskrivelse) VALUES
(N'Mad over bål', '2027-07-05 17:00', 12, 8, N'Bålpladsen', N'Vi laver snobrød og pandekager over åben ild. Medbring egen kop!'),
(N'Ridetur på Hedegården', '2027-07-06 10:00', 6, 10, N'Hedegården', N'Besøg gården, mød dyrene og tag en tur på hesteryg.'),
(N'Fang dyr i vandhullet', '2027-07-06 14:00', 15, 6, N'Vandhullet', N'Med net og spande undersøger vi hvad der lever i vandet.'),
(N'Kanotur på søen', '2027-07-07 09:30', 8, 12, N'Hedeland Sø', N'En hel formiddag på vandet. Kræver at man kan svømme og har kanotegnet.'),
(N'Byg en svævebane', '2027-07-08 13:00', 10, 11, N'Skoven', N'Vi bygger en svævebane mellem træerne med reb og knob.'),
(N'Natløb med lommelygter', '2027-07-08 21:00', 20, 11, N'Lejrens indgang', N'Find posterne i mørket og løs opgaverne undervejs.'),
(N'Lejrbål og sang', '2027-07-09 20:00', 200, 0, N'Det store bål', N'Ugens afslutning med sange og skumfiduser for hele lejren.');

INSERT INTO AktivitetLeder (AktivitetsId, LederId) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 2), (4, 3),
(5, 3),
(6, 1), (6, 2),
(7, 1), (7, 2), (7, 3);

INSERT INTO AktivitetTegn (AktivitetsId, TegnId) VALUES
(4, 3), (4, 5),
(5, 4);

INSERT INTO Tilmelding (SpejderId, AktivitetsId) VALUES
(1, 1),
(1, 3),
(3, 4),
(4, 5);
GO
