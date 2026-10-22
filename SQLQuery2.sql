CREATE TABLE [dbo].[Tilmelding]
(
    [TilmeldingId] INT IDENTITY(1,1) NOT NULL,
    [AktivitetsId] INT NOT NULL,
    [SpejderId] INT NOT NULL,
    [Tilmeldingsdato] DATE NOT NULL
        DEFAULT CAST(GETDATE() AS DATE),

    CONSTRAINT PK_Tilmelding
        PRIMARY KEY ([TilmeldingId]),

    CONSTRAINT FK_Tilmelding_Aktivitet
        FOREIGN KEY ([AktivitetsId])
        REFERENCES [dbo].[Aktivitet]([AktivitetsId]),

    CONSTRAINT FK_Tilmelding_Spejder
        FOREIGN KEY ([SpejderId])
        REFERENCES [dbo].[Spejder]([SpejderId]),

    CONSTRAINT UQ_Tilmelding
        UNIQUE ([AktivitetsId], [SpejderId])
);
