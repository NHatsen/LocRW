# LocEW

LocRW is a dll that let's you read a FTView .loc file, returning a list of the local messages. Also, it's possible to write a new .loc file is you provide a valid list of local messages.

Some methods for reading and writing local messages from/to .csv files are provided. 

## How to use

First of all, LocRW contains a public class for local messages:

```c#
public class TriggerClass
{
    public string Value { get; set; }
    public string Message { get; set; }
}
```
You need to declare a variable using this class to be able to store a FTView local message (trigger value and message text):  

```c#
LocRW.TriggerClass item = new LocRW.TriggerClass(); 
```

Some methods requiere a list of TriggerClass items:

```c#
List<LocRW.TriggerClass> list = new List<LocRW.TriggerClass>();
```

#### Methods

#### Read a loc file:

```c#
list = LocRW.LocFile.ReadLocFile(string path);
```

Reads a .loc file specified for the path provided and returns a list of local messages.

#### Write a loc file:

```c#
LocRW.LocFile.WriteLocFile(string path, List<TriggerClass> list);
```

Creates a .loc file using the provided path and list of messages. 

#### Read a csv file:

```c#
list = LocRW.CsvFile.ReadCSV(string path);
```

Reads a .csv file specified by the provided path and returns a list of local messages. CSV file has to be written using the following pattern:

```xml
Value,Message
1,First message
2,Second message
```

#### Write a csv file:

```c#
LocRW.CsvFile.WriteCSV(string path, List<TriggerClass> triggers);
```

Creates a .csv file using the path and list of messages specified. 



## To Do

Add some error managing and feedback anytime an operation is completed.

Add some options to read/write csv files using different separators and headers.

## Version History

See [CHANGELOG.md](CHANGELOG.md).

## Copyright and License

Copyright 2020 NHatsen.

Licensed under the [MIT LICENSE](LICENSE.md).
