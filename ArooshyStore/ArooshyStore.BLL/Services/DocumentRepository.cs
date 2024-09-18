using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IUnitOfWork _unitOfWork;
       // private string documentSiteName = ConfigurationManager.AppSettings["DocumentSiteName"].ToString();
        public DocumentRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<DocumentDomainModel> DocumentsList(string type, string typeId)
        {
            //Bring all documents of user and employee except ProfilePicture and 
            //bring all documents of company and branch except Logo
            List<DocumentDomainModel> documents = new List<DocumentDomainModel>();
            documents = (from d in _unitOfWork.Db.Set<tblDocument>()
                         where d.TypeId == typeId && d.DocumentType == type
                         && d.Remarks != "ProfilePicture" && d.Remarks != "Logo"
                         select new DocumentDomainModel
                         {
                             DocumentId = d.DocumentId,
                             Date = d.Date,
                             DocumentExtension = d.DocumentExtension,
                             DocumentType = d.DocumentType,
                             TypeId = typeId,
                             Remarks = d.Remarks ?? "",
                             ImagePath = "/Areas/Admin/FormsDocuments/" + d.DocumentType + "/" + d.DocumentId.ToString() + "." + d.DocumentExtension
                         }).ToList();
            return documents;
        }
        public DocumentDomainModel AttachDocumentsForPost(DocumentViewModel doc, int loggedInUserId)
        {
            DocumentDomainModel response = new DocumentDomainModel();
            try
            {
                //doc.Date = DateTime.Now;
                if (string.IsNullOrEmpty(doc.Remarks))
                {
                    doc.Remarks = "";
                }
                if (doc.Date == null)
                {
                    doc.Date = DateTime.Now;
                }
                doc.DocumentType = doc.DocumentType.Replace("|", "_");
                //If he selected any document or picture then this will trigger
                if (!(string.IsNullOrEmpty(doc.DocumentExtension)))
                {
                    //If any form has only profile pic or logo then use first condition to match only document type i.e doc.DocumentType != "Company" etc
                    //else if any form has logo or profile as well as documents also then then condition to match documenttype as well as remarks e.g (doc.DocumentType == "Company" && doc.Remarks == "Logo") etc
                    //else if any form only documents then use first condition to match only document type i.e doc.DocumentType != "Company" etc
                    if ((doc.DocumentType != "Invoice")
                         &&((doc.DocumentType == "User" && doc.Remarks == "ProfilePicture")                    
                        || (doc.DocumentType == "Product" && doc.Remarks == "ProfilePicture")
                        || (doc.DocumentType == "Category" && doc.Remarks == "ProfilePicture")
                        || (doc.DocumentType == "Supplier" && doc.Remarks == "ProfilePicture")
                        || (doc.DocumentType == "Customer" && doc.Remarks == "ProfilePicture")
                        || (doc.DocumentType == "DiscountOffer" && doc.Remarks == "ProfilePicture")))


                    {
                        //Delete previous profile picture or logo if he has any before
                        var itemDocument = _unitOfWork.Db.Set<tblDocument>().Where(x => x.TypeId == doc.TypeId && x.DocumentType == doc.DocumentType && x.Remarks == doc.Remarks).FirstOrDefault();
                        if (itemDocument != null)
                        {
                            try
                            {
                                int documentsId = itemDocument.DocumentId;
                                string documentExtension = itemDocument.DocumentExtension;
                                _unitOfWork.Db.Set<tblDocument>().Remove(itemDocument);
                                _unitOfWork.Db.SaveChanges();
                                string fileName = documentsId + "." + documentExtension;
                                string path1 = HttpContext.Current.Server.MapPath("~/Areas/Admin/FormsDocuments/" + doc.DocumentType + "/" + fileName);
                                FileInfo file = new FileInfo(path1);
                                if (file.Exists)//check file exsit or not
                                {
                                    file.Delete();
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrorHandler error = ErrorHandler.GetInstance;
                                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                                "DocumentRepository", "AttachDocumentsForPost File Delete");
                            }
                        }

                    }
                    //If he is updating document in Attach document form then do this
                    if (doc.DocumentId > 0)
                    {
                        try
                        {
                            var dcc = _unitOfWork.Db.Set<tblDocument>().Where(x => x.DocumentId == doc.DocumentId).FirstOrDefault();
                            dcc.DocumentExtension = doc.DocumentExtension;
                            dcc.Remarks = doc.Remarks;
                            _unitOfWork.Db.SaveChanges();
                            response.DocumentId = doc.DocumentId;
                        }
                        catch
                        {
                            response.DocumentId = 0;
                        }
                    }
                    else
                    {
                        tblDocument document = new tblDocument();
                        document.Date = doc.Date;
                        document.TypeId = doc.TypeId;
                        document.DocumentExtension = doc.DocumentExtension;
                        document.DocumentType = doc.DocumentType;
                        document.Remarks = doc.Remarks;
                        document.UserId = loggedInUserId;
                        _unitOfWork.Db.Set<tblDocument>().Add(document);
                        _unitOfWork.Db.SaveChanges();
                        response.DocumentId = document.DocumentId;
                    }
                }
                else
                {
                    //This will trigger when he updated any document detail in AttachDocument but did not updated document in it.
                    var docc = _unitOfWork.Db.Set<tblDocument>().Where(x => x.DocumentId == doc.DocumentId).FirstOrDefault();
                    docc.Remarks = doc.Remarks;
                    docc.Date = doc.Date;
                    _unitOfWork.Db.SaveChanges();
                }
                //Create Main Documents Folder
                var path = HttpContext.Current.Server.MapPath("~/Areas/Admin/FormsDocuments");
                bool exists = Directory.Exists(path);//check folder exsit or not
                if (!exists)
                {
                    Directory.CreateDirectory(path);
                }
                //Create Main Type Documents Folder
                var path2 = HttpContext.Current.Server.MapPath("~/Areas/Admin/FormsDocuments/" + doc.DocumentType);
                bool exists2 = Directory.Exists(path2);//check folder exsit or not
                if (!exists2)
                {
                    Directory.CreateDirectory(path2);
                }
                response.Status = true;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
                response.DocumentId = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                "DocumentRepository", "AttachDocumentsForPost");
            }
            response.DocumentType = doc.DocumentType.Replace("_", "|"); ;
            response.TypeId = doc.TypeId;
            response.DocumentExtension = doc.DocumentExtension;
            return response;
        }
        public bool UploadDocument(int documentId, string documentType, string typeId, string extension, HttpPostedFileBase file, int loggedInUser)
        {
            bool status = false;
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    //Delete Previous File for edit document in AttachDocument form
                    string file_name = documentId + "." + extension;
                    string path = HttpContext.Current.Server.MapPath("~/Areas/Admin/FormsDocuments/" + documentType + "/" + file_name);
                    FileInfo file2 = new FileInfo(path);
                    if (file2.Exists)//check file exsit or not
                    {
                        try
                        {
                            file2.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler error = ErrorHandler.GetInstance;
                            error.InsertError(loggedInUser, ex.Message.ToString(), "Web Application",
                                            "DocumentRepository", "UploadDocument Previous File Delete");
                        }
                    }

                    try
                    {
                        //try
                        //{
                        //    byte[] fileData = null;
                        //    using (var binaryReader = new BinaryReader(document.InputStream))
                        //    {
                        //        fileData = binaryReader.ReadBytes(document.ContentLength);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                        // string fileName = System.IO.Path.GetFileName(doc.Document.FileName);
                        string fileName = documentId + "." + extension;
                        //Set the Image File Path.
                        string filePath = "~/Areas/Admin/FormsDocuments/" + documentType + "/" + fileName;
                        //Save the Image File in Folder.
                        file.SaveAs(HttpContext.Current.Server.MapPath(filePath));
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        status = false;
                        ErrorHandler error = ErrorHandler.GetInstance;
                        error.InsertError(loggedInUser, ex.Message.ToString(), "Web Application",
                                        "DocumentRepository", "UploadDocument Save File");
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUser, ex.Message.ToString(), "Web Application",
                                "DocumentRepository", "UploadDocument");
            }

            return status;
        }
        public StatusMessageViewModel DeleteDocument(int id, int loggedInUser)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                var document = _unitOfWork.Db.Set<tblDocument>().Where(x => x.DocumentId == id).FirstOrDefault();
                if (document != null)
                {
                    var documentType = document.DocumentType;
                    string extension = document.DocumentExtension;
                    string file_name = id + "." + extension;
                    string path = HttpContext.Current.Server.MapPath("~/Areas/Admin/FormsDocuments/" + documentType + "/" + file_name);
                    FileInfo file = new FileInfo(path);
                    if (file.Exists)//check file exsit or not
                    {
                        try
                        {
                            file.Delete();
                            response.Status = true;
                            response.Message = "Success";
                        }
                        catch (Exception ex)
                        {
                            response.Status = false;
                            response.Message = ex.Message.ToString();
                            ErrorHandler error = ErrorHandler.GetInstance;
                            error.InsertError(loggedInUser, ex.Message.ToString(), "Web Application",
                                            "DocumentRepository", "DeleteDocument File Delete");
                        }
                    }
                    if (response.Status == true)
                    {
                        _unitOfWork.Db.Set<tblDocument>().Remove(document);
                        _unitOfWork.Db.SaveChanges();
                    }
                }
                else
                {
                    response.Status = true;
                    response.Message = "Successs";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUser, ex.Message.ToString(), "Web Application",
                                "DocumentRepository", "DeleteDocument");
            }
            return response;
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        //public void Dispose()
        //{
        //}
    }
}
