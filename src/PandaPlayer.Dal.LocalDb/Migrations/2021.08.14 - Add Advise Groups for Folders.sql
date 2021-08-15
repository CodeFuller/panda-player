BEGIN TRANSACTION;

CREATE TABLE [AdviseGroups] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_AdviseGroups] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_AdviseGroups] UNIQUE ([Name])
);

ALTER TABLE [Folders] ADD [AdviseGroup_Id] INTEGER NULL REFERENCES [AdviseGroups] ([Id]) ON DELETE SET NULL ON UPDATE NO ACTION;

COMMIT;

VACUUM;
