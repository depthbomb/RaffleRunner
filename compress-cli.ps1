$publish_dir = "$($PSScriptRoot)\RaffleRunner\bin\Publish"
$sevenzip_path = "$($PSScriptRoot)\bin\7za.exe"
$runtimes = "linux-arm64-fd",
            "linux-arm64-sc",
            "linux-arm-fd",
            "linux-arm-sc",
            "linux-x64-fd",
            "linux-x64-sc",
            "win-arm64-fd",
            "win-arm64-sc",
            "win-arm-fd",
            "win-arm-sc",
            "win-x64-fd",
            "win-x64-sc",
            "win-x86-fd",
            "win-x86-sc"

iex "cd $($publish_dir)"

Foreach ($r in $runtimes)
{
	$runtime_dir = "$($publish_dir)\$($r)"
	
	if ($r -clike 'linux-*') {
		# Windows 10 supports the tar command as of 17063
		iex "tar cvzf $($runtime_dir).tar.gz $($runtime_dir)"
	} else {
		iex "$($sevenzip_path) a -tzip $($runtime_dir).zip $($runtime_dir)/*"
	}
}