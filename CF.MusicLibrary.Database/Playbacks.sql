﻿CREATE TABLE [dbo].[Playbacks]
(
    [Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY, 
    [SongId] INT NOT NULL, 
    [PlaybackTime] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_Playbacks_Songs] FOREIGN KEY (SongId) REFERENCES [Songs]([Id])
    ON DELETE CASCADE
    ON UPDATE CASCADE
)
