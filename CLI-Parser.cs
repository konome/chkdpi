using System.Text.RegularExpressions;

// CLI-Parser - Command-line Parser for .NET
// Copyright (c) 2022 konome
// MIT License

namespace Konome
{
    public static class CLI
    {
        public static event Action<string> OnParseError = delegate { };
        public static event Action<string, string> OnParse = delegate { };

        private static readonly Dictionary<string, string> _options = new();
        public static Dictionary<string, string> Options => _options;

        public static int Parse(string[] args)
        {
            //0: Success, 1: Error, 2: Empty array.
            int errLvl = 2;

            _options.Clear();

            if (args.Length > 0)
            {
                errLvl = Validate(args);

                if (errLvl == 0)
                {
                    foreach (KeyValuePair<string, string> o in _options)
                        OnParse(o.Key, o.Value);
                }
            }

            return errLvl;
        }

        private static int Validate(in string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                // Validate option.
                if (args[i].Length < 2)
                    return RaiseError("Option length too short.");
                if (args[i][..2] != "--" && args[i][..1] != "-")
                    return RaiseError("Options must have the '--' prefix. Use a singular dash for short named options.");
                if (args[i][..1] == "-" && args[i].Length > 2 && args[i][..2] != "--")
                    return RaiseError("Short named options may only have one character.");
                if (args[i][..1] == "-" && args[i].Length == 2 && !ValidateString(args[i][1..]))
                    return RaiseError("Invalid short named option.");
                if (args[i][..1] != "-" && !ValidateString(args[i][2..]))
                    return RaiseError("Invalid option.");
                if (_options.ContainsKey(args[i]))
                    return RaiseError("Duplication...");

                // Validate parameter.
                if (false is bool bflag && i + 1 == args.Length)
                    bflag = true;
                if (!bflag && args[i + 1].Length >= 2 && args[i + 1][..1] == "-")
                    bflag = true;
                if (!bflag && !ValidateString(args[i + 1][..1]))
                    return RaiseError("Invalid parameter.");

                if (bflag)
                    _options.Add(args[i], string.Empty);
                else
                    _options.Add(args[i], args[++i]);
            }

            return 0;
        }

        private static int RaiseError(string err)
        {
            OnParseError(err);
            return 1;
        }

        private static bool ValidateString(string s) => Regex.IsMatch(s, "^[a-zA-Z0-9]+$");
    }
}
