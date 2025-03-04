using System.Runtime.InteropServices;
using Foundation;

namespace Invoices;

static class MacSkiaLoader
{
    public static void Load()
    {
        string root;
        string libPath;
        
        if (OperatingSystem.IsMacCatalyst())
        {
            root = NSBundle.MainBundle.BundlePath;
            libPath = "libQuestPdfSkia.dylib";
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
        
        var resourcesDir = Path.Combine(root, "Contents", "Resources", "Raw");
        var skiaLibPath = Path.Combine(resourcesDir, libPath);
        NativeLibrary.Load(skiaLibPath);
    }

}