# Integration Tests for PandaPlayer

## Updating Test Database

Instructions for update of test database (TestDatabase.db):

1. Make the necessary changes to tests/PandaPlayer.Services.IntegrationTests/TestDatabase.sql

1. Create empty database file TestDatabase.db

1. Open it in SQLiteStudio

1. Execute src/PandaPlayer.Dal.LocalDb/CreateDatabase.sql

1. Execute tests/PandaPlayer.Services.IntegrationTests/TestDatabase.sql

1. Commit updated TestDatabase.db to Git.

## Details on Implementation if Integration Tests

1. We use cyryllic characters both in input test database (TestDatabase.db) and in tests code. We do so for proper check of handling of non-ASCII characters by DAL.

1. Class `ReferenceData` contains reference data, which matches the data in TestDatabase.db. Some tests modify reference data for check of update operations. That is why `ReferenceData` is not static and is created anew for each test.

1. Each test for update operation (create, update, delete) should contain a call to `CheckLibraryConsistency()` method. This method verifies consistency of the database.
