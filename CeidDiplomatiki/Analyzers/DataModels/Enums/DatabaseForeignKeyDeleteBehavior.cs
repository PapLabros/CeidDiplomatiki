namespace CeidDiplomatiki
{
    /// <summary>
    /// Provides enumeration for the available delete behaviors of a foreign key
    /// </summary>
    public enum DatabaseForeignKeyDeleteBehavior
    {
        NoAction,
        Cascade,
        Restrict,
        SetNull
    }
}
