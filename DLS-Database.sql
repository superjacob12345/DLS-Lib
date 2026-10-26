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

-- Eksempeldata indsættes af programmet første gang det startes (se Program.cs)
