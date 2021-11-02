using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDBTest
{
  class Program
  {
    static void Main(string[] args)
    {
      MongoCRUD db = new MongoCRUD("AddressBook");
      PersonModel person = new PersonModel
      {
        FirstName = "E",
        LastName = "F",
        PrimaryAddress = new AddressModel { StreetAdress = "Swamsea Road", City = "Swansea", PostCode = "SA111GG" }
      };
      db.InsertRecord("Users", person);

      PersonModel record = new PersonModel();
      var recs = db.loadRecords<PersonModel>("Users");
      foreach(var rec in recs)
      {
        Console.WriteLine($"{rec.ID}: {rec.FirstName} {rec.LastName} ");
        record = rec;
      }


      db.LoadRecordByID<PersonModel>("Users", record.ID);
      Console.WriteLine("Hello World!");
    }
  }
  public class PersonModel
  {
    [BsonId]  //Store as the ID field in the database and its unique
    public Guid ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public AddressModel PrimaryAddress { get; set; }
  }
  public class AddressModel
  {
    public string StreetAdress { get; set; }
    public string City  { get; set; }

    public string PostCode{ get; set; }
  }

  

  public class MongoCRUD
  {
    private IMongoDatabase db;

    public MongoCRUD(string database)
    {
      var client = new MongoClient();
      db = client.GetDatabase(database);
    }

    public void InsertRecord<T>(string table, T record)
    {
      var collection = db.GetCollection<T>(table);
      collection.InsertOne(record);
    }

    public List<T> loadRecords<T>(string table) { 
      var collection = db.GetCollection<T>(table);
      return collection.Find(new BsonDocument()).ToList();
    }

    public T LoadRecordByID<T>(String table, Guid id)
    {
      var collection = db.GetCollection<T>(table);
      var filter = Builders<T>.Filter.Eq("id", id);
      return collection.Find(filter).First();
    }


    public void UpsertRecord<T>(String table, Guid id, T record)
    {
      var collection = db.GetCollection<T>(table);
      var result = collection.ReplaceOne(         
        new BsonDocument("_id", id ),
        record,
        new UpdateOptions { IsUpsert = true });
    }

    public void DeleteRecorsd<T>(String table, Guid id)
    {
      var collection = db.GetCollection<T>(table);
      var filter = Builders<T>.Filter.Eq("id", id);
      collection.DeleteOne(filter);
    }
  }
}
