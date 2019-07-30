using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Web.Areas.Admin.ViewModels;
using FSM.Web.Common;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class SettingController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Dependency]
        public IimportantDocuments ImportantDocsRepo { get; set; }
        [HttpGet]
        public ActionResult ManageDocument()
        {
            ManageDocumentVM manageDocumentVM = new ManageDocumentVM();
            try
            {
                // getting importantdoc list
                GetImportantDocList(manageDocumentVM);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed list of all documents.");

                return View(manageDocumentVM);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }

        }
        [ValidateInput(false)]
        public ActionResult SaveImportantDocument(ImportantDocumentVM importantDocumentVM)
        {
            ManageDocumentVM manageDocumentVM = new ManageDocumentVM();
            ImportantDocuments importantDocuments = new ImportantDocuments();
            try
            {
                DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/ImportantDocs/"));
                if (!di.Exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/ImportantDocs/"));
                }
                if (importantDocumentVM.FilePosted == null && string.IsNullOrEmpty(importantDocumentVM.Description))
                {
                    return Json(new { errormsg = "File and description is required !" });
                }
                else if (importantDocumentVM.FilePosted == null)
                {
                    return Json(new { errormsg = "File is required !" });
                }
                else if (string.IsNullOrEmpty(importantDocumentVM.Description))
                {
                    return Json(new { errormsg = "Description is required !" });
                }
                else if (importantDocumentVM.Description.Length > 300)
                {
                    return Json(new { errormsg = "Description max length is 300" });
                }

                // saving doc to server
                Guid attchGuid = Guid.NewGuid();
                string importantDoc = Path.GetFileNameWithoutExtension(importantDocumentVM.FilePosted.FileName);
                string extension = Path.GetExtension(importantDocumentVM.FilePosted.FileName);
                var filepath = Path.Combine(Server.MapPath("~/ImportantDocs/"),
                                  importantDoc + "_" + attchGuid + extension);
                importantDocumentVM.FilePosted.SaveAs(filepath);

                // saving importantdoc entity
                importantDocuments.Id = Guid.NewGuid();
                importantDocuments.Description = importantDocumentVM.Description;
                importantDocuments.FileName = System.IO.Path.GetFileName(importantDocumentVM.FilePosted.FileName);
                importantDocuments.DocType = GetDocumentType(extension);
                importantDocuments.FilePath = Path.Combine("ImportantDocs/", importantDoc + "_"
                                              + attchGuid + extension);
                importantDocuments.IsDelete = false;
                importantDocuments.CreatedDate = DateTime.Now;
                importantDocuments.CreatedBy = Guid.Parse(base.GetUserId);
                ImportantDocsRepo.Add(importantDocuments);
                ImportantDocsRepo.Save();

                // getting importantdoc list
                GetImportantDocList(manageDocumentVM);


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " imported a new document named " + importantDocuments.FileName);

                return PartialView("_ImportantDocumentList", manageDocumentVM);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                ImportantDocsRepo.Dispose();
            }
        }
        public ActionResult DeleteImportantDoc()
        {
            ManageDocumentVM manageDocumentVM = new ManageDocumentVM();
            try
            {
                var ImpDocId = Request.QueryString["Fileid"] != null ? Guid.Parse(Request.QueryString["Fileid"]) : Guid.Empty;
                if (ImpDocId != Guid.Empty)
                {
                    var importantDocument = ImportantDocsRepo.FindBy(m => m.Id == ImpDocId).FirstOrDefault();

                    // deleting from folder
                    System.IO.File.Delete(Server.MapPath("~/" + importantDocument.FilePath));

                    // deleting from database
                    importantDocument.IsDelete = true;
                    ImportantDocsRepo.Edit(importantDocument);
                    ImportantDocsRepo.Save();

                    // getting importantdoc list
                    GetImportantDocList(manageDocumentVM);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted a document named " + importantDocument.FileName);

                    return PartialView("_ImportantDocumentList", manageDocumentVM);
                }
                return Json(new { errormsg = "File doesnot exist ! " });
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                ImportantDocsRepo.Dispose();
            }
        }
        private void GetImportantDocList(ManageDocumentVM manageDocumentVM)
        {
            try
            {
                var importantDocuments = ImportantDocsRepo.GetAll().Where(m => m.IsDelete == false).OrderByDescending(m => m.CreatedDate);
                CommonMapper<ImportantDocuments, ImportantDocumentVM> mapper = new CommonMapper<ImportantDocuments, ImportantDocumentVM>();
                List<ImportantDocumentVM> importantDocumentList = mapper.MapToList(importantDocuments.ToList());

                manageDocumentVM.ImportantDocumentList = importantDocumentList;
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                ImportantDocsRepo.Dispose();
            }
        }
        public ActionResult DownloadImportantDoc()
        {
            try
            {
                string Filepath = Request.QueryString["Filepath"];
                if (!string.IsNullOrEmpty(Filepath))
                {
                    return Json(Filepath, JsonRequestBehavior.AllowGet);
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download a document");


                return Json(new { errormsg = "File doesnot exist !" });
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }
        private static string GetDocumentType(string extension)
        {
            string documentType;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".png":
                    documentType = "Image";
                    break;
                case ".doc":
                case ".docx":
                    documentType = "Word Document";
                    break;
                case ".xls":
                case ".xlsx":
                    documentType = "Word Excel";
                    break;
                case ".pdf":
                    documentType = "Pdf";
                    break;
                case ".text":
                case ".txt":
                    documentType = "Text File";
                    break;
                default:
                    documentType = "Unknown";
                    break;
            }

            return documentType;
        }
        public ActionResult ManageQuickView()
        {
            var ManageDocList = ImportantDocsRepo.GetAll().Where(m => m.IsDelete == false).ToList();

            CommonMapper<ImportantDocuments, ImportantDocumentVM> mapper = new CommonMapper<ImportantDocuments, ImportantDocumentVM>();
            var managedocs = mapper.MapToList(ManageDocList);


            ManageDocumentVM manageDocViewModel = new ManageDocumentVM();
            manageDocViewModel.ImportantDocumentList = managedocs;

            return PartialView("_ManageQuickView", manageDocViewModel);
        }


        public ActionResult UploadFiles(string Description, IEnumerable<HttpPostedFileBase> files)
        {
            ManageDocumentVM manageDocumentVM = new ManageDocumentVM();
            ImportantDocuments importantDocuments = new ImportantDocuments();
            try
            {
                DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/ImportantDocs/"));
                if (!di.Exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/ImportantDocs/"));
                }
                //if (importantDocumentVM.FilePosted == null && string.IsNullOrEmpty(importantDocumentVM.Description))
                //{
                //    return Json(new { errormsg = "File and description is required !" });
                //}
                //else if (importantDocumentVM.FilePosted == null)
                //{
                //    return Json(new { errormsg = "File is required !" });
                //}
                //else if (string.IsNullOrEmpty(importantDocumentVM.Description))
                //{
                //    return Json(new { errormsg = "Description is required !" });
                //}
                //else if (importantDocumentVM.Description.Length > 300)
                //{
                //    return Json(new { errormsg = "Description max length is 300" });
                //}

                // saving doc to server
                for (int i = 0; i < files.Count(); i++)
                {
                    var File = Request.Files[i];
                    Guid attchGuid = Guid.NewGuid();
                    string importantDoc = Path.GetFileNameWithoutExtension(File.FileName);
                    string extension = Path.GetExtension(File.FileName);
                    var filepath = Path.Combine(Server.MapPath("~/ImportantDocs/"),
                                      importantDoc + "_" + attchGuid + extension);
                    File.SaveAs(filepath);

                    // saving importantdoc entity
                    importantDocuments.Id = Guid.NewGuid();
                    importantDocuments.Description = Description;
                    importantDocuments.FileName = System.IO.Path.GetFileName(File.FileName);
                    importantDocuments.DocType = GetDocumentType(extension);
                    importantDocuments.FilePath = Path.Combine("ImportantDocs/", importantDoc + "_"
                                                  + attchGuid + extension);
                    importantDocuments.IsDelete = false;
                    importantDocuments.CreatedDate = DateTime.Now;
                    importantDocuments.CreatedBy = Guid.Parse(base.GetUserId);
                    ImportantDocsRepo.Add(importantDocuments);
                    ImportantDocsRepo.Save();
                    // getting importantdoc list
                    GetImportantDocList(manageDocumentVM);
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " imported a new document named " + importantDocuments.FileName);

                return PartialView("_ImportantDocumentList", manageDocumentVM);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                ImportantDocsRepo.Dispose();
            }
        }
    }
}