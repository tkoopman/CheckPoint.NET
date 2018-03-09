git config --global credential.helper store
Add-Content "$env:USERPROFILE.git-credentials" "https://$($env:GitToken):x-oauth-basic@github.com`n"
git config --global user.email tkoopman@users.noreply.github.com
git config --global user.name "tkoopman"
git clone --single-branch --branch gh-pages https://github.com/tkoopman/CheckPoint.NET.git C:\projects\gh-pages
Remove-Item -Path C:\projects\gh-pages\* -Force -Recurse
Get-ChildItem -Path C:\projects\checkPoint.net\docs\* -Recurse -File | Move-Item -Destination C:\projects\gh-pages\
cd C:\projects\gh-pages\
git add --all
git commit --message="Version $($env:VERSION)" --author="Tim Koopman <tkoopman@users.noreply.github.com>"
git push origin gh-pages