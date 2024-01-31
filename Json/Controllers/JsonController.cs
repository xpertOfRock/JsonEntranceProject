using Json.Interfaces;
using Json.Models;
using Json.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Json.Controllers
{
    public class JsonController : Controller
    {
        private readonly IProcessingRepository _processingRepository;
        public JsonController(IProcessingRepository processingRepository)
        {
            _processingRepository = processingRepository;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(InputFileViewModel inputFileViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "An error occured during the loading of the file.");
            }
            
            try
            {
                _processingRepository.DeleteAllData();
                _processingRepository.Save();
                var fileContent = _processingRepository.ReadJsonFile(inputFileViewModel.File);
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(fileContent);
                if (items == null)
                {
                    ModelState.AddModelError("", "Object is empty.");
                }
                foreach(var item in items)
                {
                    _processingRepository.AddItem(item);
                }
                _processingRepository.Save();
            } catch (Exception ex)
            {
                ModelState.AddModelError("","An error occured during the uploading of the file.");
                return View("Index");
            }
            return View("Index");
        }
        public List<Item> BuildHierarchy(List<Item> flatItems)
        {
            Dictionary<int, Item> itemDictionary = new Dictionary<int, Item>();


            foreach (var item in flatItems)
            {
                item.Children = new List<Item>();
                itemDictionary[item.Id] = item;
            }


            foreach (var item in flatItems)
            {
                if (item.ParentId.HasValue && itemDictionary.ContainsKey(item.ParentId.Value))
                {
                    var parent = itemDictionary[item.ParentId.Value];
                    parent.Children.Add(item);
                }
            }

            return flatItems.Where(item => !item.ParentId.HasValue).ToList();
        }
        [HttpGet]
        public async Task<IActionResult> GetHierarchyJson()
        {

            var flatItems = await _processingRepository.GetAllItemsAsync();
            var hierarchy = BuildHierarchy(flatItems);
           
            var jsonResult = new JsonResult(hierarchy)
            {
                SerializerSettings = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }
            };

            return jsonResult;
        }        
    }
}
