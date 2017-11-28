using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Text { get; set; }
    }
}