$exePath = "${PSScriptRoot}\Cache.Service.exe"
sc.exe garnet binPath= "${exePath}" DisplayName= "Garnet" start= auto depend= TCPIP
sc.exe description garnet "Garnet cache server service"
net.exe start garnet