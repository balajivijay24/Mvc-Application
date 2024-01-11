using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityManagement;
using IdentityManagement.Models;
using System.Diagnostics;

namespace RequestController.Controllers
{
    public class RequestController : Controller
    {
        private readonly EmployeeDataDbContext db;
        private readonly IWebHostEnvironment _environment;
        
        public RequestController(EmployeeDataDbContext context, IWebHostEnvironment environment)
        {
            db = context;
            _environment = environment;
        }
        
        public async Task<IActionResult> RequestStatus(){ 
      
        var userRequest = await db.Requestss.Where(b=>b.UserId ==Database.mail).ToListAsync();
        return View(userRequest);
        }
  
        [HttpGet]

        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)

        {



            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;


            var list = from selectedlist in db.Requestss
                     select selectedlist;

            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(selectedlist => selectedlist.UserId.Contains(searchString) || selectedlist.Name.Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    list = list.OrderByDescending(selectedlist => selectedlist.Name);
                    break;
                case "Date":
                    list = list.OrderBy(selectedlist => selectedlist.UserId);
                    break;
                case "date_desc":
                    list = list.OrderByDescending(selectedlist => selectedlist.UserId);
                    break;
                default:
                    list = list.OrderBy(selectedlist => selectedlist.UserId);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Request>.CreateAsync(list.AsNoTracking(), pageNumber ?? 1, pageSize));
             return View(await list.AsNoTracking().ToListAsync());
        }



       
        [HttpGet]
        public IActionResult Create()
        {
            Request userrequest = new Request();

            return View(userrequest);
        }

        [HttpPost]
        public async Task<IActionResult>  Create(Request employeerequest, IFormFile file)
        {
            Request userrequest = new Request();
            userrequest.Name=Database.Name;
            userrequest.UserId = Database.mail;
            userrequest.request = employeerequest.request;
            userrequest.ExistingData=employeerequest.ExistingData;
            userrequest.NewData=employeerequest.NewData;
            userrequest.status="Pending";
            db.Requestss.Add(userrequest);
            await db.SaveChangesAsync();
            return RedirectToAction("Create");

        }

         [HttpGet]

        public async Task<IActionResult> Edit(int? id)

        {

            if (id == null)
            {
                return NotFound();
            }

            Request userrequest = await db.Requestss.Where(x => x.ID == id).FirstOrDefaultAsync();

            if (userrequest == null)
            {
                return NotFound();
            }
            return View(userrequest);

        }
        [HttpPost]

        public async Task<IActionResult> Edit(int? id,Request employeerequest, IFormFile file)

        {
           if (id == null)
            {
                return NotFound();

            }


            Request userrequest = await db.Requestss.Where(x => x.ID == id).FirstOrDefaultAsync();



            if (userrequest == null)

            {

                return NotFound();

            }

            // userrequest.Name=employeerequest.Name;
            // userrequest.UserId = employeerequest.UserId;
            // userrequest.request = employeerequest.request;
            // userrequest.ExistingData=employeerequest.ExistingData;
            // userrequest.NewData=employeerequest.NewData;
             userrequest.status=employeerequest.status;
            
                await db.SaveChangesAsync();
                return RedirectToAction("Index");    

        }



       

  [HttpGet]


        public async Task<IActionResult> Detail(int? id)


        {

            if (id == null)

            {

                return NotFound();

            }



            Request userrequest = await db.Requestss.Where(x => x.ID == id).FirstOrDefaultAsync();





            if (userrequest == null)

            {

                return NotFound();

            }



            return View(userrequest);

        }

  [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || db.Requestss == null)
            {
                return NotFound();
            }

            var Requestlist = await db.Requestss
                .FirstOrDefaultAsync(m => m.ID == id);
            if (Requestlist == null)
            {
                return NotFound();
            }

            return View(Requestlist );
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (db.Requestss == null)
            {
                return Problem("Entity set 'EmployeeDataDbContext.Requestss'  is null.");
            }
            var EmployeeDatas = await db.Requestss.FindAsync(id);
            if (EmployeeDatas != null)
            {
                db.Requestss.Remove(EmployeeDatas);
            }
            
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
 
    }
}