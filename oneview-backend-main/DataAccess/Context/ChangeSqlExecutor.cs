using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DataAccess.Context {
    internal class ChangeSqlExecutor {
        public static void ApplyPendingChanges(Database context, string changeSqlPath) {
            var dbVersion = DatabaseVersionHelper.GetHighestDatabaseVersion(context);
            var fileVersion = ChangeSqlHelper.GetHighestChangeNumber(changeSqlPath);

            Console.WriteLine($"📊 Database version: {dbVersion}, change.sql version: {fileVersion}");

            if (fileVersion <= dbVersion) {
                Console.WriteLine("✅ Database is already up-to-date.");
                return;
            }

            var sqlText = File.ReadAllText(changeSqlPath);
            var blocks = Regex.Split(sqlText, @"(?=#\d+)").Where(b => !string.IsNullOrWhiteSpace(b));

            foreach (var block in blocks) {
                var match = Regex.Match(block, @"#(\d+)");
                if (!match.Success) {
                    continue;
                }

                var blockVersion = int.Parse(match.Groups[1].Value);
                if (blockVersion <= dbVersion) {
                    continue;
                }

                // 🔍 Find IssueID in comment, is should look like Issue: ONEVIEW-123
                var issueMatch = Regex.Match(block, @"--\s*Issue(?:ID)?[:=]\s*([A-Za-z0-9\-_]+)",
                                             RegexOptions.IgnoreCase);
                var issueId = issueMatch.Success ? issueMatch.Groups[1].Value : "N/A";

                // Remove metadata before executing
                var blockSql = Regex.Replace(block, @"#\d+", "")
                                       .Replace(issueMatch.Value, "")
                                       .Trim();

                Console.WriteLine($"➡️ Executing block #{blockVersion} (Issue: {issueId})...");

                using var transaction = context.Database.BeginTransaction();
                try {
                    context.Database.ExecuteSqlRaw(blockSql);

                    context.Versions.Add(new DBVersion {
                        VersionNo = blockVersion,
                        IssueID = issueId,
                        ReleaseDate = DateTime.Now
                    });

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex) {
                    Console.WriteLine($"❌ Error at block #{blockVersion}: {ex.Message}");
                    transaction.Rollback();
                    throw;
                }
            }

            Console.WriteLine("🎉 Database successfully updated!");
        }
    }
}