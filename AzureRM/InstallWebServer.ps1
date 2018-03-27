#
# InstallWebServer.ps1
#
configuration IIS {
node ("localhost")
{
WindowsFeature WebServer{
    Ensure = "Present"
    Name = "web-server"
    IncludeAllSubFeature = $true
}
}
}