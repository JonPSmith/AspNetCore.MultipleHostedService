// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Test.DataLayer
{
    public static class SqliteServiceExtensions
    {
        public static SqliteConnection SetupSqliteInMemoryConnection()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
            return connection;
        }

        public static DbContextOptions<MyDbContext> GetSqliteOptions(this SqliteConnection connection)
        {
            var builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseSqlite(connection);
            return builder.Options;
        }
    }
}