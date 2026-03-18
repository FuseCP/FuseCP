public void SetupSqliteDb()
{
    // Existing code...
    InstallFreshDatabase();

    // Apply EF migrations to the SQLite test database
    Database.Migrate();
}