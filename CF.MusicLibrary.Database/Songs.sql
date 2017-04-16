CREATE TABLE [dbo].[Songs]
(
	[Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY, 
    [ArtistId] INT NOT NULL, 
    [DiscId] INT NOT NULL, 
    [OrderNumber] SMALLINT NOT NULL, 
    [Year] SMALLINT NULL, 
    [Title] NVARCHAR(MAX) NOT NULL, 
    [GenreId] INT NULL, 
    [Duration] INT NOT NULL,
    [Rating] TINYINT NULL, 
    [Uri] NVARCHAR(MAX) NOT NULL, 
    [FileSize] INT NOT NULL, 
    [Bitrate] INT NULL, 
    [LastPlaybackTime] DATETIME2 NULL, 
    [PlaybacksCount] INT NOT NULL, 
    CONSTRAINT [FK_Songs_Artists] FOREIGN KEY ([ArtistId]) REFERENCES [Artists]([Id])
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_Songs_Discs] FOREIGN KEY ([DiscId]) REFERENCES [Discs]([Id])
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_Songs_Genres] FOREIGN KEY ([GenreId]) REFERENCES [Genres]([Id])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
