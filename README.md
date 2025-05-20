# Read memory

This project provides a class to read memory values ​​from a process.

### Usage

- You need to find the memory addresses and pointers of the application you want to monitor. I recommend using [Chat Engine](https://www.cheatengine.org) for this.
- You need to create an application for your needs with the reader class. It can be a console application or a windows form.
- In your application, use code similar to the one below to open the process.
- With the process open, read the desired values ​​as per the example below.

#### Sample code to open a process:

```
var process = Process.GetProcessesByName("processName").FirstOrDefault() ?? throw new Exception("process not found!");
var memory = new MemoryHelper(process, "processName.exe");
```

#### Sample code to read a value from memory:

```
var value = memory.ReadMemoryAddress("0x002B117C");
```

### ⚠️ Note

Remember that memory addresses change every time the game is opened. Look for pointers because most of the time they only change the address when a new version of the application is generated.
