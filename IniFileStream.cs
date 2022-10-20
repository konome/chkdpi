// IniFileStream - INI File Parser for .NET
// Copyright (c) 2022 konome
// MIT License

using System.Text;

namespace Konome
{
    public class IniFileStream
    {
        public FileInfo Path { get; }
        public List<string> Text { get; }
        public List<string> Sections { get; }
        public char Delimiter { get; }

        public IniFileStream(string? file = null, char? delimiter = null)
        {
            if (string.IsNullOrEmpty(file))
                file = "config.ini";

            Path = new FileInfo(file);

            if (!Path.Exists)
                throw new IOException($"Read error: Path does not exist.\n{Path.FullName}");
            if (Path.Extension != ".ini")
                throw new IOException($"Read error: This is not a .ini file.\n{Path.Name}");

            byte[] bytes = File.ReadAllBytes(Path.FullName);
            string str = Encoding.UTF8.GetString(bytes);

            Text = str.Split(Environment.NewLine).ToList();
            Sections = new List<string>();
            Delimiter = delimiter ?? '=';

            ValidateSections();
            CleanEnd();
        }

        public void WriteKey(string key, string? value, string section, bool writeBuffer = true)
        {
            string? str = key + Delimiter + value;

            // Get position of each key in the relevant section.
            GetKeysFromSection(section, out Dictionary<string, int> keys, out int indexSection);

            for (int i = 0; i < keys.Count; i++)
            {
                KeyValuePair<string, int> k = keys.ElementAt(i);

                // Remove key
                if (value == null && k.Key == key)
                {
                    Text.RemoveAt(k.Value);
                    break;
                }

                // Update key
                if (k.Key == key)
                {
                    Text[k.Value] = str;
                    break;
                }

                // New key.
                if (i == keys.Count - 1)
                {
                    Text.Insert(k.Value + 1, str);
                    break;
                }
            }

            // New key and section.
            if (keys.Count == 0)
            {
                if (indexSection != -1)
                {
                    Text.Insert(indexSection + 1, str);
                }
                else
                {
                    Text.Add(string.Empty);
                    Text.Add($"[{section}]");
                    Text.Add(str);
                }
            }

            // Update filestream.
            if (writeBuffer)
                Write();
        }

        public Dictionary<string, string> GetKeysFromSection(
            string section, out Dictionary<string, int> indexArr, out int indexSection)
        {
            Dictionary<string, string> keys = new();

            indexSection = FindSection(section);
            indexArr = new Dictionary<string, int>();

            if (indexSection != -1)
            {
                for (int i = indexSection + 1; i < Text.Count; i++)
                {
                    if (Text[i].StartsWith("[") && Text[i].EndsWith("]"))
                        break;

                    // Skip line breaks, comments and tabs.
                    if (string.IsNullOrEmpty(Text[i]))
                        continue;
                    if (Text[i].StartsWith("#") && Text[i].StartsWith(";"))
                        continue;
                    if (Text[i].StartsWith("\t"))
                        continue;

                    // Check delimiter.
                    if (!Text[i].Contains(Delimiter))
                        throw new Exception("No delimiter found");

                    // Split key and value.
                    string? k = Text[i].Split(Delimiter)[0].Trim();
                    string? v = Text[i].Split(Delimiter)[1].Trim();

                    if (keys.ContainsKey(k))
                        throw new Exception("key already exists");

                    keys.Add(k, v);
                    indexArr.Add(k, i);
                }
            }

            return keys;
        }

        public string ReadKey(string key, string section)
        {
            string? value = string.Empty;
            Dictionary<string, string> keys = GetKeysFromSection(section);

            if (keys.Count == 0)
                return value;

            foreach (KeyValuePair<string, string> k in keys)
            {
                if (k.Key.ToLower() == key.ToLower())
                {
                    value = k.Value;
                    break;
                }
            }

            return value;
        }

        public void DeleteSection(string section)
        {
            GetKeysFromSection(section, out Dictionary<string, int> keys, out int i);

            // Remove all keys from section in reverse order.
            foreach (KeyValuePair<string, int> k in keys.Reverse())
                Text.RemoveAt(k.Value);

            // Remove section header and extra line break.
            do
            {
                Text.RemoveAt(i);

            } while (Text[i] == string.Empty);

            Write();
        }

        public void FixFormat()
        {
            // Fix extra white spaces between sections.
            Text.RemoveAll(x => x == string.Empty);

            for (int i = 0; i < Text.Count; i++)
            {
                if (Text[i].StartsWith("[") && i > 0)
                    Text.Insert(i++, string.Empty);
            }

            // Trim delimiter.
            foreach (string? section in Sections)
            {
                Dictionary<string, string> keys =
                    GetKeysFromSection(section, out Dictionary<string, int> indexArr, out _);

                foreach (KeyValuePair<string, string> k in keys)
                    WriteKey(k.Key, k.Value, section, false);
            }

            CleanEnd();
            Write();
        }

        public void CleanEnd()
        {
            // Clean up line breaks at end of the buffer.
            int i = Text.Count - 1;
            while (string.IsNullOrEmpty(Text[i]))
            {
                Text.RemoveAt(i);
                i--;
            }
        }

        private void Write()
        {
            if (!Path.Exists)
                throw new IOException($"Write error: Path does not exist.\n{Path.FullName}");
            if (Path.Extension != ".ini")
                throw new IOException($"Write error: This is not a .ini file.\n{Path.Name}");

            string? str = string.Join(Environment.NewLine, Text);
            File.WriteAllBytes(Path.FullName, Encoding.UTF8.GetBytes(str));

            ValidateSections();
        }

        private void ValidateSections()
        {
            Sections.Clear();

            static void SyntaxError(string s)
                => throw new Exception($"Section incorrect syntax: {s}");
            static void DuplicationError(string s)
                => throw new Exception($"Duplicated section found: {s}");

            foreach (string s in Text)
            {
                if (s.Length == 0)
                    continue;

                if (s[..1] == "[" && s[^1..] != "]")
                    SyntaxError(s);
                if (s[..1] != "[" && s[^1..] == "]")
                    SyntaxError(s);

                if (s.StartsWith("[") && s.EndsWith("]"))
                {
                    string? sec = s[1..^1];
                    if (Sections.Contains(sec))
                        DuplicationError(sec);

                    Sections.Add(sec);
                }
            }
        }

        public int FindSection(string section) => Text.IndexOf($"[{section}]");

        public void DeleteKey(string key, string section)
            => WriteKey(key, null, section);

        public Dictionary<string, string> GetKeysFromSection(string section)
            => GetKeysFromSection(section, out _, out _);
    }
}