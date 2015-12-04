function Push-Nuget($path, $csproj) {
    $fullPathToCsprog = Join-Path -Path $path -ChildPath $csproj -resolve;
    
    nuget pack $fullPathToCsprog -Prop Configuration=Release -IncludeReferencedProjects
    
    get-childitem -Filter *.nupkg -name | foreach ($_) {
        Write-Host "Pushing " $_ -backgroundcolor darkgreen -foregroundcolor white;
    
        nuget push $_
        Remove-Item $_
        
        Write-Host "Done " $_ -backgroundcolor darkgreen -foregroundcolor white;
    }
}

Push-Nuget "PrimitiveSvgBuilder" "PrimitiveSvgBuilder.csproj"

#Delete all *.nupkg and *.symbols.nupkg
get-childitem -Filter *.nupkg -name | foreach ($_) {
    Remove-Item $_
}
get-childitem -Filter *.symbols.nupkg -name | foreach ($_) {
    Remove-Item $_
}