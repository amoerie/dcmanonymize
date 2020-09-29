namespace DcmOrganize
{
    public class FolderNameCleaner
    {
        private static readonly char[] CharsToRemove = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        private static readonly char[] CharsToTrim = { '.', ' ' };

        public static string Clean(string folderName)
        {
            return string.Join("", folderName.Split(CharsToRemove)).Trim(CharsToTrim);
        }
    }
}