# jerk
Joe Everyman's Reporting Konverter

This is a small CLI utility to take the XML output from `dotnet test` in TRX-format and transform it into something else. The assumption is that you'll pass in several input XML files and output to a combined transformed file.

The only transformer currently included will convert to a CSV that simply shows the test name, the test outcome (passed/failed/skipped), and the test output. 

For passed tests, the test output column will be empty. For skipped test, the test output column will show the skip reason (note, this was only verified with xUnit). For failed test, the test output will be the message... including line breaks. If your CSV parser doesn't support line breaks then open an issue and let's talk!.

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

### As a dotnet tool

For your needs, `jerk` probably makes the most sense as a globally installed tool. And, since I want the command to be a bit more instructive, the dotnet tool is actually called `trx2csv`. 

Head to https://www.nuget.org/packages/hyrmn.trx2csv/ and copy the install command.

```
dotnet tool install --global hyrmn.trx2csv
```

Then, at the root of your solution directory, you can run `trx2csv`

```
trx2csv
```

This will run with the defaults. To see the available options, you can run

```
trx2csv -h
```

For example, you (or at least your engineering manager), probably won't like the default file name. To customize that, run the command like this:

```
trx2csv . -o testSummary.csv
```

### Building it yourself
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

## From here?

This was written for a single use case for a single user. But, if you find it's useful, go ahead and fork or modify and have fun. If you have any issues, please open an issue here or, better, DM me on twitter [@hyrmn](https://twitter.com/hyrmn)