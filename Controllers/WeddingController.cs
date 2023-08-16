using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using wedding_planner.Models;

namespace wedding_planner.Controllers;
[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;

    private MyContext db;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("wedding")]
    public IActionResult Index()
    {

        List<Wedding> weddings = db.Weddings.Include(v => v.RSVPers).ThenInclude(s => s.User).Include(v => v.Creator).ThenInclude(r => r.UserRSVP).ToList();

        return View("All", weddings);
    }


// ======================new=========================
    [HttpGet("wedding/new")]
    public IActionResult New()
    {
        return View();
    }





    // ==============create========
    [HttpPost("wedding/create")]
    public IActionResult Create(Wedding newWed)
    {
        if (!ModelState.IsValid)
        {
            return View("New");
        }
        newWed.UserId = (int)HttpContext.Session.GetInt32("UUID");
        db.Weddings.Add(newWed);
        db.SaveChanges();
        return RedirectToAction("Details", new {id = newWed.WeddingId});
    }








// =====================details====================================
    [HttpGet("wedding/{id}")]
    public IActionResult Details(int id)
    {
        // include all the other dbs
        Wedding? weddings = db.Weddings.Include(v => v.RSVPers).ThenInclude(s => s.User).Include(v => v.Creator).FirstOrDefault(v => v.WeddingId == id);
        if (weddings == null)
        {
            return RedirectToAction("Index");
        }
        return View("Details", weddings);
    }







    // =====================RSVP=====================\
    [HttpPost("wedding/{id}/rsvp")]
    public IActionResult RSVP(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UUID");
        if(userId == null)
        {
            return RedirectToAction("Index");
        }
        // make sure you cant sign up mutiple times
        RSVP? existingRsvp = db.RSVPs.FirstOrDefault(u => u.UserId  == userId.Value && u.WeddingId == id);

        if(existingRsvp != null)
        {
            db.RSVPs.Remove(existingRsvp);
        }
        else
        {
            RSVP newRSVP = new RSVP()
            {
                WeddingId = id,
                UserId = userId.Value
            };
            db.RSVPs.Add(newRSVP);
        }
        db.SaveChanges();
        return RedirectToAction("Index");
    }






// =====================DELETE============================
    [HttpPost("wedding/{id}/delete")]
    public IActionResult Delete(int id)
    {
        // olny lets creator delete
        Wedding? weddings = db.Weddings.Include(v => v.Creator).FirstOrDefault(u => u.WeddingId == id);
        if (weddings == null || weddings.UserId != HttpContext.Session.GetInt32("UUID"))
        {
            return RedirectToAction("Index");
        }
        db.Weddings.Remove(weddings);
        db.SaveChanges();
        return RedirectToAction("Index");

    }






    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}







// Name this anything you want with the word "Attribute" at the end
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {

        int? userId = context.HttpContext.Session.GetInt32("UUID");

        if (userId == null)
        {

            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
