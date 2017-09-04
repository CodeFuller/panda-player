CREATE TABLE [Songs] (
  [Id] INTEGER NOT NULL,
  [TrackNumber] smallint NULL,
  [Year] smallint NULL,
  [Title] ntext NOT NULL,
  [Duration] float NOT NULL,
  [Rating] int NULL,
  [Uri] ntext NOT NULL,
  [FileSize] int NOT NULL,
  [Bitrate] int NULL,
  [LastPlaybackTime] datetime NULL,
  [PlaybacksCount] int NOT NULL,
  [Artist_Id] int NULL,
  [Disc_Id] int NOT NULL,
  [Genre_Id] int NULL,
  CONSTRAINT [sqlite_master_PK_Songs] PRIMARY KEY ([Id]),
  FOREIGN KEY ([Genre_Id]) REFERENCES [Genres] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION,
  FOREIGN KEY ([Artist_Id]) REFERENCES [Artists] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Playbacks] (
  [Id] INTEGER NOT NULL,
  [PlaybackTime] datetime NOT NULL,
  [Song_Id] int NOT NULL,
  CONSTRAINT [sqlite_master_PK_Playbacks] PRIMARY KEY ([Id]),
  FOREIGN KEY ([Song_Id]) REFERENCES [Songs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE [Genres] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Genres] PRIMARY KEY ([Id])
);

CREATE TABLE [Discs] (
  [Id] INTEGER NOT NULL,
  [Year] int NULL,
  [Title] ntext NOT NULL,
  [AlbumTitle] ntext NULL,
  [Uri] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Discs] PRIMARY KEY ([Id])
);

CREATE TABLE [Artists] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Artists] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_Genre_Id] ON [Songs] ([Genre_Id] ASC);
CREATE INDEX [IX_Disc_Id] ON [Songs] ([Disc_Id] ASC);
CREATE INDEX [IX_Artist_Id] ON [Songs] ([Artist_Id] ASC);
CREATE INDEX [IX_Song_Id] ON [Playbacks] ([Song_Id] ASC);
