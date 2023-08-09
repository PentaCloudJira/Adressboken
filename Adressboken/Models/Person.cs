using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Person
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Förnamn { get; set; }
    public string Efternamn { get; set; }
    public string Adress { get; set; }
    public string Telefonnummer { get; set; }
}

