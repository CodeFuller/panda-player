BEGIN TRANSACTION;

CREATE TABLE [AdviseSets] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_AdviseSets] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_AdviseSets] UNIQUE ([Name])
);

ALTER TABLE [Discs] ADD [AdviseSet_Id] REFERENCES [AdviseSets] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE [Discs] ADD [AdviseSetOrder] int NULL;

COMMIT;

VACUUM;
