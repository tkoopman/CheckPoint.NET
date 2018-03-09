cd ..
git config --global credential.helper store
Add-Content "$env:USERPROFILE.git-credentials" "https://$($env:GitToken):x-oauth-basic@github.com`n"
git config --global user.email tkoopman@users.noreply.github.com
git config --global user.name "tkoopman"
git clone --single-branch --branch gh-pages https://github.com/tkoopman/CheckPoint.NET.git gh-pages
Remove-Item -Path .\gh-pages\* -Force -Recurse
Get-ChildItem -Path .\CheckPoint.NET\docs\* -Recurse -File | Move-Item -Destination .\gh-pages\
cd .\gh-pages\
git add --all
git commit --message="Version $($env:VERSION)" --author="Tim Koopman <tkoopman@users.noreply.github.com>"
git push origin gh-pages