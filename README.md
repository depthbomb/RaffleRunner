<p align="center">
	<table>
		<tbody>
			<td align="center">
				<h1>RaffleRunner</h1>
				<p>
					<a href="https://github.com/depthbomb/RaffleRunner/commits"><img src="https://img.shields.io/github/last-commit/depthbomb/RaffleRunner.svg?label=Updated&logo=github&style=flat-square"></a>
					<img src="https://img.shields.io/github/repo-size/depthbomb/RaffleRunner.svg?label=Repo%20Size&logo=github&style=flat-square">
					<a href="https://github.com/depthbomb/RaffleRunner/releases"><img src="https://img.shields.io/github/downloads/depthbomb/RaffleRunner/total.svg?label=Downloads&logo=github&style=flat-square"></a>
					<a href="https://github.com/depthbomb/RaffleRunner/blob/main/LICENSE"><img src="https://img.shields.io/github/license/depthbomb/RaffleRunner.svg?label=License&logo=apache&style=flat-square"></a>
				</p>
				<p>
					<a href="https://github.com/depthbomb/RaffleRunner/releases/latest"><img src="https://img.shields.io/github/release/depthbomb/RaffleRunner.svg?label=Stable&logo=github&style=flat-square"></a>
					<a href="https://github.com/depthbomb/RaffleRunner/releases/latest"><img src="https://img.shields.io/github/release-date/depthbomb/RaffleRunner.svg?label=Released&logo=github&style=flat-square"></a>
					<a href="https://github.com/depthbomb/RaffleRunner/releases/latest"><img src="https://img.shields.io/github/downloads/depthbomb/RaffleRunner/latest/total.svg?label=Downloads&logo=github&style=flat-square"></a>
				</p>
				<p>
					<a href='https://ko-fi.com/O4O1DV77' target='_blank'><img height='36' src='https://cdn.ko-fi.com/cdn/kofi1.png?v=3' alt='Buy Me a Coffee at ko-fi.com' /></a>
				</p>
				<img width="2000" height="0">
			</td>
		</tbody>
	</table>
</p>

This is a beta version of the cross-platform CLI application version of my [Scraps project.](https://github.com/depthbomb/Scraps) RaffleRunner features the full raffle-joining functionality of Scraps bundled into a single portable binary that can be executed from your favorite terminal with the `rafrun` command.

## Requirements

RaffleRunner releases provide both self-contained (`-sc`) and framework-dependent (`-fd`) binaries.

The difference between the two is that self-contained binaries do not require the .NET 7 Runtime to be installed to run. However this does mean that self-contained binaries are quite a bit larger (up to **1000%**!) but may boot up _slightly_ faster.

If you choose to use a framework-dependent binary then you may download and install the .NET 7 Runtime from [here.](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Installation

You can simply extract the binary from the archive you download from the [latest release.](https://github.com/depthbomb/RaffleRunner/releases/latest) However, if you wish to execute RaffleRunner in your terminal from any location, you must add it to your system's PATH.

If you are on a non-Windows system then you likely already know how to do this. If you are on Windows, on the other hand, you can follow [this answer.](https://stackoverflow.com/a/41895179/2526063) A full release of RaffleRunner may include an installer that sets this up for you.

## Compatibility

RaffleRunner has builds for Windows (x86, x64, arm, arm64) as well as builds Linux. However, the Linux builds have only been tested on Ubuntu 20.04.

## Commands

- `$ rafrun check-won [options]`
  - `-c | --cookie <value>` - Your scr_session cookie value, omit to use saved cookie instead (see `save-cookie`)
  - `-o | --open` - Open the won raffles page in your system's default browser
- `$ rafrun join-raffles [options]`
  - `-c | --cookie <value>` - Your scr_session cookie value, omit to use saved cookie instead (see `save-cookie`)
  - `-r | --repeat <number>` - How many times to re-scan and join available raffles, omit to run indefinitely
  - `-e | --ending` - Whether to sort raffles by time remaining, by default they are sorted by when they were created
  - `-i | --increment-scan-delay <number>` - Whether to increment the scan delay by 1 second if a scan resulted in no available raffles
  - `-p | --paranoid` - Enable paranoid mode
- `$ rafrun save-cookie <cookie>` - Saves your cookie value to the file system so you do not need to supply it each time you use a command that requires it
- `$ rafrun check-updates` - Check for new releases

All commands also support a `-d | --debug` option which enables debug logging.

## Beta Disclaimer

This project is currently a work-in-progress so it may be buggy and missing features.
