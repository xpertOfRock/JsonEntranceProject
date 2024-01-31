using Json.Data;
using Json.Interfaces;
using Json.Models;
using Microsoft.EntityFrameworkCore;

namespace Json.Repositories
{
    public class ProcessingRepository : IProcessingRepository
    {
        private readonly ApplicationDbContext _context;

        public ProcessingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddItem(Item item)
        {
            _context.Items.Add(item);
            foreach (var child in item.Children)
            {
                child.ParentId = item.Id;
                AddItem(child);
            }           
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public string ReadJsonFile(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var json = reader.ReadToEnd();               
                return json;
            }
        }

        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0 ? true : false;
        }
        public void DeleteAllData()
        {
            _context.Database.EnsureDeleted(); 
            _context.Database.EnsureCreated();
            _context.SaveChanges();
        }
    }
}
