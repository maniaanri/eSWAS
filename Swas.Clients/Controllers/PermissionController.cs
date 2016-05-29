﻿namespace Swas.Clients.Controllers
{
    using Common;
    using Swas.Business.Logic.Classes;
    using Swas.Business.Logic.Entity;
    using Swas.Clients.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    [ClientErrorHandler]
    public class PermissionController : Controller
    {
        // GET: Region
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Load()
        {
            var result = new List<PermissionItem>();
            var bussinessLogic = new PermissionBusinessLogic();

            try
            {
                result = bussinessLogic.Load();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bussinessLogic = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var NewItem = new PermissionViewModel
            {
                Description = string.Empty,
                Name = string.Empty
            };

            return View(NewItem);
        }

        [HttpPost]
        public JsonResult Create(string descirption, string name)
        {
            var bussinessLogic = new PermissionBusinessLogic();

            try
            {
                bussinessLogic.Create(new PermissionItem
                {
                    Name = name,
                    Description = descirption
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

        public ActionResult Edit(int Id)
        {
            var bussinessLogic = new PermissionBusinessLogic();

            try
            {
                var permissionItem = bussinessLogic.Get(Id);

                var model = new PermissionViewModel
                {
                    Id = permissionItem.Id,
                    Name = permissionItem.Name,
                    Description = permissionItem.Description
                };

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bussinessLogic = null;
            }
        }

        [HttpPost]
        public JsonResult Edit(int id, string name, string description)
        {
            var bussinessLogic = new PermissionBusinessLogic();

            try
            {
                bussinessLogic.Edit(new PermissionItem { Id = id, Name = name, Description = description });

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

        public ActionResult Delete(int id)
        {
            var bussinessLogic = new PermissionBusinessLogic();

            try
            {
                var regionItem = bussinessLogic.Get(id);
                var model = new PermissionViewModel
                {
                    Id = regionItem.Id,
                    Description = regionItem.Description
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

        [HttpPost]
        public JsonResult Remove(int Id)
        {
            var bussinessLogic = new PermissionBusinessLogic();

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