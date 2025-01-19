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
        public IMongoCollection<Category> GetCategoriesCollection() => _database.GetCollection<Category>("Categories");

        public async Task SaveToMongoDBService(List<QuestionPack> questionPacks, List<string> categories)
        {
            var questionPackCollection = GetQuestionPackCollection();
            var categoryCollection = GetCategoriesCollection();

            await questionPackCollection.DeleteManyAsync(_ => true);
            await categoryCollection.DeleteManyAsync(_ => true);

            if (questionPacks.Count > 0)
            {
                await questionPackCollection.InsertManyAsync(questionPacks);
            }

            if (categories.Count > 0)
            {
                var categoryDocuments = categories.Select(c => new Category { Name = c }).ToList();
                await categoryCollection.InsertManyAsync(categoryDocuments);
            }
        }

        public async Task<(List<QuestionPack>, List<string>)> LoadFromMongoDBService()
        {
            var questionPackCollection = GetQuestionPackCollection();
            var categoryCollection = GetCategoriesCollection();

            var questionPacksTask = questionPackCollection.Find(_ => true).ToListAsync();
            var categoriesTask = categoryCollection.Find(_ => true).ToListAsync();

            await Task.WhenAll(questionPacksTask, categoriesTask);

            var questionPacks = await questionPacksTask;
            var categories = await categoriesTask;

            return (questionPacks, categories.Select(c => c.Name).ToList());
        }
    }
}
