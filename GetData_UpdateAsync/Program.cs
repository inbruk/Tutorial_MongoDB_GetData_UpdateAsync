using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;


namespace GetData_UpdateAsync
{
    static class Program
    {

        static void Main(String[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("AddressBook");
            var coll = db.GetCollection<Rootobject>("Book");

            var bookId = new ObjectId("61a7ff6b45c6feb20ca1e086");
            var books = coll
                .Find( x => x.publisher==$"O'Reilly Media")
                .SortBy( x=>x.author )
                .Limit(2)
                .ToListAsync()
                .Result;

            foreach (var item in books)
            {
                var iStr = JsonSerializer.Serialize(item);
                Console.WriteLine(iStr);
                Console.WriteLine("-----------------------------------------------------------");
            }

            var first = books.First();
            first.title = first.title.ToUpper();
            coll.SaveAsync(first).Wait();

            // Console.WriteLine("message");
            Console.ReadKey();
        }

        private static async Task<ReplaceOneResult> SaveAsync<T>(this IMongoCollection<T> coll, T entity) 
            where T:IIdentified 
        {
            return await coll.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = true } );
        }
    }

    public interface IIdentified
    {
        public ObjectId Id { get; set; }
    }


    public class Rootobject : IIdentified
    {
        public ObjectId Id { get; set; }
        public string isbn { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string author { get; set; }
        public DateTime published { get; set; }
        public string publisher { get; set; }
        public int pages { get; set; }
        public string description { get; set; }
        public string website { get; set; }
    }
}