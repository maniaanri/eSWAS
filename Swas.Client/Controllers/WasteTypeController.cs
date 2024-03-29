﻿namespace Swas.Client.Controllers
{
    using Swas.Business.Logic.Classes;
    using Swas.Business.Logic.Entity;
    using Swas.Client.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class WasteTypeController : Controller
    {
        // GET: Waste
        public ActionResult Index()
        {
            var bussinessLogic = new WasteTypeBusinessLogic();

            try
            {
                var wasteTipeDataContext = bussinessLogic.Load();
                var model = new List<WasteTypeViewModel>();

                foreach (var wasteType in wasteTipeDataContext)
                    model.Add(new WasteTypeViewModel
                    {
                        Id = wasteType.Id,
                        Name = wasteType.Name,
                        LessQuantity = wasteType.LessQuantity,
                        FromQuantity = wasteType.FromQuantity,
                        EndQuantity = wasteType.EndQuantity,
                        MoreQuantity = wasteType.MoreQuantity,
                        MunicipalityLessQuantityPrice = wasteType.MunicipalityLessQuantityPrice,
                        MunicipalityIntervalQuantityPrice = wasteType.MunicipalityIntervalQuantityPrice,
                        MunicipalityMoreQuantityPrice = wasteType.MunicipalityMoreQuantityPrice,
                        LegalPersonLessQuantityPrice = wasteType.LegalPersonLessQuantityPrice,
                        LegalPersonIntervalQuantityPrice = wasteType.LegalPersonIntervalQuantityPrice,
                        LegalPersonMoreQuantityPrice = wasteType.LegalPersonMoreQuantityPrice,
                        PhysicalPersonLessQuantityPrice = wasteType.PhysicalPersonLessQuantityPrice,
                        PhysicalPersonIntervalQuantityPrice = wasteType.PhysicalPersonIntervalQuantityPrice,
                        PhysicalPersonMoreQuantityPrice = wasteType.PhysicalPersonMoreQuantityPrice,
                        Coeficient = wasteType.Coeficient,
                    });

                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
            finally
            {
                bussinessLogic = null;
            }

        }

        //Get Waste/Create
        public ActionResult Create()
        {
            try
            {

                //var regionItemSource = 

                var NewItem = new WasteTypeViewModel()
                {
                    Name = string.Empty,
                    LessQuantity = 0M,
                    FromQuantity = 0M,
                    EndQuantity = 0M,
                    MoreQuantity = 0M,
                    MunicipalityLessQuantityPrice = 0M,
                    MunicipalityIntervalQuantityPrice = 0M,
                    MunicipalityMoreQuantityPrice = 0M,
                    LegalPersonLessQuantityPrice = 0M,
                    LegalPersonIntervalQuantityPrice = 0M,
                    LegalPersonMoreQuantityPrice = 0M,
                    PhysicalPersonLessQuantityPrice = 0M,
                    PhysicalPersonIntervalQuantityPrice = 0M,
                    PhysicalPersonMoreQuantityPrice = 0M,
                    Coeficient = 0M
                };

                return View(NewItem);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // POST: Waste/Create
        [HttpPost]
        public JsonResult Create(string name, decimal lessQuantity, decimal fromQuantity, decimal endQuantity, decimal moreQuantity,
                                    decimal municipalityLessQuantityPrice, decimal municipalityIntervalQuantityPrice, decimal municipalityMoreQuantityPrice,
                                    decimal legalPersonLessQuantityPrice, decimal legalPersonIntervalQuantityPrice, decimal legalPersonMoreQuantityPrice,
                                    decimal physicalPersonLessQuantityPrice, decimal physicalPersonIntervalQuantityPrice, decimal physicalPersonMoreQuantityPrice,
                                    decimal coeficient)
        {
            var bussinessLogic = new WasteTypeBusinessLogic();

            try
            {




                bussinessLogic.Create(new WasteTypeItem
                {
                    Name = name,
                    LessQuantity = lessQuantity,
                    FromQuantity = fromQuantity,
                    EndQuantity = endQuantity,
                    MoreQuantity = moreQuantity,
                    MunicipalityLessQuantityPrice = municipalityLessQuantityPrice,
                    MunicipalityIntervalQuantityPrice = municipalityIntervalQuantityPrice,
                    MunicipalityMoreQuantityPrice = municipalityMoreQuantityPrice,
                    LegalPersonLessQuantityPrice = legalPersonLessQuantityPrice,
                    LegalPersonIntervalQuantityPrice = legalPersonIntervalQuantityPrice,
                    LegalPersonMoreQuantityPrice = legalPersonMoreQuantityPrice,
                    PhysicalPersonLessQuantityPrice = physicalPersonLessQuantityPrice,
                    PhysicalPersonIntervalQuantityPrice = physicalPersonIntervalQuantityPrice,
                    PhysicalPersonMoreQuantityPrice = physicalPersonMoreQuantityPrice,
                    Coeficient = coeficient
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bussinessLogic = null;
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        // GET: Waste/Edit/5
        public ActionResult Edit(int Id)
        {
            var bussinessLogic = new WasteTypeBusinessLogic();

            try
            {
                var wasteType = bussinessLogic.Get(Id);

                var model = new WasteTypeViewModel
                {
                    Id = wasteType.Id,
                    Name = wasteType.Name,
                    LessQuantity = wasteType.LessQuantity,
                    FromQuantity = wasteType.FromQuantity,
                    EndQuantity = wasteType.EndQuantity,
                    MoreQuantity = wasteType.MoreQuantity,
                    MunicipalityLessQuantityPrice = wasteType.MunicipalityLessQuantityPrice,
                    MunicipalityIntervalQuantityPrice = wasteType.MunicipalityIntervalQuantityPrice,
                    MunicipalityMoreQuantityPrice = wasteType.MunicipalityMoreQuantityPrice,
                    LegalPersonLessQuantityPrice = wasteType.LegalPersonLessQuantityPrice,
                    LegalPersonIntervalQuantityPrice = wasteType.LegalPersonIntervalQuantityPrice,
                    LegalPersonMoreQuantityPrice = wasteType.LegalPersonMoreQuantityPrice,
                    PhysicalPersonLessQuantityPrice = wasteType.PhysicalPersonLessQuantityPrice,
                    PhysicalPersonIntervalQuantityPrice = wasteType.PhysicalPersonIntervalQuantityPrice,
                    PhysicalPersonMoreQuantityPrice = wasteType.PhysicalPersonMoreQuantityPrice,
                    Coeficient = wasteType.Coeficient,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
            finally
            {
                bussinessLogic = null;
            }
        }

        // POST: Waste/Edit/5
        [HttpPost]
        public JsonResult Edit(int id, string name, decimal lessQuantity, decimal fromQuantity, decimal endQuantity, decimal moreQuantity,
                                    decimal municipalityLessQuantityPrice, decimal municipalityIntervalQuantityPrice, decimal municipalityMoreQuantityPrice,
                                    decimal legalPersonLessQuantityPrice, decimal legalPersonIntervalQuantityPrice, decimal legalPersonMoreQuantityPrice,
                                    decimal physicalPersonLessQuantityPrice, decimal physicalPersonIntervalQuantityPrice, decimal physicalPersonMoreQuantityPrice,
                                    decimal coeficient)
        {
            var bussinessLogic = new WasteTypeBusinessLogic();

            try
            {
                bussinessLogic.Edit(new WasteTypeItem
                {
                    Id = id,
                    LessQuantity = lessQuantity,
                    FromQuantity = fromQuantity,
                    EndQuantity = endQuantity,
                    MoreQuantity = moreQuantity,
                    MunicipalityLessQuantityPrice = municipalityLessQuantityPrice,
                    MunicipalityIntervalQuantityPrice = municipalityIntervalQuantityPrice,
                    MunicipalityMoreQuantityPrice = municipalityMoreQuantityPrice,
                    LegalPersonLessQuantityPrice = legalPersonLessQuantityPrice,
                    LegalPersonIntervalQuantityPrice = legalPersonIntervalQuantityPrice,
                    LegalPersonMoreQuantityPrice = legalPersonMoreQuantityPrice,
                    PhysicalPersonLessQuantityPrice = physicalPersonLessQuantityPrice,
                    PhysicalPersonIntervalQuantityPrice = physicalPersonIntervalQuantityPrice,
                    PhysicalPersonMoreQuantityPrice = physicalPersonMoreQuantityPrice,
                    Coeficient = coeficient

                });

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                bussinessLogic = null;
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        // GET: Waste/Delete/5
        public ActionResult Delete(int id)
        {
            var bussinessLogic = new WasteTypeBusinessLogic();

            try
            {
                var regionItem = bussinessLogic.Get(id);
                var model = new WasteTypeViewModel
                {
                    Id = regionItem.Id,
                    Name = regionItem.Name
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
            finally
            {
                bussinessLogic = null;
            }
        }

        // POST: Waste/Delete/5
        [HttpPost]
        public JsonResult Remove(int Id)
        {
            var bussinessLogic = new WasteTypeBusinessLogic();

            try
            {
                bussinessLogic.Remove(Id);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bussinessLogic = null;
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}