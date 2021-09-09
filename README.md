# jerk
Joe Everyman's Reporting Konverter

This is a small CLI utility to take the XML output from `dotnet test` in TRX-format and transform it into something else. The assumption is that you'll pass in several input XML files and output to a combined transformed file.

The only transformer currently included will convert to a CSV that simply shows the test status, the test name and???

## Generating the test runner XML output

Before calling this tool, you should have done something to generate some test output. For example, with .NET Core 2.2+ (including .NET 5.0 and .NET 6.0), you might run something like

```
dotnet test --logger "trx"
```

This will place a .TRX file in a `TestResults` directory within each test project. The filename will contain a unique pattern so that future runs won't overwrite your current output. For your usage, that may not be useful. Instead, you can specify a filename for the output.

```
dotnet test --logger "trx;LogFileName=TestOutputResults.xml"
```

This will, again, be placed within each test project's `TestOutput` directory. But, this time the file will be overwritten each run.

## Using

Once you have some test runner xml output to process, you can actually run jerk to transform the output.

It's not a dotnet tool yet so you're on your own to build it. Clone this repository and run the following locally at the root:

```
dotnet build
dotnet publish -o ./dist
```

That will give you everything you need in the /dist folder.

Once built, you call `jerk` with the root path to search and, optionally, the glob pattern to match, and the output csv file name:

```
jerk . -p **/TestOutputResults.xml -o testSummary.csv
```

Or, if you're not running `jerk` from your solution's root directory, you might want to run it like:

```
jerk c:\work\MyAwesomeProject -p **/TestOutputResults.xml -o c:\work\testSummary.csv
```
