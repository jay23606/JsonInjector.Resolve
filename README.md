# JsonInjector.Resolve

Uses Activator.CreateInstance to instantiate objects recursively.

Registration looks like this:
```json
[
    {
    'Implementation': 'DemoApp.DummyClass, af',
    'Interface': 'DemoApp.DummyClass'
    }
    ,
    {
    'Implementation': 'DemoApp.ConsoleOutput, af',
    'Interface': 'DemoApp.IOutput'
    }
    ,
    {
    'Implementation': 'DemoApp.TodayWriter, af',
    'Interface': 'DemoApp.IDateWriter'
    }
]
```

Normally, to instantiate these objects you would do this:
```csharp
  var dc = DummyClass(new TodayWriter(new ConsoleOutput()))
  dc.WriteSomething();
```

Using JsonInjector.Resolve, the parameters are filled for you:
```csharp
  var instances = JsonInjector.Resolve(json);
  if (resolvedInstances["DemoApp.DummyClass"] is IDummyClass dc)
    dc.WriteSomething();
```

This is a good video describing dependency ibjection if it is a new concept:

https://www.youtube.com/watch?v=mCUNrRtVVWY



