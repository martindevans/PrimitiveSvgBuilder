function Push-Nuget($path, $csproj) {
    $fullPathToCsprog = Join-Path -Path $path -ChildPath $csproj -resolve;
    
    #Create *.nupkg and *.symbols.nupkg
    nuget pack $fullPathToCsprog -Prop Configuration=Release -IncludeReferencedProjects -Verbosity quiet
    
    #Push *.nupkg and then delete it
    get-childitem -Filter *.nupkg -name | foreach ($_) {
        Write-Host "Pushing " $_ -backgroundcolor darkgreen -foregroundcolor white;
    
        nuget push $_ 94eb36f22f1143beb553dd23451bb2ce -Source http://martin-server:8083/nuget/Default -Verbosity quiet
        Remove-Item $_
        
        Write-Host "Done " $_ -backgroundcolor darkgreen -foregroundcolor white;
    }
    
    Write-Host ""
}

Push-Nuget "PrimitiveSvgBuilder" "PrimitiveSvgBuilder.csproj"

#Delete all *.nupkg and *.symbols.nupkg
get-childitem -Filter *.nupkg -name | foreach ($_) {
    Remove-Item $_
}
get-childitem -Filter *.symbols.nupkg -name | foreach ($_) {
    Remove-Item $_
}