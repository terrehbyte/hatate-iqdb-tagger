using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Hatate.Properties;
using System;

using TraceMoe;
using TraceMoe.NET;
using TraceMoe.NET.DataStructures;
using System.Diagnostics;

namespace Hatate
{
    class TraceMoe
    {
        private List<Match> matches = new List<Match>();

        public async Task SearchFile(string filePath)
        {
            await this.SearchImage(filePath);
            return;
        }

        private async Task<bool> SearchImage(string filePath)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "what-anime-cli.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = $"file \"{filePath}\"";
                process.Start();

                bool success = false;

                string titleForParsing = "";

                StreamReader reader = process.StandardOutput;
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    
                    if(!success && line.Contains("Found")) { success = true; }
                    if(success)
                    {
                        const string key = "Title English: ";
                        int startIndex = line.LastIndexOf(key);
                        if (startIndex > -1)
                        {
                            titleForParsing = line.Substring(key.Length+startIndex);
                        }
                    }
                }

                if(success)
                {
                    Match bestMatch = new Match();
                    bestMatch.Tags.Add(new Tag($"series:{titleForParsing}")); // TODO: fix parsing
                    bestMatch.Similarity = 99f; // TODO: get real score
                    bestMatch.Source = Enum.Source.TraceMoe;

                    // TODO: figure out what's up - this isn't enough
                    matches.Add(bestMatch);
                }

                return success;
            }
        }

        public ImmutableList<Match> Matches
        {
            get { return ImmutableList.Create(this.matches.ToArray()); }
        }
    }
}
