using Microsoft.Data.SqlClient;

namespace DataAccess.Context {
    internal class DatabaseVersionHelper {
        internal static int GetHighestDatabaseVersion(Database context) {
            try {
                var current = context.Versions
                    .OrderByDescending(v => v.ReleaseDate)
                    .FirstOrDefault();

                return current == null ? 0 : current.VersionNo;
            }
            catch (SqlException ex) when (ex.Number == 208) {
                return 0;
            }
            catch (Exception) {
                throw;
            }
        }
    }
}