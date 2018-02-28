using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisCaching
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisConnection"]);
            IDatabase cache = connectionMultiplexer.GetDatabase();

            //simple set to cache
            cache.StringSet("Key1", "ValueforKey1");
            cache.StringSet("Key2", 10);

            var key1 = cache.StringGet("Key1");
            var key2 = (int)cache.StringGet("Key2");

            // redis cache-aside pattern.

            var key3Check1 = cache.StringGet("Key3");
            if(!key3Check1.HasValue)
            {
                cache.StringSet("Key3", "Set Cache Key 3");
            }
            var key3Check2 = cache.StringGet("Key3");

            string key4Check1 = cache.StringGet("Key4");
            if (key4Check1 != null)
            {
                cache.StringSet("Key4", "Set Cache Key 4");
            }
            var key4Check2 = cache.StringGet("Key4");

            cache.StringSet("Key5", "expire based on duration",new TimeSpan(0,0,200));

            cache.StringSet("Key6", JsonConvert.SerializeObject(new Employee(1, "Mark", "Law")));

            var mark = JsonConvert.DeserializeObject<Employee>(cache.StringGet("Key5"));
 
        }

        public class Employee
        {
            public int EmpNo { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public Employee(int n, string f, string l)
            {
                this.EmpNo = n;
                this.FirstName = f;
                this.LastName = l;
            }
        }
    }
}
