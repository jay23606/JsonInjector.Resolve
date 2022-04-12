# JsonInjector.Resolve

Barebones dependency injection from a json source.

Uses Activator.CreateInstance to instantiate objects rescursively.

Registration looks like this:
```csharp
json = @"
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
";
```

Normally, to instantiate these objects you would do this:
```csharp
  var dc = DummyClass(new TodayWriter(new ConsoleOutput()))
  dc.WriteSomething();
```

Using JsonInjector.Resolve, the parameters are matched and filled for you:
```csharp
  var instances = JsonInjector.Resolve(json);
  if (resolvedInstances["DemoApp.DummyClass"] is IDummyClass dc)
    dc.WriteSomething();
```

Disclaimer: I am sure there might be edge cases that I don't handle and you should probably not use this in production code.

Good video describing dependency injection using Autofac:

https://www.youtube.com/watch?v=mCUNrRtVVWY

Inspiration for this project:

https://www.youtube.com/watch?v=NSVZa4JuTl8




