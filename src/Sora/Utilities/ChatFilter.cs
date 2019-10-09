using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Sora.Utilities
{
    public class ChatFilter
    {
        private Dictionary<string, string> _filters = new Dictionary<string, string>();
        
        public ChatFilter()
        {
            if (!File.Exists("filters.txt"))
                File.WriteAllLines("filters.txt", new [] {  "fuck=truck" });

            var lines = File.ReadAllLines("filters.txt");

            foreach (var ln in lines)
            {
                var filter = ln.Split("=");
                _filters.Add(filter[0], filter[1]);
            }
        }
        
        
        public string Filter(string input)
        {
            var s = input;
            
            foreach (var (key, value) in _filters)
            {
                var regex = new Regex($"(.*)\b{key}\b(.*)");
                s = regex.Replace(s, value);
            }

            return s;
        }
    }
}