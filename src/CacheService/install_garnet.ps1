$exePath = "${PSScriptRoot}\CacheService.exe"
$serviceParameters = @{
    Name = "garnet"
    BinaryPathName = "${exePath} --env=dev"
    DisplayName = "Garnet"
    DependsOn = "TCPIP"
    StartupType = "Automatic"
    Description = "Garnet cache server service"
}

New-Service @serviceParameters
Start-Service -Name "garnet"