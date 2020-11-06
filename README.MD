🕵️‍ DICOM ANONYMIZE
---------------

[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2Famoerie%2Fdcmanonymize%2Fbadge%3Fref%3Dmaster&style=for-the-badge&label=Build)](https://actions-badge.atrox.dev/amoerie/dcmanonymize/goto?ref=master) [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/DcmAnonymize?label=DcmAnonymize&style=for-the-badge)](https://www.nuget.org/packages/DcmAnonymize)

Anonymizes DICOM files on your disk.

Installation
------------

```
    dotnet tool install dcmanonymize --global
```

Usage
-----

```powershell
    dcmanonymize file1.dcm file2.dcm file3.dcm
```

or using piped input

```powershell
    dcmfind | dcmanonymize
```


Options 

```
  files pos. 0               Required. (Default: .) Anonymize these DICOM files. When missing, this option will be read from the piped input.

  -p, --parallelism          (Default: 8) Process this many files in parallel
  
  --help                     Display this help screen.

  --version                  Display version information.
```
