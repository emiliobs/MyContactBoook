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

        public ActionResult View(int id)
        {
            Contact c = GetContact(id);


            return View(c);
        }

        //Now Edit Part:
     
        public ActionResult Edit(int id)
        {
            //Fecht Contact:
            Contact contact = GetContact(id);//GetContact i hevr created in the previous part:
            if (contact == null)
            {
                return HttpNotFound("Contact Not Found.!!!");
            }
            //fetch Country & state
            List<Country> countries = new List<Country>();
            List<State> states = new List<State>();

            using (MyContact db = new MyContact ())
            {
                countries = db.Countries.OrderBy(c => c.CountryName).ToList();
                states = db.States.Where(s=>s.CountryId.Equals(contact.CountryId)).OrderBy(s=>s.StateName).ToList();

            }

            ViewBag.Country = new SelectList(countries,"CountryId","CountryName", contact.CountryId);
            ViewBag.State = new SelectList(states,"StateId","StateName",contact.StateId);

            return View(contact);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contact c, HttpPostedFileBase file)
        {
            //fetch country & state for dropdown:
            List<Country> contries = new List<Country>();
            List<State> states = new List<State>();

            using (MyContact db = new MyContact ())
            {
                contries = db.Countries.Where(a=>a.CountryId.Equals(c.CountryId)).OrderBy(a=>a.CountryName).ToList();

                if (c.CountryId > 0)
                {
                    states = db.States.Where(a=>a.CountryId.Equals(c.CountryId)).OrderBy(a=>a.StateName).ToList();
                }

                ViewBag.Country = new SelectList(contries,"CountryId","CountryName",c.CountryId);
                ViewBag.State = new SelectList(states,"StateId","StateName",c.StateId);
            }

            //Validate file is selected:
            if (file != null)
            {
                //if (file.ContentLength > (512 * 100))//512 bb
                //{
                //    ModelState.AddModelError("FileErrorMessage","File size must be within 512KB");
                //}

                string[] allowedType = new string[] { "image/png", "image/jpg", "image/gif","image/jpeg" };
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
                    ModelState.AddModelError("FileErrorMessage","Only .png, .gif and .jpg file allowed.");
                }

            }

            //Update contact:
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string savePath = Server.MapPath("~/img");
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(savePath, fileName));
                    c.Image = fileName;
                }

                using (MyContact db = new MyContact ())
                {
                    var v = db.Contacts.Where(a => a.ContactId.Equals(c.ContactId)).FirstOrDefault();

                    if (v != null)
                    {
                        v.ContactFirstName = c.ContactFirstName;
                        v.ContactLastName = c.ContactLastName;
                        v.Address = c.Address;
                        v.CountryId = c.CountryId;
                        v.StateId = c.StateId;
                        v.Phone1 = c.Phone1;
                        v.Phone2 = c.Phone2;
                        v.Email = c.Email;

                        if (file != null)
                        {
                            v.Image = c.Image;
                        }
                    }

                    
                        db.SaveChanges();
                        
                  
                }

                return RedirectToAction("Index");
               
            }
            else
            {
                return View(c);
            }

           
        }


       [HttpGet]
        public ActionResult Delete(int id)
        {
            //fetch Contact
            Contact contact = GetContact(id);//getcontact is a funcion for get data i have written in the previous part:


            return View(contact);
        }

        //delete post:
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]//here action name is required as we can not make signature for get & post
        public ActionResult DeleteConfirm(int id)
        {

            using (MyContact db = new MyContactBoook.MyContact ())
            {
                var contact = db.Contacts.Where(a=>a.ContactId.Equals(id)).FirstOrDefault();

                if (contact != null)
                {
                    db.Contacts.Remove(contact);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {

                    return HttpNotFound("Contact Not Found.!!");
                }
            }

            
        }

        private Contact GetContact(int contactId)
        {
            Contact contact = null;

            using (MyContact db = new MyContact())
            {
                var v = (
                             from  a in db.Contacts
                             join  b in db.Countries on a.CountryId  equals b.CountryId
                             join  c in db.States on a.StateId equals c.StateId
                             where a.ContactId.Equals(contactId)
                             select new
                             {
                                 a,
                                 b.CountryName,
                                 c.StateName
                             } 
                         ).FirstOrDefault();

                if (v != null)
                {
                    contact = v.a;
                    contact.CountryName = v.CountryName;
                    contact.StateName = v.StateName;
                }
            }

            return contact;
           
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