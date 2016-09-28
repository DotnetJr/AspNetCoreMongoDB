using System;
using MongoDB.Bson.Serialization.Attributes;

namespace M101DotNet.WebApp.Models
{
    [BsonIgnoreExtraElements]
    public class Comment
    {
        // XXX WORK HERE
        // Add in the appropriate properties.
        // The homework instructions have the necessary schema.

        public string Author { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}