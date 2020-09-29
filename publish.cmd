SET nugetversion=1.0.0
nuget push ./DcmOrganize/nupkg/DcmOrganize.%nugetversion%.nupkg -source nuget.org
nuget push ./DcmOrganize/nupkg/DcmOrganize.%nugetversion%.nupkg -source Github
pause
