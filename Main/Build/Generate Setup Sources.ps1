# Set paths to be fully qualified
$conqatDir = [system.io.path]::GetFullPath("$(pwd)\..\Lib\ConQAT 2.5")
$outputDir = [system.io.path]::GetFullPath("$(pwd)\..\Output\Setup")

# Remember current path to restore it later
$location = pwd

# Get all bundle directories
$bundleDirectories = ls "$conqatDir\bundles" | ? { $_.PSIsContainer }

# Make sure the output directory (where we write the WiX source files to) exists
# Otherwise heat will fail.
if ( !(test-path $outputDir -pathtype container) )
{
    #Ignore return value of mkdir
    mkdir $outputDir > $nul
}

# Go to the ConQAT directory and run heat.
cd $conqatDir
$bundleDirectories | % { heat dir $_.Fullname -o "$outputDir\$($_.Name).wxs" -cg $_.Name -nologo -dr bundles -gg -ke -scom -sfrag -sreg -svb6 -var "var.Bundle_$($_.Name)" }
cd $location