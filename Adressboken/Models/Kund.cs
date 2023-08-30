using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class Kund
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Namn { get; set; }
   
    public string? Adress { get; set; }

    [RegularExpression(@"^[0-9-]+$", ErrorMessage = "Kan endast innehålla siffror och tecken.")]
    public string? Telefonnummer { get; set; }

    [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
    public string? Email {get; set;}

    public string? Bilmodell {get; set; }

    public string? Årsmodell {get; set;}

    public bool ReparationKlar {get; set;}
}

