namespace CSXTests.TestsDataProvider;

public static class FilePathGenerator
{
    private static readonly char[] DriveLetters = "CDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly string[] FileExtensions = [".txt", ".log", ".csv", ".exe", ".dll", ".jpg", ".png", ".mp4", ".json", ".xml"];
    private static readonly string[] FolderNames = ["Documents", "Users", "Temp", "Data", "Logs", "Projects", "Reports", "Backups", "Music", "Videos"];
    private static readonly string[] FileNames = ["report", "config", "data", "log", "file", "backup", "image", "video", "index", "readme"];

    private static readonly Random RandomGen = new();

    public static string[] GenerateRandomWindowsPaths(int totalPaths)
    {
        var paths = new HashSet<string>();

        while (paths.Count < totalPaths)
        {
            string drive = GetRandomDrive();
            string fullPath = Path.Combine(drive, GenerateRandomFolderTree());

            if (RandomGen.Next(2) == 0) // 50% chance it's a folder, 50% a file
            {
                fullPath = Path.Combine(fullPath, GenerateRandomFileName());
            }

            paths.Add(fullPath);
        }

        return [.. paths];
    }

    private static string GetRandomDrive() => $"{DriveLetters[RandomGen.Next(DriveLetters.Length)]}:\\";

    private static string GenerateRandomFolderTree()
    {
        int depth = RandomGen.Next(1, 6); // Up to 5 levels deep
        List<string> folders = [];

        for (int i = 0; i < depth; i++)
        {
            folders.Add(FolderNames[RandomGen.Next(FolderNames.Length)]);
        }

        return Path.Combine([.. folders]);
    }

    private static string GenerateRandomFileName()
    {
        string name = FileNames[RandomGen.Next(FileNames.Length)];
        int version = RandomGen.Next(1, 100); // Add some variation
        string extension = FileExtensions[RandomGen.Next(FileExtensions.Length)];

        return $"{name}_{version}{extension}";
    }
}