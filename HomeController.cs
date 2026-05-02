using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly DbHelper db;

    public HomeController(DbHelper _db)
    {
        db = _db;
    }

    public IActionResult Index()
    {
        ViewBag.Slots = db.GetSlots();
        ViewBag.Vans = db.GetActiveVans();
        return View();
    }

    [HttpPost]
    public IActionResult Park(string vehicleNumber, int slotId)
    {
        db.ParkVan(vehicleNumber, slotId);
        return RedirectToAction("Index");
    }

    public IActionResult Exit(int vanId, int slotId)
    {
        db.ExitVan(vanId, slotId);
        return RedirectToAction("Index");
    }
}