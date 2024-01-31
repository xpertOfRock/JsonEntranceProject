using Json.Models;

namespace Json.Interfaces
{
    public interface IProcessingRepository
    {
        Task<List<Item>> GetAllItemsAsync();
        string ReadJsonFile(IFormFile file);
        void AddItem(Item item);
        public void DeleteAllData();
        bool Save();
    }
}
