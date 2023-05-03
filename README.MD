🕵️‍ DICOM ANONYMIZE
---------------

[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2Famoerie%2Fdcmanonymize%2Fbadge%3Fref%3Dmain&style=for-the-badge&label=Build)](https://actions-badge.atrox.dev/amoerie/dcmanonymize/goto?ref=main) 
[![Codecov](https://img.shields.io/codecov/c/github/amoerie/dcmanonymize?label=Coverage&logo=codecov&style=for-the-badge)](https://app.codecov.io/gh/amoerie/dcmanonymize)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/DcmAnonymize?label=DcmAnonymize&style=for-the-badge)](https://www.nuget.org/packages/DcmAnonymize)

Anonymizes DICOM files on your disk.

Features
--------

* Modified implementation of [DICOM Basic Security Profile](https://dicom.nema.org/medical/dicom/current/output/chtml/part15/chapter_E.html#sect_E.1)
* Does not change or remove pixel data
* Generates unique accession numbers, patient names, physician names, birth dates, UIDs, etc.
* Retains patient > study > series > image structure across many files
* Retains patient data consistency, DICOM file UID references, etc across many files
* Fast
* Simple

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

See https://github.com/amoerie/dcmfind


Options 

```
  files pos. 0               Required. (Default: .) Anonymize these DICOM files. When missing, this option will be read from the piped input.

  -p, --parallelism          (Default: 8) Process this many files in parallel
  
  --help                     Display this help screen.

  --version                  Display version information.
```
