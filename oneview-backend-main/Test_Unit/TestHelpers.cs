using DataAccess;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Test_Unit {
    internal class TestHelpers {
        public static async Task ResetDatabaseWithChangeSql(IServiceProvider provider) {
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Database>();

            // 1. Drop all foreign keys
            await db.Database.ExecuteSqlRawAsync(@"
            DECLARE @sql NVARCHAR(MAX) = N'';

            SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].['
                + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];'
            FROM sys.foreign_keys;

            EXEC(@sql);
        ");

            // 2. Drop all tables
            await db.Database.ExecuteSqlRawAsync(@"
            DECLARE @sql NVARCHAR(MAX) = N'';
            SELECT @sql += 'DROP TABLE [' + TABLE_SCHEMA + '].[' + TABLE_NAME + '];'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE';
            EXEC(@sql);
        ");

            // 3. Drop all views
            await db.Database.ExecuteSqlRawAsync(@"
            DECLARE @sql NVARCHAR(MAX) = N'';
            SELECT @sql += 'DROP VIEW [' + TABLE_SCHEMA + '].[' + TABLE_NAME + '];'
            FROM INFORMATION_SCHEMA.VIEWS;
            EXEC(@sql);
        ");

            // 4. Drop all triggers
            await db.Database.ExecuteSqlRawAsync(@"
            DECLARE @sql NVARCHAR(MAX) = N'';
            SELECT @sql += 'DROP TRIGGER [' + s.name + '].[' + o.name + '];'
            FROM sys.objects o
            JOIN sys.schemas s ON o.schema_id = s.schema_id
            WHERE o.type = 'TR';
            EXEC(@sql);
        ");

            // 5. Rebuild schema via change.sql
            DependencyInjection.ApplyDatabaseUpdates(provider);
        }
    }
}
