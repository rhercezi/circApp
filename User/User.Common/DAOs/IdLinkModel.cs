using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace User.Common.DAOs
{
    public class IdLinkModel
    {
        public IdLinkModel()
        {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; }
        public string LinkId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}