using MyContactBoook.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyContactBoook.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            List<ContactModel> contacts = new List<Models.ContactModel>();
            using (MyContact db = new MyContact())
            {
                var contact = (from a in db.Contacts
                               join b in db.Countries on a.CountryId equals b.CountryId
                               join c in db.States on a.StateId equals c.StateId
                               select new ContactModel
                               {
                                   ContactId = a.ContactId,
                                   FirstName = a.ContactFirstName,
                                   LastName = a.ContactLastName,
                                   Phone1 = a.Phone1,
                                   Phone2 =a.Phone2,
                                   Email = a.Email,
                                   Country = b.CountryName,
                                   State =   c.StateName,
                                   Address =a.Address,
                                   Image = a.Image
                                  }).ToList();

                contacts = contact;
            }

            return View(contacts);
        }

        [HttpGet]
      
        public ActionResult Add()
        {
            List<Country> AllCountry = new List<Country>();
            List<State> states = new List<State>();

            using (MyContact db = new MyContact ())
            {
                AllCountry = db.Countries.OrderBy(a=>a.CountryName).ToList();
                
                
           }

            ViewBag.Country = new SelectList(AllCountry,"CountryId","CountryName");
            ViewBag.State = new SelectList(states.OrderBy(s=>s.StateName), "StateId", "StateName");

            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Contact contact, HttpPostedFileBase file)
        {
            #region
            /*//Fetch Country & State*/
            List<Country> AllCountry = new List<Country>();
            List<State> states = new List<State>();

            using (MyContact db = new MyContact())
            {
                AllCountry = db.Countries.OrderBy(c => c.CountryName).ToList();

                if (contact.CountryId > 0)
                {
                    states = db.States.Where(s => s.CountryId.Equals(contact.CountryId)).OrderBy(s => s.StateName).ToList();
                }

                ViewBag.Country = new SelectList(AllCountry, "CountryId", "CountryName", contact.ContactId);
                ViewBag.State = new SelectList(states, "StateId", "StateName", contact.StateId);
            }
            #endregion

            //Validate file if selected:
            if (file != null)
            {
                //if (file.ContentLength > (512 * 100))//512KB
                //{
                //    ModelState.AddModelError("FileErrorMessage", "File size must be withing 512 KB");
                //}

                string[] allowedType = new string[] { "image/png", "image/gif", "image/jpeg", "image/jpg" };

                bool isFileTypeValid = false;

                foreach (var i in allowedType)
                {
                    if (file.ContentType == i.ToString())
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }
                if (!isFileTypeValid)
                {
                    ModelState.AddModelError("FileErrorMessage", "Only .png , .gif and .jpg");
                }


            }


            //validate Model and Save to database:

            if (ModelState.IsValid)
            {
                //Save here:
                if (file != null)
                {
                    string savePatch = Server.MapPath("~/img");
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(savePatch, fileName));
                    contact.Image = fileName;

                }

                using (MyContact db = new MyContact())
                {
                    db.Contacts.Add(contact);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return RedirectToAction("Index");

            }
            else
            {
                return View(contact);
            }

        }


        public JsonResult GetStates(int countryId)
        {
            using (MyContact db = new MyContact ())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var state = (from a in db.States where a.CountryId.Equals(countryId)
                             orderby a.StateName select a).ToList();

                return new JsonResult { Data = state, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

           
        }

        
    }
}