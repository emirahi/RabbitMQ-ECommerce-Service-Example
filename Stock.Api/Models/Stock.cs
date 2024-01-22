using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stock.Api.Models;

public class Stock
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    [BsonElement(Order = 0)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.Int32)]
    [BsonElement(Order = 1)]
    public int ProductId { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    [BsonElement(Order = 2)]
    public int Count { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
    [BsonElement(Order = 3)]
    public double Price { get; set; }
}