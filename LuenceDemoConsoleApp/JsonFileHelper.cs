using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuenceDemoConsoleApp
{
  
    public class JsonFileHelper
    {

        private string _jsonName;
        private string _path;
        private IConfiguration Configuration { get; set; }
        public JsonFileHelper(string jsonName)
        {
            _jsonName = jsonName;
            if (!jsonName.EndsWith(".json"))
                _path = $"DataSource/{jsonName}.json";
            else
                _path = jsonName;
       
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = _path, ReloadOnChange = true, Optional = true })
            .Build();
        }

        public T Read<T>() => Read<T>("");

        public T Read<T>(string section)
        {
            try
            {
                using (var file = new StreamReader(_path))
                using (var reader = new JsonTextReader(file))
                {
                    var jObj = (JObject)JToken.ReadFrom(reader);
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        var secJt = jObj[section];
                        if (secJt != null)
                        {
                            return JsonConvert.DeserializeObject<T>(secJt.ToString());
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<T>(jObj.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return default(T);
        }

        public List<T> ReadList<T>() => ReadList<T>("");

        public List<T> ReadList<T>(string section)
        {
            try
            {
                using (var file = new StreamReader(_path))
                using (var reader = new JsonTextReader(file))
                {
                    var jObj = (JObject)JToken.ReadFrom(reader);
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        var secJt = jObj[section];
                        if (secJt != null)
                        {
                            return JsonConvert.DeserializeObject<List<T>>(secJt.ToString());
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<List<T>>(jObj.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return default(List<T>);
        }

        public void Write<T>(T t) => Write("", t);


        public void Write<T>(string section, T t)
        {
            try
            {
                JObject jObj;
                using (StreamReader file = new StreamReader(_path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObj = (JObject)JToken.ReadFrom(reader);
                    var json = JsonConvert.SerializeObject(t);
                    if (string.IsNullOrWhiteSpace(section))
                        jObj = JObject.Parse(json);
                    else
                        jObj[section] = JObject.Parse(json);
                }

                using (var writer = new StreamWriter(_path))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jObj.WriteTo(jsonWriter);
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public void Remove(string section)
        {
            try
            {
                JObject jObj;
                using (StreamReader file = new StreamReader(_path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObj = (JObject)JToken.ReadFrom(reader);
                    jObj.Remove(section);
                }

                using (var writer = new StreamWriter(_path))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jObj.WriteTo(jsonWriter);
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
    }
}
