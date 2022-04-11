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
            //NORMAL WAY
            IDateWriter tw = new TodayWriter(new ConsoleOutput());
            tw.WriteDate();


            //USING AUTOFAC
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleOutput>().AsImplementedInterfaces();//.As<IOutput>();
            //builder.RegisterType<TodayWriter>().SingleInstance();
            builder.RegisterType<TodayWriter>().AsImplementedInterfaces();//.As<IDateWriter>();
            var container = builder.Build();
            var writer = container.Resolve<IDateWriter>(); //with AsImplementedInterfaces
            //var writer = container.Resolve<TodayWriter>(); //with SingleInstance
            writer.WriteDate();


            //USING CreateInstance
            var co = Activator.CreateInstance(typeof(ConsoleOutput)) as IOutput;
            var instance = Activator.CreateInstance(typeof(TodayWriter), co) as IDateWriter;
            instance.WriteDate();

            //USING CreateInstance with parameters array
            List<object> parameters = new List<object>();
            co = Activator.CreateInstance(typeof(ConsoleOutput)) as IOutput;
            parameters.Add(co);
            instance = Activator.CreateInstance(typeof(TodayWriter), parameters.ToArray()) as IDateWriter;
            instance.WriteDate();


            //USING CreateInstance and Type.GetType (so that parameters could be stored in xml,json for example)
            Type type = Type.GetType("DemoApp.ConsoleOutput, af");
            co = Activator.CreateInstance(type) as IOutput;
            type = Type.GetType("DemoApp.TodayWriter, af");
            parameters = new List<object>();
            parameters.Add(co);
            //instance = Activator.CreateInstance(type, co) as IDateWriter;
            instance = Activator.CreateInstance(type, parameters.ToArray()) as IDateWriter;
            instance.WriteDate();


            //we could look up the parameters from the fully qualified name here in a dictionary
            var json = @"
[
    {
    'Name': 'DemoApp.ConsoleOutput, af',
    'Parameters': []
    }
    ,
    {
    'Name': 'DemoApp.TodayWriter, af',
    'Parameters': ['DemoApp.ConsoleOutput, af']
    }
]
".Replace("'", "\""); //System.Text.Json.JsonSerializer.Deserialize doesn't like single quotes

            var instances = JsonSerializer.Deserialize<List<Instance>>(json);
            Dictionary<string, object> nameInstance = new Dictionary<string, object>();

            //We could instantiate objects without constructor parameters first
            foreach (var inst in instances)
            {
                if (inst.Parameters.Count == 0)
                {
                    var myType = Type.GetType(inst.Name);
                    var myInst = Activator.CreateInstance(myType);
                    //myInst = Convert.ChangeType(myInst, myType);
                    nameInstance.Add(inst.Name, myInst);
                }
            }

            //Now instantiate objects that do require parameters using dictionary
            foreach (var inst in instances)
            {
                if (inst.Parameters.Count > 0)
                {
                    var myType = Type.GetType(inst.Name);

                    var myParameters = new List<object>();
                    foreach (string name in inst.Parameters) myParameters.Add(nameInstance[name]);

                    var myInst = Activator.CreateInstance(myType, myParameters.ToArray());
                    //myInst = Convert.ChangeType(myInst, myType);
                    nameInstance.Add(inst.Name, myInst);
                }
            }

            if (nameInstance["DemoApp.TodayWriter, af"] is IDateWriter myWriter)
                myWriter.WriteDate();



            //using DI.Resolve extension method
            json = @"
[
    {
    'Name': 'DemoApp.ConsoleOutput, af',
    'Parameters': []
    }
    ,
    {
    'Name': 'DemoApp.TodayWriter, af',
    'Parameters': ['DemoApp.ConsoleOutput, af']
    }
]
";
            var resolvedInstances = json.Resolve();
            if (resolvedInstances["DemoApp.TodayWriter, af"] is IDateWriter myWriter2)
                myWriter2.WriteDate();


            //using DI.Resolve extension method
            json = @"
[
    {
    'Name': 'DemoApp.ConsoleOutput, af',
    'Parameters': []
    }
    ,
    {
    'Name': 'DemoApp.TodayWriter, af',
    'Parameters': ['testing1', 'testing2']
    }
]
";
            resolvedInstances = json.Resolve();
            if (resolvedInstances["DemoApp.TodayWriter, af"] is IDateWriter myWriter3)
                myWriter3.WriteDate();

        }
    }



    //can we create a helper class to do this for us?
    public static class DI
    {
        public static Dictionary<string, object> Resolve(this string json)
        {
            json = json.Replace("'", "\"");

            var instances = JsonSerializer.Deserialize<List<Instance>>(json);
            Dictionary<string, object> nameInstance = new Dictionary<string, object>();

            //We could instantiate objects without constructor parameters first
            foreach (var inst in instances)
            {
                if (inst.Parameters.Count == 0)
                {

                    var myType = Type.GetType(inst.Name);
                    var myInst = Activator.CreateInstance(myType);
                    nameInstance.Add(inst.Name, myInst);
                }
            }

            //Now instantiate objects that do require parameters using dictionary
            foreach (var inst in instances)
            {
                if (inst.Parameters.Count > 0)
                {
                    var myType = Type.GetType(inst.Name);

                    var myParameters = new List<object>();
                    foreach (string name in inst.Parameters)
                    {
                        if(nameInstance.ContainsKey(name))myParameters.Add(nameInstance[name]);
                        else myParameters.Add(name);
                    }

                    var myInst = Activator.CreateInstance(myType, myParameters.ToArray());
                    nameInstance.Add(inst.Name, myInst);
                }
            }
            return nameInstance;
        }
    }



    public class Instance
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; }
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
}