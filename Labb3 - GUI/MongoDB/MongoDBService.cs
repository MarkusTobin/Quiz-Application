using Labb3___GUI.Model;
using MongoDB.Driver;

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

        public async Task SaveToMongoDBService(List<QuestionPack> questionPacks)
        {

            var questionPackCollection = GetQuestionPackCollection();

            await questionPackCollection.DeleteManyAsync(_ => true);
            if (questionPacks.Count == 0)
            {
                return;
            }
            await questionPackCollection.InsertManyAsync(questionPacks);
        }

        public async Task<List<QuestionPack>> LoadFromMongoDBService()
        {
            var questionPackCollection = GetQuestionPackCollection();
            return await questionPackCollection.Find(_ => true).ToListAsync();
        }
    }
}
