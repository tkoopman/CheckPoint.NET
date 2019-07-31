cd $env:Temp

Start-FileDownload https://github.com/EWSoftware/SHFB/releases/download/v2017.12.30.2/SHFBInstaller_v2017.12.30.2.zip
7z x -y SHFBInstaller_v2017.12.30.2.zip | Out-Null
Write-Host "Installing MSI..."
cmd /c start /wait msiexec /i InstallResources\SandcastleHelpFileBuilder.msi /quiet
Write-Host "Installing VSIX..."
. "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\VSIXInstaller.exe" /q /a InstallResources\SHFBVisualStudioPackage_VS2015AndLater.vsix
Write-Host "Sandcastle installed" -ForegroundColor Green

Write-Host "Installing OpenVPN..."
(New-Object Net.WebClient).DownloadFile("https://swupdate.openvpn.org/community/releases/openvpn-install-2.4.7-I603.exe", "${env:Temp}\openvpn-install-2.4.7-I603.exe")

# Trust OpenVPN Cert so TAP Driver will silently install
certutil -addstore -f "TrustedPublisher" "$env:APPVEYOR_BUILD_FOLDER\AppVeyor\OpenVPN_Install.cer"
# Install OpenVPN
Start-Process -Wait -FilePath .\openvpn-install-2.4.7-I603.exe -ArgumentList "/S /D=C:\OpenVPN"
Write-Host "OpenVPN installed" -ForegroundColor Green

# Add config
iex ((New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/appveyor/secure-file/master/install.ps1'))
.\appveyor-tools\secure-file -decrypt "$env:APPVEYOR_BUILD_FOLDER\AppVeyor\AppVeyor.ovpn.enc" -secret "$env:TEST_SECRET" -out C:\OpenVPN\config\AppVeyor.ovpn
.\appveyor-tools\secure-file -decrypt "$env:APPVEYOR_BUILD_FOLDER\AppVeyor\AppVeyor.auth.enc" -secret "$env:TEST_SECRET" -out C:\OpenVPN\config\AppVeyor.auth

$TestIP = $env:TestMgmtServer

Write-Host "Connecting to VPN..."
$p = Start-Process -FilePath C:\OpenVPN\bin\openvpn.exe -ArgumentList "--config AppVeyor.ovpn --log AppVeyor.log --connect-retry-max 5" -WorkingDirectory C:\OpenVPN\config\ -PassThru
# Wait for it to connect
do { $ping = Test-Connection -ComputerName $TestIP -Count 1 -Quiet -TimeToLive 2 }until($ping -or $p.HasExited)
if (-not $ping) { throw "Unable to establish VPN" } else { Write-Host "VPN Established" -ForegroundColor Green }

cd $env:APPVEYOR_BUILD_FOLDER