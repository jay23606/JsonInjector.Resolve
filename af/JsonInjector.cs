using System.Reflection;
using System.Text.Json;

namespace DemoApp
{
    //I think this name makes more sense
    public class Registration
    {
        public string? Interface { get; set; }
        public string? Implementation { get; set; }
    }

    public static class JsonInjector
    {
        public static Dictionary<string, object> Resolve(this string json, Dictionary<string, object>? instanceLookup = null, List<Registration>? registrations = null)
        {
            json = json.Replace("'", "\"");
            if (registrations == null) registrations = JsonSerializer.Deserialize<List<Registration>>(json);
            if (instanceLookup == null) instanceLookup = new Dictionary<string, object>();
            if (registrations == null) return instanceLookup;

            //Instantiate parameterless constructors first
            foreach (var reg in registrations)
            {
                if (reg.Interface == null || reg.Implementation == null || instanceLookup.ContainsKey(reg.Interface)) continue;
                var myType = Type.GetType(reg.Implementation);
                if (myType == null) continue;
                var ctors = myType.GetConstructors();
                if (ctors == null || ctors.Length == 0) continue;
                foreach (var ctor in ctors)
                {
                    if (ctor.GetParameters().Length > 0) continue;
                    var myInst = Activator.CreateInstance(myType);
                    if (myInst != null) instanceLookup.Add(reg.Interface, myInst);
                    break;
                }
            }

            //Instantiate constructors that have parameters
            foreach (var reg in registrations)
            {
                if (reg.Interface == null || reg.Implementation == null || instanceLookup.ContainsKey(reg.Interface)) continue;
                var myType = Type.GetType(reg.Implementation);
                if (myType == null) continue;
                var ctors = myType.GetConstructors();
                if (ctors == null || ctors.Length == 0) continue;
                foreach (var ctor in ctors)
                {
                    bool hasAllTypes = true;
                    var myParams = new List<object>();
                    foreach (var param in ctor.GetParameters())
                    {
                        if (!instanceLookup.ContainsKey(param.ParameterType.ToString())) hasAllTypes = false;
                        else myParams.Add(instanceLookup[param.ParameterType.ToString()]);
                    }
                    if (hasAllTypes && myParams.Count > 0)
                    {
                        var myInst = Activator.CreateInstance(myType, myParams.ToArray());
                        if (myInst != null) instanceLookup.Add(reg.Interface, myInst); 
                        break; 
                    }
                }
            }

            int oldinstanceLookupCount = -1;
            while (registrations.Count > instanceLookup.Count && instanceLookup.Count != oldinstanceLookupCount)
            {
                oldinstanceLookupCount = instanceLookup.Count;
                instanceLookup = json.Resolve(instanceLookup, registrations);
            }

            return instanceLookup;
        }
    }
}
