using System;
using System.Reflection;
using Autofac;
using System.Text.Json;

namespace DemoApp
{

    public class Program
    {
        static void Main(string[] args)
        {
//            //NORMAL WAY
//            IDateWriter tw = new TodayWriter(new ConsoleOutput());
//            tw.WriteDate();


//            //USING AUTOFAC
//            var builder = new ContainerBuilder();
//            builder.RegisterType<ConsoleOutput>().AsImplementedInterfaces();//.As<IOutput>();
//            //builder.RegisterType<TodayWriter>().SingleInstance();
//            builder.RegisterType<TodayWriter>().AsImplementedInterfaces();//.As<IDateWriter>();
//            var container = builder.Build();
//            var writer = container.Resolve<IDateWriter>(); //with AsImplementedInterfaces
//            //var writer = container.Resolve<TodayWriter>(); //with SingleInstance
//            writer.WriteDate();


//            //USING CreateInstance
//            var co = Activator.CreateInstance(typeof(ConsoleOutput)) as IOutput;
//            var instance = Activator.CreateInstance(typeof(TodayWriter), co) as IDateWriter;
//            instance?.WriteDate();

//            //USING CreateInstance with parameters array
//            List<object> parameters = new List<object>();
//            co = Activator.CreateInstance(typeof(ConsoleOutput)) as IOutput;
//            parameters.Add(co);
//            instance = Activator.CreateInstance(typeof(TodayWriter), parameters.ToArray()) as IDateWriter;
//            instance?.WriteDate();


//            //USING CreateInstance and Type.GetType (so that parameters could be stored in xml,json for example)
//            Type? type = Type.GetType("DemoApp.ConsoleOutput, af");
//            co = Activator.CreateInstance(type) as IOutput;
//            type = Type.GetType("DemoApp.TodayWriter, af");
//            parameters = new List<object>();
//            parameters.Add(co);
//            //instance = Activator.CreateInstance(type, co) as IDateWriter;
//            instance = Activator.CreateInstance(type, parameters.ToArray()) as IDateWriter;
//            instance?.WriteDate();


//            //we could look up the parameters from the fully qualified name here in a dictionary
//            var json = @"
//[
//    {
//    'Name': 'DemoApp.ConsoleOutput, af',
//    'Parameters': []
//    }
//    ,
//    {
//    'Name': 'DemoApp.TodayWriter, af',
//    'Parameters': ['DemoApp.ConsoleOutput, af']
//    }
//]
//".Replace("'", "\""); //System.Text.Json.JsonSerializer.Deserialize doesn't like single quotes

//            var instances = JsonSerializer.Deserialize<List<Instance>>(json);
//            Dictionary<string, object> nameInstance = new Dictionary<string, object>();

//            //We could instantiate objects without constructor parameters first
//            foreach (var inst in instances)
//            {
//                if (inst.Name == null) continue;
//                if (inst.Parameters?.Count == 0 && !nameInstance.ContainsKey(inst.Name))
//                {
//                    var myType = Type.GetType(inst.Name);
//                    if (myType != null)
//                    {
//                        var myInst = Activator.CreateInstance(myType);
//                        if(myInst != null) nameInstance.Add(inst.Name, myInst);
//                    }
//                }
//            }

//            //Now instantiate objects that do require parameters using dictionary
//            foreach (var inst in instances)
//            {
//                if (inst.Parameters.Count > 0 && !nameInstance.ContainsKey(inst.Name))
//                {
//                    var myType = Type.GetType(inst.Name);

//                    var myParameters = new List<object>();
//                    foreach (string name in inst.Parameters) myParameters.Add(nameInstance[name]);

//                    var myInst = Activator.CreateInstance(myType, myParameters.ToArray());
//                    //myInst = Convert.ChangeType(myInst, myType);
//                    nameInstance.Add(inst.Name, myInst);
//                }
//            }

//            if (nameInstance["DemoApp.TodayWriter, af"] is IDateWriter myWriter)
//                myWriter.WriteDate();



//            //using DI.Resolve extension method
//            json = @"
//[
//    {
//    'Name': 'DemoApp.ConsoleOutput, af',
//    'Parameters': []
//    }
//    ,
//    {
//    'Name': 'DemoApp.TodayWriter, af',
//    'Parameters': ['DemoApp.ConsoleOutput, af']
//    }
//]
//";
//            var resolvedInstances = DI.Resolve(json);
//            if (resolvedInstances["DemoApp.TodayWriter, af"] is IDateWriter myWriter2)
//                myWriter2.WriteDate();


//            //using DI.Resolve extension method and pass string parameters
//            json = @"
//[
//    {
//    'Name': 'DemoApp.ConsoleOutput, af',
//    'Parameters': []
//    }
//    ,
//    {
//    'Name': 'DemoApp.TodayWriter, af',
//    'Parameters': ['testing1', 'testing2']
//    }
//]
//";
//            resolvedInstances = DI.Resolve(json);
//            if (resolvedInstances["DemoApp.TodayWriter, af"] is IDateWriter myWriter3)
//                myWriter3.WriteDate();


//            //Could we look up what parameters/types the object has at runtime
//            //to be more in line with what other DI containers do?
//            //So in this case we want to look up the instance based on the interface name
//            //
//            //Added a recursive call to Resolve so if TodayWriter, for example, is passed
//            //into another object's ctor, it will get instantiated
//            json = @"
//[
//    {
//    'Name': 'DemoApp.DummyClass, af',
//    'Interface': 'DemoApp.DummyClass'
//    }
//    ,
//    {
//    'Name': 'DemoApp.ConsoleOutput, af',
//    'Interface': 'DemoApp.IOutput'
//    }
//    ,
//    {
//    'Name': 'DemoApp.TodayWriter, af',
//    'Interface': 'DemoApp.IDateWriter'
//    }
//]
//";
//            resolvedInstances = DI.Resolve(json);
//            if (resolvedInstances["DemoApp.IDateWriter"] is IDateWriter myWriter4)
//                myWriter4.WriteDate();

//            if (resolvedInstances["DemoApp.DummyClass"] is IDummyClass myDummy)
//                myDummy.WriteSomething();


            var json = @"
[
    {
    'Implementation': 'DemoApp.DummyClass, af',
    'Interface': 'DemoApp.IDummyClass'
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
            //cleaned up the code a bit in a separate file and to only use Implementation & Interface
            var resolvedInstances = JsonInjector.Resolve(json);
            if (resolvedInstances["DemoApp.IDateWriter"] is IDateWriter myWriter5)
                myWriter5.WriteDate();

            if (resolvedInstances["DemoApp.IDummyClass"] is IDummyClass myDummy2)
                myDummy2.WriteSomething();

        }
    }

    public interface IOutput
    {
        void Write(string content);
    }

    public class ConsoleOutput : IOutput
    {
        public void Write(string content)
        {
            Console.WriteLine(content);
        }
    }

    public interface IDateWriter
    {
        void WriteDate();
    }

    public class TodayWriter : IDateWriter
    {
        private IOutput? _output;
        string testString = "";
        public TodayWriter(IOutput output)
        {
            this._output = output;
        }

        public TodayWriter(string test1, string test2)
        {
            testString = test1 + ", " + test2;
        }

        public void WriteDate()
        {
            if (_output == null) Console.WriteLine(testString);
            else this._output.Write(DateTime.Today.ToShortDateString());
        }
    }

    public class DummyClass: IDummyClass
    {
        IDateWriter _dw;
        public DummyClass(IDateWriter dw)
        {
            _dw = dw;
        }

        public void WriteSomething()
        {
            Console.WriteLine("Something");
            _dw.WriteDate();
        }
    }

    public interface IDummyClass
    {
        void WriteSomething();
    }




    ////can we create a helper class to do this for us?
    //public static class DI
    //{
    //    public static Dictionary<string, object> Resolve(this string json, Dictionary<string, object>? nameInstance = null)
    //    {
    //        json = json.Replace("'", "\"");

    //        var instances = JsonSerializer.Deserialize<List<Instance>>(json);
    //        if (nameInstance == null) nameInstance = new Dictionary<string, object>();

    //        if (instances == null) return nameInstance;
    //        //We could instantiate objects without constructor parameters first
    //        foreach (var inst in instances)
    //        {
    //            if (inst.Parameters == null && inst.Interface != null)
    //            {
    //                if (inst.Name == null) continue;
    //                //lets check what actually happens in these scenarios
    //                var myType = Type.GetType(inst.Name);
    //                if (myType == null) continue;
    //                var ctors = myType.GetConstructors();
    //                if (ctors != null && ctors.Length > 0)
    //                {
    //                    foreach (var ctor in ctors)
    //                    {
    //                        if (ctor.GetParameters().Length == 0 && !nameInstance.ContainsKey(inst.Interface))
    //                        {
    //                            var myInst = Activator.CreateInstance(myType);
    //                            if (myInst == null) continue;
    //                            nameInstance.Add(inst.Interface, myInst);
    //                        }
    //                    }
    //                }
    //                //I am not sure this will ever happen so will remove for now
    //                //else
    //                //{
    //                //    var myInst = Activator.CreateInstance(myType);
    //                //    nameInstance.Add(inst.Interface, myInst);
    //                //}
    //            }
    //            else if (inst?.Parameters?.Count == 0)
    //            {
    //                if (inst.Name == null) continue;
    //                var myType = Type.GetType(inst.Name);
    //                if (myType != null && !nameInstance.ContainsKey(inst.Name))
    //                {
    //                    var myInst = Activator.CreateInstance(myType);
    //                    if (myInst != null) nameInstance.Add(inst.Name, myInst);
    //                }
    //            }
    //        }

    //        //Now instantiate objects with Parameters in json or in call to GetConstructors.GetParameters
    //        foreach (var inst in instances)
    //        {
    //            if (inst.Parameters == null && inst.Interface != null)
    //            {
    //                if (inst.Name == null) continue;
    //                var myType = Type.GetType(inst.Name);
    //                var ctors = myType?.GetConstructors();
    //                if (ctors != null && ctors.Length > 0)
    //                {
    //                    foreach (var ctor in ctors)
    //                    {
    //                        bool hasAllTypes = true;
    //                        var myParameters = new List<object>();
    //                        foreach (var param in ctor.GetParameters())
    //                        {
    //                            //we need to check if our dictionary has all types
    //                            if (!nameInstance.ContainsKey(param.ParameterType.ToString())) hasAllTypes = false;
    //                            else myParameters.Add(nameInstance[param.ParameterType.ToString()]);
    //                        }
    //                        if (hasAllTypes && myParameters.Count > 0 && myType != null && !nameInstance.ContainsKey(inst.Interface))
    //                        {
    //                            var myInst = Activator.CreateInstance(myType, myParameters.ToArray());
    //                            if (myInst != null) nameInstance.Add(inst.Interface, myInst); //should also use interface I suppose
    //                            break; //we found one so don't try and create another one with same interface key
    //                        }
    //                    }
    //                }
    //            }
    //            else if (inst?.Parameters?.Count > 0)
    //            {
    //                if (inst.Name == null) continue;
    //                var myType = Type.GetType(inst.Name);

    //                var myParameters = new List<object>();
    //                foreach (string name in inst.Parameters)
    //                {
    //                    if (nameInstance.ContainsKey(name)) myParameters.Add(nameInstance[name]);
    //                    else myParameters.Add(name);
    //                }

    //                if (myType != null && !nameInstance.ContainsKey(inst.Name))
    //                {
    //                    var myInst = Activator.CreateInstance(myType, myParameters.ToArray());
    //                    if (myInst != null) nameInstance.Add(inst.Name, myInst);
    //                }
    //            }
    //        }

    //        //While instances.Count > nameInstance.Count and nameInstance.Count != oldNameInstanceCount we'll keep calling resolve recursively on json
    //        int oldNameInstanceCount = -1;
    //        while (instances.Count > nameInstance.Count && nameInstance.Count != oldNameInstanceCount)
    //        {
    //            oldNameInstanceCount = nameInstance.Count;
    //            nameInstance = json.Resolve(nameInstance);
    //        }

    //        return nameInstance;
    //    }
    //}

    //public class Instance
    //{
    //    public string? Name { get; set; }
    //    public string? Interface { get; set; }
    //    public List<string>? Parameters { get; set; }
    //}
}