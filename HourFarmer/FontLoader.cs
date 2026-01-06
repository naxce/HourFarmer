using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using static System.Runtime.InteropServices.Marshal;

namespace FatumStyles
{
    public static class FontLoader
    {
        private static PrivateFontCollection _fontCollection = new PrivateFontCollection();
        private static readonly List<IntPtr> _gdiHandles = new List<IntPtr>();
        private static readonly List<string> _loadedFonts = new List<string>();

        private const string EmbeddedResourceName1 = "HourFarmer.Fonts.Poppins-Regular.ttf";
        private const string EmbeddedResourceName2 = "HourFarmer.Fonts.Poppins-Bold.ttf";

        private const uint GMEM_MOVEABLE = 0x0002;
        private const uint GMEM_ZEROINIT = 0x0040;
        private const uint MEMORY_ALLOC_FLAGS = GMEM_MOVEABLE | GMEM_ZEROINIT;

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern bool RemoveFontMemResourceEx(IntPtr fh);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        private static void _Internal_LoadSingleResource(string fontResource)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            using (Stream fontStream = currentAssembly.GetManifestResourceStream(fontResource))
            {
                if (fontStream == null)
                {
                    Console.WriteLine($"[ERR] Resource not found: {fontResource}.");
                    return;
                }

                byte[] fontData = new byte[fontStream.Length];
                fontStream.Read(fontData, 0, (int)fontStream.Length);

                IntPtr hGlobalMemory = GlobalAlloc(MEMORY_ALLOC_FLAGS, (UIntPtr)fontData.Length);

                if (hGlobalMemory == IntPtr.Zero)
                {
                    throw new OutOfMemoryException($"Allocation error: {fontResource}.");
                }

                IntPtr lpGlobalLock = GlobalLock(hGlobalMemory);

                if (lpGlobalLock == IntPtr.Zero)
                {
                    GlobalFree(hGlobalMemory);
                    throw new ApplicationException($"Locking error: {fontResource}.");
                }

                Copy(fontData, 0, lpGlobalLock, fontData.Length);

                GlobalUnlock(hGlobalMemory);

                uint pcf = 0;
                IntPtr fontResourceHandle = AddFontMemResourceEx(lpGlobalLock, (uint)fontData.Length, IntPtr.Zero, ref pcf);

                if (fontResourceHandle == IntPtr.Zero)
                {
                    GlobalFree(hGlobalMemory);
                    throw new InvalidOperationException($"GDI error: {fontResource}.");
                }

                _gdiHandles.Add(fontResourceHandle);
                _fontCollection.AddMemoryFont(lpGlobalLock, fontData.Length);
                _loadedFonts.Add(fontResource);

                GlobalFree(hGlobalMemory);
            }
        }

        public static void LoadCustomFont()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string[] actualResources = currentAssembly.GetManifestResourceNames();

            Console.WriteLine("--- Actual Embedded Resources ---");
            foreach (string res in actualResources)
            {
                Console.WriteLine(res);
            }
            Console.WriteLine("---------------------------------");
            try
            {
                _Internal_LoadSingleResource(EmbeddedResourceName1);
                _Internal_LoadSingleResource(EmbeddedResourceName2);

                if (_fontCollection.Families.Length == 0)
                {
                    Console.WriteLine("[WARN] No font families loaded. Check resource names.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] Loading error: {ex.Message}");
            }
        }

        public static Font GetFont(float size, FontStyle style = FontStyle.Regular)
        {
            if (_fontCollection.Families.Length > 0)
            {
                return new Font(_fontCollection.Families.First(), size, style);
            }

            return new Font("Segoe UI", size, style);
        }

        public static void ClearResource()
        {
            foreach (IntPtr handle in _gdiHandles)
            {
                RemoveFontMemResourceEx(handle);
            }
            _gdiHandles.Clear();
            _fontCollection.Dispose();
        }
    }
}