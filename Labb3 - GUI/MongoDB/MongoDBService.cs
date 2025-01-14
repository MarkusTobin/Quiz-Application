using Labb3___GUI.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Labb3___GUI.MongoDB
{
    class MongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<QuestionPack> GetQuestionPackCollection() => _database.GetCollection<QuestionPack>("QuestionPacks");

        public IMongoCollection<Question> Questions => _database.GetCollection<Question>("Questions");


        public async Task SaveToMongoDBService(List<QuestionPack> packs)
        {

            var questionPackCollection = GetQuestionPackCollection();
            // om jag lägger till nya saker kanske denna behöver justeras
            await questionPackCollection.DeleteManyAsync(_ => true);
            if (packs.Count == 0)
            {
                return;
            }
            await questionPackCollection.InsertManyAsync(packs);

        }

        public async Task<List<QuestionPack>> LoadFromMongoDBService()
        {
            var questionPackCollection = GetQuestionPackCollection();
            return await questionPackCollection.Find(_ => true).ToListAsync();
        }
    }
}
