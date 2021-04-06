using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace MomsSpaghetti.Models
{
    public class UserSearch
    {
        public string Query { get; set; }

        public  JObject Result { get; set; }


    }
}
