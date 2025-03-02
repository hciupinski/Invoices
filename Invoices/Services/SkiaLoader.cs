using System.Runtime.InteropServices;
using Foundation;

namespace Invoices.Services;

static class SkiaLoader
{
    public static void Load()
    {
        if (OperatingSystem.IsMacCatalyst())
        {
            var root = NSBundle.MainBundle.BundlePath;
            var aa = Path.Combine(root, "Contents", "Resources");
            var skiaLibPath = Path.Combine(aa, "Raw",  "libQuestPdfSkia.dylib");
            NativeLibrary.Load(skiaLibPath);
        }
    }
}