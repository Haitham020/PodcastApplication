using Microsoft.AspNetCore.Mvc;
using PodcastApplication.Data;

namespace PodcastApplication.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public MenuViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var menuItems = _db.Menus.ToList();
            return View(menuItems);
        }
    }
}
