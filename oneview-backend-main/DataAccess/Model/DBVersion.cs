namespace DataAccess.Model
{
    internal class DBVersion
    {
        public int Id { get; set; }
        public int VersionNo { get; set; }
        public required string IssueID { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}