///
/// The latest version of SleepyUnityTools can be found at 
/// https://github.com/sleepyparadox/SleepyUnityTools
///
/// The MIT License (MIT)
///
/// Copyright (c) 2015 Don Logan (sleepyparadox)
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:

/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.

/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sleepy.UnityTools
{
    public static class AssetGenerator
    {
        static string[] invalidChars = new string[]
        {
            ".", " ", "-","(",")", "!",
        };
#if UNITY_EDITOR || (!UNITY_WEBPLAYER && !UNITY_ANDROID && !UNITY_IPHONE)
        public static void GenerateAssetCodeFile(string assetsDirectory, string outputFile, string outputClassName)
        {
            var outFile = new FileInfo(outputFile);
            if (!Directory.Exists(outFile.Directory.FullName))
                Directory.CreateDirectory(outFile.Directory.FullName);

            using (var writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("///");
                writer.WriteLine("/// This is a generated code file");
                writer.WriteLine("/// Expect to lose any changes you make");
                writer.WriteLine("///");
                
                List<string> dirStack = new List<string>();
                var resourcesPath = assetsDirectory + "/Resources/";
                if (!Directory.Exists(resourcesPath))
                    Directory.CreateDirectory(resourcesPath);

                var resourcesDir = new DirectoryInfo(resourcesPath);
                resourcesPath = resourcesDir.FullName; //Step to disk based path
                var tab = 0;

                var extensions = new Dictionary<string, string>()
                {
                    {".prefab", typeof(PrefabAsset).Name},
                    {".mat", typeof(MaterialAsset).Name},
                    {".png", typeof(Texture2dAsset).Name},
                    {".PNG", typeof(Texture2dAsset).Name},
                    {".wav", typeof(AudioClipAsset).Name},
                };
                writer.WriteLine("using Sleepy.UnityTools;");
                HandleDirectory(resourcesDir, writer, ref tab, resourcesPath, extensions, outputClassName);
                writer.Close();
            }
            //System.Diagnostics.Process.Start(outFile.FullName);
        }

        private static void HandleDirectory(DirectoryInfo dir, StreamWriter writer, ref int tab, string resourcesPath, Dictionary<string, string> extensions, string outputNamespace)
        {
            var files = dir.GetFiles();
            files = files.Where(f => extensions.ContainsKey(f.Extension)).ToArray();

            var safeDirName = dir.Name;
            foreach (var invalidChar in invalidChars)
                safeDirName = safeDirName.Replace(invalidChar, "_");

            if (safeDirName == "Resources")
                safeDirName = outputNamespace;

            //if (files.Length > 0)
                writer.WriteLine(string.Format("{0}public class {1}", GetIncrement(tab, "   "), safeDirName));
            //else
            //    writer.WriteLine(string.Format("{0}namespace {1}", GetIncrement(tab, "   "), safeDirName));

            writer.WriteLine(GetIncrement(tab, "   ") + "{");

            tab++;

            if (files.Length > 0)
            {
                var assetsInDir = new List<string>();
                foreach (var file in files)
                {
                    string assetType = extensions[file.Extension];
                    var safeFileName = file.Name.Replace(file.Extension, string.Empty);
                    foreach (var invalidChar in invalidChars)
                        safeFileName = safeFileName.Replace(invalidChar, "_");
                    
                    //Allow files of same name with diff type to exist
                    safeFileName += assetType.Replace("Asset", string.Empty);

                    if (assetsInDir.Contains(safeFileName))
                        throw new Exception("Class safe asset name already in use " + safeFileName + " is a classname safe version of " + file.FullName + ", \".\" and \" \" are replaced by _ check for similarly named assets of files");
                    assetsInDir.Add(safeFileName);

                    var resourcePath = file.FullName.Replace(resourcesPath, string.Empty).Replace('\\', '/').Replace(file.Extension, string.Empty);
                    writer.WriteLine(string.Format("{0}public readonly static {1} {2} = new {3}(@\"{4}\");",
                        GetIncrement(tab, "   "),
                        assetType,
                        safeFileName,
                        assetType,
                        resourcePath));
                }

                var getAssetsLine = GetIncrement(tab, "   ") + "public static Asset[] GetAssets() { return new Asset []{ ";
                //getAssetsLine += string.Join(", ", assetsInDir); //thanks unity . . .
                for(var i = 0; i < assetsInDir.Count; ++i)
                {
                    if(i != 0)
                        getAssetsLine += ", ";
                    getAssetsLine += assetsInDir[i];
                }

                getAssetsLine += " }; }";
                writer.WriteLine(getAssetsLine);

            }

            foreach (var child in dir.GetDirectories())
            {
                HandleDirectory(child, writer, ref tab, resourcesPath, extensions, outputNamespace);
            }

            tab--;

            writer.WriteLine(GetIncrement(tab, "   ") + "}");

        }

        private static string GetIncrement(int tabs, string tabFormat)
        {
            var s = string.Empty;
            for (var i = 0; i < tabs; ++i)
                s += tabFormat;
            return s;
        }
#endif //#if UNITY_EDITOR || (!UNITY_WEBPLAYER && !UNITY_ANDROID && !UNITY_IPHONE)
    }
}
