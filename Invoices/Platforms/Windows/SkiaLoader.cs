using System.Runtime.InteropServices;

namespace Invoices;

static class SkiaLoader
{
    public static void Load()
    {
        string root;
        string libPath;
        
        if (OperatingSystem.IsWindows())
        {
            root = AppContext.BaseDirectory;
            libPath = "QuestPdfSkia.dll";
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
        
        var resourcesDir = Path.Combine(root, "Resources", "Raw");
        var skiaLibPath = Path.Combine(resourcesDir, libPath);
        NativeLibrary.Load(skiaLibPath);
    }

}