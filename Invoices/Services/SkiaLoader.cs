using System.Runtime.InteropServices;
using Foundation;

namespace Invoices.Services;

static class SkiaLoader
{
    public static void Load()
    {
        string root = AppContext.BaseDirectory;
        string libPath = "QuestPdfSkia.dll";
        
        #if MACCATALYST
            if (OperatingSystem.IsMacCatalyst())
            {
                root = NSBundle.MainBundle.BundlePath;
                libPath = "libQuestPdfSkia.dylib";
            }
        #endif
        
        var resourcesDir = Path.Combine(root, "Contents", "Resources", "Raw");
        var skiaLibPath = Path.Combine(resourcesDir, libPath);
        NativeLibrary.Load(skiaLibPath);
    }

}