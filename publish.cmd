SET nugetversion=1.0.3
nuget push ./DcmAnonymize/nupkg/DcmAnonymize.%nugetversion%.nupkg -source nuget.org
nuget push ./DcmAnonymize/nupkg/DcmAnonymize.%nugetversion%.nupkg -source Github
pause
