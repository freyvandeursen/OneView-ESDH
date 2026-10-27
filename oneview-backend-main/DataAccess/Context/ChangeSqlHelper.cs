using System.Text.RegularExpressions;

namespace DataAccess.Context {
    internal class ChangeSqlHelper {
        internal static int GetHighestChangeNumber(string filePath) {
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException($"Could not find change.sql at {filePath}");
            }

            var text = File.ReadAllText(filePath);

            // Mathces lines like:  #120 eller #300
            var matches = Regex.Matches(text, @"#(\d{1,4})");
            return matches.Count == 0
                ? throw new InvalidOperationException("No version numbers (#xxx) found in change.sql.")
                : matches
                .Select(m => int.Parse(m.Groups[1].Value))
                .Max();
        }
    }
}