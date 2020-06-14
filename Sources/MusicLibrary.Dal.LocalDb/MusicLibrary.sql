CREATE TABLE [Folders] (
  [Id] INTEGER NOT NULL,
  [ParentFolder_Id] INTEGER NULL,
  [Name] ntext NOT NULL,
  [DeleteDate] datetime NULL,
  CONSTRAINT [sqlite_master_PK_Folders] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Folders] UNIQUE ([ParentFolder_Id], [Name]),
  FOREIGN KEY ([ParentFolder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Discs] (
  [Id] INTEGER NOT NULL,
  [Folder_Id] INTEGER NOT NULL,
  [Year] int NULL,
  [Title] ntext NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [AlbumTitle] ntext NULL,
  CONSTRAINT [sqlite_master_PK_Discs] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Discs] UNIQUE ([Folder_Id], [TreeTitle]),
  FOREIGN KEY ([Folder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Artists] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Artists] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Artists] UNIQUE ([Name])
);

CREATE TABLE [Genres] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Genres] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Genres] UNIQUE ([Name])
);

CREATE TABLE [Songs] (
  [Id] INTEGER NOT NULL,
  [Artist_Id] int NULL,
  [Disc_Id] int NOT NULL,
  [Genre_Id] int NULL,
  [TrackNumber] smallint NULL,
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
  CONSTRAINT [sqlite_master_UC_Songs] UNIQUE ([Disc_Id], [TreeTitle]),
  FOREIGN KEY ([Genre_Id]) REFERENCES [Genres] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION,
  FOREIGN KEY ([Artist_Id]) REFERENCES [Artists] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Playbacks] (
  [Id] INTEGER NOT NULL,
  [Song_Id] int NOT NULL,
  [PlaybackTime] datetime NOT NULL,
  CONSTRAINT [sqlite_master_PK_Playbacks] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Playbacks] UNIQUE ([Song_Id], [PlaybackTime]),
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
  CONSTRAINT [sqlite_master_UC_DiscImages] UNIQUE ([Disc_Id], [TreeTitle]),
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE [SessionData] (
  [Key] ntext NOT NULL,
  [Data] ntext NULL,
  CONSTRAINT [sqlite_master_PK_SessionData] PRIMARY KEY ([Key])
);

CREATE INDEX [IX_Genre_Id] ON [Songs] ([Genre_Id] ASC);
CREATE INDEX [IX_Disc_Id] ON [Songs] ([Disc_Id] ASC);
CREATE INDEX [IX_Artist_Id] ON [Songs] ([Artist_Id] ASC);
CREATE INDEX [IX_Song_Id] ON [Playbacks] ([Song_Id] ASC);
CREATE INDEX [IX_Folder_ParentFolder_Id] ON [Folders] ([ParentFolder_Id] ASC);
CREATE INDEX [IX_Disc_Folder_Id] ON [Discs] ([Folder_Id] ASC);

-- Seeding required initial data
INSERT INTO [Folders]([Id], [Name], [ParentFolder_Id]) VALUES(1, '<ROOT>', NULL);
