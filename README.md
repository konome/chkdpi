# chkdpictx
A tool for Windows to retrieve DPI related data. Incidentally `chkdpictx` is also able to encode/decode base64 from any string and file.

The following data are being retrieved:
- Available DPI awareness modes and their associated context handles
- Current system DPI
- Primary display resolution
- Windows version of host.

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
