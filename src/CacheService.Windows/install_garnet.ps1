$exePath = "${PSScriptRoot}\CacheService.exe"
$serviceParameters = @{
    Name = "garnet"
    BinaryPathName = "${exePath}"
    DisplayName = "Garnet"
    DependsOn = "TCPIP"
    StartupType = "Automatic"
    Description = "Garnet cache server service"
}

New-Service @serviceParameters