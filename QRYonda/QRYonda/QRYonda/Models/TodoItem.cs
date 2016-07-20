using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace QRYonda.Models
{
    public class TodoItem
    {
        string id;
        string name;
        string image;
        string url;
        bool done;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "text")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "image")]
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        [JsonProperty(PropertyName = "url")]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        [JsonProperty(PropertyName = "complete")]
        public bool Done
        {
            get { return done; }
            set { done = value; }
        }

        [Version]
        public string Version { get; set; }
    }
}

