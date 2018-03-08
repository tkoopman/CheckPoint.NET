Start-FileDownload https://github.com/EWSoftware/SHFB/releases/download/v2017.12.30.2/SHFBInstaller_v2017.12.30.2.zip
7z x -y SHFBInstaller_v2017.12.30.2.zip | Out-Null
Write-Host "Installing MSI..."
cmd /c start /wait msiexec /i InstallResources\SandcastleHelpFileBuilder.msi /quiet
Write-Host "Installing VSIX..."
. "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\VSIXInstaller.exe" /q /a InstallResources\SHFBVisualStudioPackage_VS2015AndLater.vsix
Write-Host "Sandcastle installed" -ForegroundColor Green