BEGIN TRANSACTION;

ALTER TABLE [Songs] ADD [DeleteComment] ntext NULL;

COMMIT;
