function ZipDirectory($zipCommand, $directory, $file)
{
    write-host "`r`n`r`nZipping directory $directory to $file ..."
    delete_file $file
    $current_dir = Get-Location
    cd $directory
    & $zipCommand a -r $file
    cd $current_dir
}

function delete_file($file)
{
    if($file) { remove-item $file -force -ErrorAction SilentlyContinue | out-null } 
}
