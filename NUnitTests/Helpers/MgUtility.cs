
namespace NUnitTests.Helpers
{
  public static class MgUtility
  {
    public static string? FindProjectRoot()
    {
      string? currentDirectory = AppContext.BaseDirectory;
      while (currentDirectory != null)
      {
        // Search for a .csproj file in the current directory
        string[] files = Directory.GetFiles(currentDirectory, "*.csproj");
        if (files.Length > 0)
        {
          return currentDirectory;
        }

        // Move up to the parent directory
        currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
      }
      return null; // Project root not found
    }
  }
}
