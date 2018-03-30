if ($($env:InfomationalVersion).Contains("-")) {
	Write-Host "Skipping deploying gh-pages."
} else {
	git config --global credential.helper store
	Add-Content "$env:USERPROFILE\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
	git config --global user.email tkoopman@users.noreply.github.com
	git config --global user.name "Tim Koopman"
	git config --global core.autocrlf true

	git clone --single-branch --branch gh-pages https://github.com/tkoopman/CheckPoint.NET.git C:\projects\gh-pages
	cd C:\projects\gh-pages\
	git rm -r --quiet *
	Get-ChildItem -Path C:\projects\checkpoint-net\docs\* | Move-Item -Destination C:\projects\gh-pages\

	git add --all
	git commit --message="Version $($env:APPVEYOR_BUILD_VERSION)"
	git push origin gh-pages
}