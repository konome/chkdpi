# chkdpi
A tool for Windows to retrieve DPI related data. Incidentally `chkdpi` is also able to encode/decode base64 from any string and file.

The following data are being retrieved:
- Available DPI awareness modes and their associated context handles.
- Current system DPI.
- Primary display resolution.
- Windows version of the host.

## How to use:
- Double click on `"chkdpi.exe"` to retrieve and copy DPI related data to clipboard. Data is encoded to a base64 string hash.
- Run `"chkdpi --decode HASH"` from a command prompt to read the data. 
- Running `"chkdpi"` from a command prompt without passing any arguments will retrieve and copy DPI related data to clipboard, and will also display both the raw and encoded data immediately simultaneously.
- Note that you can convert any string data to and from base64, including from file, using the appropiate command-line option.
- Additionally, DPI related data can be written to a .ini file using `--ini FILE` in a command prompt.

## Command line options:
`-d`, `--decode HASH`<br />
  Decode base64 data to a readable string.

`--decode-file FILE`<br />
  Decode a .txt file containing a base64 hash.

`-e`, `--encode STRING`<br />
  Encode a string value to base64.

`--encode-file FILE`<br />
  Encode a file to base64.

`--ini FILE`<br />
  Output DPI related data to a .ini file.

`--no-clipboard`<br />
  Do not copy base64 to clipboard.

`--base64`<br />
  Output base64 only.

`-v`, `--version`<br />
  Show version of this application.

`-h`, `--help`<br />
  Show help.
  
  ## System Requirements:
  Windows 10 or later.
