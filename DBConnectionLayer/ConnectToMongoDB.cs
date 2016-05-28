using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using FluentAssertions;


namespace DBConnectionLayer
{
    public class ConnectToMongoDB
    {
        static IMongoClient _client;
        static IMongoDatabase _dataBase;
        
        public void MongoDBConnection()
        {

            _client = new MongoClient();
            _dataBase = _client.GetDatabase("ServiceDB");
            
            
        }


        public void insertDocumentToDB(BsonDocument Document, string collectionName)
        {

            if(Document != null)
            {

                var collection = _dataBase.GetCollection<BsonDocument>(collectionName);

                collection.InsertOneAsync(Document);

            }
        }
        
        public void updateDocumentInDB (BsonDocument Document, string collectionName, string documentIDTag, string documentID, string tagID)
        {
            var colleciton = _dataBase.GetCollection<BsonDocument>(collectionName);
            //colleciton.update({ documentIDTag: documentID},
            //        {$addToSet: { tagID: { $each:[Document]} } } );

            colleciton.UpdateOne(new BsonDocument { { documentIDTag, documentID } }, Document);
            var query = QueryableExecutionModel.Equals(documentIDTag, documentID );
            //colleciton.FindOneAndUpdate(query, Document);
            
        }

        public int numberOfDocumentsInCollection(string collectionName)
        {
            int numberOfDocuments = 0;

            var collection = _dataBase.GetCollection<BsonDocument>(collectionName);
            //collection = _dataBase.GetCollection<collectionName>();
            numberOfDocuments = Convert.ToInt16( collection.Count(new BsonDocument()));
            //collection.cou
            return numberOfDocuments;
        }



        public void insertTestJson()
        {
            var document = new BsonDocument
            {
                { "address" , new BsonDocument
                    {
                        { "street", "2 Avenue" },
                        { "zipcode", "10075" },
                        { "building", "1480" },
                        { "coord", new BsonArray { 73.9557413, 40.7720266 } }
                    }
                },
                { "borough", "Manhattan" },
                { "cuisine", "Italian" },
                { "grades", new BsonArray
                        {
                            new BsonDocument
                            {
                                { "date", new DateTime(2014, 10, 1, 0, 0, 0, DateTimeKind.Utc) },
                                { "grade", "A" },
                                { "score", 11 }
                            },
                            new BsonDocument
                            {
                                { "date", new DateTime(2014, 1, 6, 0, 0, 0, DateTimeKind.Utc) },
                                { "grade", "B" },
                                { "score", 17 }
                            }
                        }
                 },
                 { "name", "Vella" },
                 { "restaurant_id", "41704620" }
           };

            var collection = _dataBase.GetCollection<BsonDocument>("testOrders");

            collection.InsertOneAsync(document);

        }

        public async Task<IEnumerable<BsonDocument>> findDocument(string collectionName, string findTag, string findValue)
        {
            var collection = _dataBase.GetCollection<BsonDocument>(collectionName);

            var filter = Builders<BsonDocument>.Filter.Eq(findTag, findValue);
            
            
            var list = await collection.Find(new BsonDocument(findTag, findValue)).ToListAsync();

            return list;

        }

        public async Task<IEnumerable<BsonDocument>> findAllDocument(string collectionName)
        {
            var collection = _dataBase.GetCollection<BsonDocument>(collectionName);
            var filter = new BsonDocument();
            //List<BsonDocument> returnList = new List<BsonDocument>();
            var list = await collection.Find(filter).ToListAsync();


            //using (var cursor = await collection.Find(filter).ToCursorAsync())
            //{
            //    while (await cursor.MoveNextAsync())
            //    {
            //        foreach (var doc in cursor.Current)
            //        {
            //            // do something with the documents
            //            //list.Add(doc);
            //            returnList.Add(doc);
            //        }
            //    }
            //}

            
            //return returnList;

            return list;
        }


    }
}
