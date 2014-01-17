using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;

namespace KeyHub.Web.ViewModels.CustomerApp
{
    /// <summary>
    /// Viewmodel for creating an CustomerApp
    /// </summary>
    public class CustomerAppCreateEditModel : RedirectUrlModel
    {
        public Guid? ApplicationId { get; set; }

        [Required]
        [StringLength(256)]
        public string ApplicationName { get; set; }

        [Required]
        public List<Guid> SelectedLicenseGUIDs { get; set; }

        /// <summary>
        /// List of licenses to select
        /// </summary>
        public IEnumerable<SelectListItem> LicenseList { get; set; }

        public static CustomerAppCreateEditModel ForCreate(IDataContextByUser context)
        {
            CustomerAppCreateEditModel viewModel;

            var availableLicenses = (from x in context.Licenses select x)
                .Include(x => x.Sku)
                .Include(x => x.OwningCustomer)
                .ToList();

            viewModel = new CustomerAppCreateEditModel()
            {
                LicenseList = availableLicenses.Select(l => new SelectListItem()
                {
                    Text = String.Format("{0} owned by {1}", l.Sku.SkuCode, l.OwningCustomer.Name),
                    Value = l.ObjectId.ToString()
                }).ToList(),
                SelectedLicenseGUIDs = new List<Guid>()
            };

            return viewModel;
        }

        public static CustomerAppCreateEditModel ForEdit(IDataContextByUser context, Guid key)
        {
            var model = ForCreate(context);

            var customerApp = context.CustomerApps.Where(a => a.CustomerAppId == key)
                .Include(a => a.LicenseCustomerApps)
                .SingleOrDefault();

            if (customerApp == null)
                return null;

            model.ApplicationId = customerApp.CustomerAppId;
            model.ApplicationName = customerApp.ApplicationName;
            model.SelectedLicenseGUIDs = customerApp.LicenseCustomerApps.Select(lca => lca.LicenseId).ToList();

            return model;
        }

        public CustomerAppCreateEditModel WithUserInput(CustomerAppCreateEditModel inputModel)
        {
            ApplicationName = inputModel.ApplicationName;
            SelectedLicenseGUIDs = inputModel.SelectedLicenseGUIDs;
            RedirectUrl = inputModel.RedirectUrl;
            return this;
        }

        public bool TryToSaveCustomerApp(IDataContextByUser context, Action<string, string> modelErrorAccumulator)
        {
            Model.CustomerApp customerApp;

            if (!ApplicationId.HasValue)
            {
                customerApp = new Model.CustomerApp();
                customerApp.CustomerAppKeys.Add(new CustomerAppKey() {});

                context.CustomerApps.Add(customerApp);
            }
            else
            {
                customerApp = context.CustomerApps.Where(a => a.CustomerAppId == ApplicationId.Value).SingleOrDefault();
            }

            customerApp.ApplicationName = ApplicationName;

            var allowedLicenses =
                context.Licenses.Where(l => SelectedLicenseGUIDs.Contains(l.ObjectId)).ToArray();

            if (SelectedLicenseGUIDs.Count() != allowedLicenses.Count())
            {
                modelErrorAccumulator("", "Attempted to license application with unrecognized or unpermitted license.");
            }
            else
            {
                customerApp.LicenseCustomerApps.Clear();
                customerApp.LicenseCustomerApps.AddRange(
                    allowedLicenses.Select(lid => new LicenseCustomerApp()
                    {
                        CustomerApp = customerApp,
                        License = lid
                    }));

                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch (BusinessRuleValidationException ex)
                {
                    foreach (var error in ex.ValidationResults.Where(x => x != BusinessRuleValidationResult.Success))
                    {
                        modelErrorAccumulator("CustomerApp." + error.PropertyName, error.ErrorMessage);
                    }
                }
            }

            return false;
        }
    }
}