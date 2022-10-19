# chkdpictx
A tool for Windows to retrieve DPI related data. Incidentally `chkdpictx` is also able to encode/decode base64 from any string and file.

The following data are being retrieved:
- Available DPI awareness modes and their associated context handles.
- Current system DPI.
- Primary display resolution.
- Windows version of the host.

## How to use:
- Double click on `"chkdpictx.exe"` to retrieve and copy DPI related data to clipboard. Data is encoded to a base64 string hash.
- Run `"chkdpictx --decode HASH"` from a command prompt to read the data. 
- Running `"chkdpictx"` from a command prompt without passing any arguments will retrieve and copy DPI related data to clipboard, and will also display both the raw and encoded data immediately simultaneously.
- Note that you can convert any string data to and from base64, including from a file, using the appropiate command-line option.

## Command line options:
`-d`, `--decode HASH`<br />
  Decode base64 data to a readable string.

`--decode-file FILE`<br />
  Decode a .txt file containing a base64 hash.

`-e`, `--encode STRING`<br />
  Encode a string value to base64.

`--encode-file FILE`<br />
  Encode a file to base64.

`-v`, `--version`<br />
  Show version of this application.

`-h`, `--help`<br />
  Show help.
  
  ## System Requirements:
  Windows 10 or later.
