using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;
using KeyHub.Model;
using KeyHub.Web.ViewModels.UserObjectRight;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    [Authorize]
    public class AccountRightsController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public AccountRightsController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Show list of rights per user
        /// </summary>
        /// <param name="userId">Id if the user to show rights for</param>
        /// <returns>Index view of specific users rights</returns>
        [ChildActionOnly]
        public ActionResult IndexPartial(int userId)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var objectRightsQuery = (from x in context.UserVendorRights where x.UserId == userId select x as UserObjectRight)
                    .Union(from x in context.UserCustomerRights where x.UserId == userId select x as UserObjectRight)
                    .Union(from x in context.UserLicenseRights where x.UserId == userId select x as UserObjectRight)
                    .Include(x => x.Right);

                var viewModel = new UserObjectRightIndexViewModel(context.GetUser(HttpContext.User.Identity), userId, objectRightsQuery.ToList());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a new UserObjectRight
        /// </summary>
        /// <param name="userId">Id of the user to create right for</param>
        /// <param name="objectType">The type of entity to create a right for</param>
        /// <exception cref="NotImplementedException">NotImplementedException if ObjectType is unhandled</exception>
        /// <returns>UserObjectRightCreateViewModel</returns>
        public ActionResult Create(int userId, ObjectTypes objectType)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                UserObjectRight userObjectRight = NewUserObjectRight(objectType);
                SelectList objectList = null;
                Model.Right right = null;
                switch(objectType)
                {
                    case ObjectTypes.Vendor:
                        objectList = (from v in context.Vendors select v).ToSelectList(x => x.ObjectId, x => x.Name);
                        right = (from x in context.Rights where x.RightId == Model.VendorAdmin.Id select x).FirstOrDefault();
                        break;
                    case ObjectTypes.Customer:
                        objectList = (from c in context.Customers select c).ToSelectList(x => x.ObjectId, x => x.Name);
                        right = (from x in context.Rights where x.RightId == Model.EditEntityMembers.Id select x).FirstOrDefault();
                        break;
                    case ObjectTypes.License:
                        objectList = (from l in context.Licenses select new {Id = l.ObjectId, Name = l.Sku.SkuCode}).ToSelectList(x => x.Id, x => x.Name);
                        right = (from x in context.Rights where x.RightId == Model.EditLicenseInfo.Id select x).FirstOrDefault();
                        break;
                    default:
                        throw new NotImplementedException("ObjectType not known");
                }

                userObjectRight.User = (from x in context.Users where x.UserId == userId select x).FirstOrDefault();
                userObjectRight.UserId = userId;
                userObjectRight.Right = right;
                userObjectRight.RightId = right.RightId;

                var viewModel = new UserObjectRightCreateViewModel(userObjectRight, objectType, objectList);

                viewModel.UseLocalReferrerAsRedirectUrl(Request);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created UserObjectRight into db and redirect to account overview
        /// </summary>
        /// <param name="viewModel">Created UserObjectRightCreateViewModel</param>
        /// <returns>Redirectaction to account overview if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(UserObjectRightCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var userObjectRight = viewModel.ToEntity(null);

                    if (userObjectRight is UserVendorRight)
                        context.UserVendorRights.Add(userObjectRight as UserVendorRight);
                    else if (userObjectRight is UserCustomerRight)
                        context.UserCustomerRights.Add(userObjectRight as UserCustomerRight);
                    else if (userObjectRight is UserLicenseRight)
                        context.UserLicenseRights.Add(userObjectRight as UserLicenseRight);

                    context.SaveChanges();
                    Flash.Success(String.Format("Successfully granted {0} rights to {1}.", viewModel.ObjectType, viewModel.Email));
                }

                if (!string.IsNullOrEmpty(viewModel.RedirectUrl))
                {
                    return Redirect(viewModel.RedirectUrl);
                }
                else
                {
                    return RedirectToAction("Edit", "Account", new { id = viewModel.UserId});
                }
            }
            return Create(viewModel.UserId, viewModel.ObjectType);
        }

        public class UserObjectRightViewModel
        {
            public int UserId { get; set; }
            public Guid RightId { get; set; }
            public Guid ObjectId { get; set; }
            public ObjectTypes Type { get; set; }

            public string UserEmail { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
        }

        [Authorize]
        public ActionResult Delete(int userId, Guid rightId, Guid objectId, ObjectTypes type)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var model = new UserObjectRightViewModel()
                {
                    UserId = userId,
                    RightId = rightId,
                    ObjectId = objectId,
                    Type = type,
                    UserEmail = context.Users.Single(u => u.UserId == userId).Email
                };

                switch (type)
                {
                    case ObjectTypes.Vendor:
                        var vendorRight = (from x in context.UserVendorRights 
                            where x.UserId == userId && x.RightId == rightId && x.ObjectId == objectId select x)
                            .Include(r => r.Vendor)
                            .FirstOrDefault();

                        model.Name = vendorRight.Vendor.Name;
                        model.Url = "/Vendor/Details?key=" + objectId;
                        break;
                    case ObjectTypes.Customer:
                        var customerRight = (from x in context.UserCustomerRights 
                            where x.UserId == userId && x.RightId == rightId && x.ObjectId == objectId select x)
                            .Include(r => r.Customer)
                            .FirstOrDefault();

                        model.Name = customerRight.Customer.Name;
                        model.Url = "/Customer/Edit?key=" + objectId;
                        break;
                    case ObjectTypes.License:
                        var licenseRight = (from x in context.UserLicenseRights 
                            where x.UserId == userId && x.RightId == rightId && x.ObjectId == objectId select x)
                            .Include(r => r.License)
                            .Include(r => r.License.Sku)
                            .FirstOrDefault();

                        model.Name = licenseRight.License.Sku.SkuCode;
                        model.Url = "/License/Details?key=" + objectId;
                        break;
                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(UserObjectRightViewModel model)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var userId = model.UserId;
                var rightId = model.RightId;
                var objectId = model.ObjectId;
                
                switch (model.Type)
                {
                    case ObjectTypes.Vendor:
                        var vendorRight = (from x in context.UserVendorRights
                                           where x.UserId == userId && x.RightId == rightId && x.ObjectId == objectId
                                           select x)
                            .Include(r => r.Vendor)
                            .FirstOrDefault();

                        context.UserVendorRights.Remove(vendorRight);
                        break;
                    case ObjectTypes.Customer:
                        var customerRight = (from x in context.UserCustomerRights
                                             where x.UserId == userId && x.RightId == rightId && x.ObjectId == objectId
                                             select x)
                            .Include(r => r.Customer)
                            .FirstOrDefault();

                        context.UserCustomerRights.Remove(customerRight);
                        break;
                    case ObjectTypes.License:
                        var licenseRight = (from x in context.UserLicenseRights
                                            where x.UserId == userId && x.RightId == rightId && x.ObjectId == objectId
                                            select x)
                            .Include(r => r.License)
                            .Include(r => r.License.Sku)
                            .FirstOrDefault();

                        context.UserLicenseRights.Remove(licenseRight);
                        break;
                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                context.SaveChanges();

                return RedirectToAction("Edit", "Account", new {id = userId});
            }            
        }
        
        /// <summary>
        /// Create a new instance of an UserObjectRight based on the provided objectType
        /// </summary>
        /// <param name="objectType">Type from <c>ObjectTypes</c> of object to create</param>
        /// <returns>New instance of UserObjectRight</returns>
        private UserObjectRight NewUserObjectRight(ObjectTypes objectType)
        {
            switch (objectType)
            {
                case ObjectTypes.Vendor:
                    return new Model.UserVendorRight();
                case ObjectTypes.Customer:
                    return new Model.UserCustomerRight();
                case ObjectTypes.License:
                    return new Model.UserLicenseRight();
                default:
                    throw new NotImplementedException("ObjectType not known");
            }
        }
    }
}
