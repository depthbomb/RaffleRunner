$publish_dir = "$($PSScriptRoot)\RaffleRunner\bin\Publish"
$sevenzip_path = "$($PSScriptRoot)\bin\7za.exe"
$runtimes = "linux-x64",
		    "linux-arm",
		    "linux-arm64",
		    "linux-musl-x64",
		    "win-arm",
		    "win-arm64",
		    "win-x64",
		    "win-x86"

iex "cd $($publish_dir)"

Foreach ($r in $runtimes)
{
	$runtime_dir = "$($publish_dir)\$($r)"
	
	if ($r -clike 'linux*') {
		# Windows 10 supports the tar command as of 17063
		iex "tar -cvzf $($runtime_dir).tar.gz $($runtime_dir)"
	} else {
		iex "$($sevenzip_path) a -tzip $($runtime_dir).zip $($runtime_dir)/* -mx9"
	}
}