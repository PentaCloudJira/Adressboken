using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Kund
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Namn { get; set; }
   
    public string? Adress { get; set; }
    public string? Telefonnummer { get; set; }

    public string? Email {get; set;}

    public string? Bilmodell {get; set; }

    public string? Årsmodell {get; set;}

    public bool ReparationKlar {get; set;}
}

