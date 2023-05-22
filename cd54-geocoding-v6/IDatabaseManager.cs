interface IDatabaseManager
{
    string Open(string database_name, string database_user, string database_password);
    Task<Dictionary<long, Geo>> LoadAddrFromDBAsync(string connection_string);
}