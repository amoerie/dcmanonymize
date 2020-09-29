DICOM ORGANIZE
--------------

Organizes DICOM files on your disk.

Options 

```
  files pos. 0               Required. (Default: .) Organize these DICOM files. When missing, this option will be read from the piped input.
  
  -t, --targetDirectory      Required. (Default: .) Organize DICOM files in this directory

  -p, --targetFilePattern    Required. (Default: {PatientName}/{AccessionNumber}/{SeriesNumber}/{InstanceNumber ?? SOPInstanceUID}.dcm) Organize DICOM files in this directory

  --help                     Display this help screen.

  --version                  Display version information.
```