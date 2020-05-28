PRAGMA foreign_keys=off;

BEGIN TRANSACTION;

ALTER TABLE Folders RENAME TO OldFolders; 
ALTER TABLE Discs RENAME TO OldDiscs; 
ALTER TABLE Artists RENAME TO OldArtists; 
ALTER TABLE Genres RENAME TO OldGenres; 
ALTER TABLE Songs RENAME TO OldSongs; 
ALTER TABLE Playbacks RENAME TO OldPlaybacks; 
ALTER TABLE DiscImages RENAME TO OldDiscImages; 

CREATE TABLE [Folders] (
  [Id] INTEGER NOT NULL,
  [ParentFolder_Id] INTEGER NULL,
  [Name] ntext NOT NULL,
  [DeleteDate] datetime NULL,
  CONSTRAINT [sqlite_master_PK_Folders] PRIMARY KEY ([Id]),
  FOREIGN KEY ([ParentFolder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Discs] (
  [Id] INTEGER NOT NULL,
  [Folder_Id] INTEGER NOT NULL,
  [Title] ntext NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [AlbumTitle] ntext NULL,
  CONSTRAINT [sqlite_master_PK_Discs] PRIMARY KEY ([Id]),
  FOREIGN KEY ([Folder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Artists] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Artists] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Artists] UNIQUE (Name)
);

CREATE TABLE [Genres] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Genres] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Genres] UNIQUE (Name)
);

CREATE TABLE [Songs] (
  [Id] INTEGER NOT NULL,
  [Artist_Id] int NULL,
  [Disc_Id] int NOT NULL,
  [Genre_Id] int NULL,
  [TrackNumber] smallint NULL,
  [Year] smallint NULL,
  [Title] ntext NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [Duration] float NOT NULL,
  [Rating] int NULL,
  [FileSize] int NULL,
  [Checksum] int NULL,
  [Bitrate] int NULL,
  [LastPlaybackTime] datetime NULL,
  [PlaybacksCount] int NOT NULL,
  [DeleteDate] datetime NULL,
  CONSTRAINT [sqlite_master_PK_Songs] PRIMARY KEY ([Id]),
  FOREIGN KEY ([Genre_Id]) REFERENCES [Genres] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION,
  FOREIGN KEY ([Artist_Id]) REFERENCES [Artists] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Playbacks] (
  [Id] INTEGER NOT NULL,
  [Song_Id] int NOT NULL,
  [PlaybackTime] datetime NOT NULL,
  CONSTRAINT [sqlite_master_PK_Playbacks] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Playbacks] UNIQUE (PlaybackTime, Song_Id),
  FOREIGN KEY ([Song_Id]) REFERENCES [Songs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE [DiscImages] (
  [Id] INTEGER NOT NULL,
  [Disc_Id] int NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [ImageType] int NOT NULL,
  [FileSize] int NOT NULL,
  [Checksum] int NOT NULL,
  CONSTRAINT [sqlite_master_PK_Discs] PRIMARY KEY ([Id]),
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
);

INSERT INTO Folders(Id, ParentFolder_Id, Name, DeleteDate) SELECT Id, ParentFolder_Id, Name, DeleteDate FROM OldFolders;
INSERT INTO Discs(Id, Folder_Id, Title, TreeTitle, AlbumTitle) SELECT Id, Folder_Id, Title, TreeTitle, AlbumTitle FROM OldDiscs;
INSERT INTO Artists(Id, Name) SELECT Id, Name FROM OldArtists;
INSERT INTO Genres(Id, Name) SELECT Id, Name FROM OldGenres;
INSERT INTO Songs(Id, Artist_Id, Disc_Id, Genre_Id, TrackNumber, Year, Title, TreeTitle, Duration, Rating, FileSize, Checksum, Bitrate, LastPlaybackTime, PlaybacksCount, DeleteDate)
           SELECT Id, Artist_Id, Disc_Id, Genre_Id, TrackNumber, Year, Title, TreeTitle, Duration, Rating, FileSize, Checksum, Bitrate, LastPlaybackTime, PlaybacksCount, DeleteDate FROM OldSongs;
INSERT INTO Playbacks(Id, Song_Id, PlaybackTime)
               SELECT Id, Song_Id, PlaybackTime FROM OldPlaybacks;
INSERT INTO DiscImages(Id, Disc_Id, TreeTitle, ImageType, FileSize, Checksum)
                SELECT Id, Disc_Id, TreeTitle, ImageType, FileSize, Checksum FROM OldDiscImages;

DROP TABLE OldFolders;
DROP TABLE OldDiscs;
DROP TABLE OldArtists;
DROP TABLE OldGenres;
DROP TABLE OldSongs;
DROP TABLE OldPlaybacks;
DROP TABLE OldDiscImages;

UPDATE Songs SET FileSize = NULL, Checksum = NULL WHERE DeleteDate IS NOT NULL;

COMMIT;

PRAGMA foreign_keys=on;

CREATE INDEX [IX_Genre_Id] ON [Songs] ([Genre_Id] ASC);
CREATE INDEX [IX_Disc_Id] ON [Songs] ([Disc_Id] ASC);
CREATE INDEX [IX_Artist_Id] ON [Songs] ([Artist_Id] ASC);
CREATE INDEX [IX_Song_Id] ON [Playbacks] ([Song_Id] ASC);
CREATE INDEX [IX_Folder_ParentFolder_Id] ON [Folders] ([ParentFolder_Id] ASC);
CREATE INDEX [IX_Disc_Folder_Id] ON [Discs] ([Folder_Id] ASC);
