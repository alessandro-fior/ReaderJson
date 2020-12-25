using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Student
    {
        [EmailAddress]
        [JsonProperty("email", Required = Required.Always)]
        public string Email;

        public int ID { get; set; }
    }

    public partial class JsonWebform : System.Web.UI.Page
    {
        private void JSONSerilaize()
        {
            // Serializaion
            Employee empObj = new Employee();
            empObj.ID = 1;
            empObj.Name = "Manas";
            empObj.Address = "India";
            //empObj.DOB = DateTime.Now;

            // Convert Employee object to JOSN string format 
            string jsonData = JsonConvert.SerializeObject(empObj);

            Response.Write(jsonData);
        }

        private void JSONDeserilaize()
        {
            string json = @"{
                'ID': '1',
                'Name': 'Manas',
                'Address': 'India'
            }";

            Employee empObj = JsonConvert.DeserializeObject<Employee>(json);

            Response.Write(empObj.Name);
        }

        private void LINQtoJSON()
        {

            string data = @"{
                'Name1': 'Manas',
                'Name': 'Manas',
               'Languages': [
                 'C',
                 'C++',
                 'PHP',
                 'Java',
                 'C#'
               ]
             }";
            JObject studentObj = JObject.Parse(data);

            string name = (string)studentObj["Name"];
            string firstLanguage = (string)studentObj["Languages"][0];
            List<string> allDrives = studentObj["Languages"].Where(temp => temp.ToString().Contains("C")).Select(t => (string)t).ToList();

            JArray array = new JArray();
            array.Add("Manual text");
            array.Add(new DateTime(2000, 5, 23));

            JObject jObj = JObject.Parse(data);
            var url = (string)jObj.Descendants()
                                .OfType<JProperty>()
                                .Where(p => p.Name == "Name")
                                .First()
                                .Value;

            JObject o = new JObject();
            o["MyArray"] = array;

            string json = o.ToString();

            string json1 = @"[
                        'Small',
                        'Medium',
                        'Large']";
            JArray a = JArray.Parse(json1);
        }

        private void ValidateJSON()
        {
            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'language': {'type': 'array'}
              }
            }");

            JObject user = JObject.Parse(@"{
                'name':null,              
                'language': 'manas'
            }");

            bool valid = user.IsValid(schema); // False

            user = JObject.Parse(@"{
                'name':'manas',              
                'language': ['C', 'C++']
            }");

            valid = user.IsValid(schema); // True
        }

        private void GenerateSchema()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Student));
        }

        private void ValidateDeserialization()
        {
            JSchema schema = JSchema.Parse(@"{
              'type': 'array',
              'item': {'type':'string'}
            }");

            JsonTextReader reader = new JsonTextReader(new StringReader(@"[
              'Developer',
              'Administrator'
            ]"));

            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.Schema = schema;

            JsonSerializer serializer = new JsonSerializer();
            List<string> roles = serializer.Deserialize<List<string>>(validatingReader);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            JSONSerilaize();
            JSONDeserilaize();
            LINQtoJSON();
            ValidateJSON();
            GenerateSchema();
            ValidateDeserialization();
        }
    }
}