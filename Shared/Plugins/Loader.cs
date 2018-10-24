#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Shared.Helpers;

namespace Shared.Plugins
{
    public static class Loader
    {
        public static void LoadPlugins()
        {
            Logger.L.Info("Start loading plugins!");
            if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");

            foreach (string f in Directory.GetFiles("Plugins")) // Press F for File
            {
                Assembly file = Assembly.LoadFrom(f);
                Stream   fs   = file.GetManifestResourceStream($"{file.GetName().Name}.plugin.xml");
                if (fs == null) continue;
                XDocument doc = XDocument.Load(fs);
                if (doc.Root != null)
                    Logger.L.Info(
                        $"Loaded plugin: {doc.Root.Attribute("Name")?.Value}. Version: {doc.Root.Attribute("Version")?.Value}");
                Handlers.Handlers.InitHandlers(file, false);
            }

            Logger.L.Info("Finish loading plugins!");
        }
    }
}