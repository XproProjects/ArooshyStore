using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;
using AutoMapper;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class DocumentsController : BaseController
    {
        private readonly IDocumentRepository _repository;
        private readonly IMapper _mapper;
        public DocumentsController(IDocumentRepository repository)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
        }
        [HttpGet]
        public ActionResult AttachDocument(string type, string id)
        {
            if (User != null)
            {
                DocumentViewModel document = new DocumentViewModel();
                document.TypeId = id;
                document.DocumentType = type;
                return PartialView(document);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        public ActionResult DocumentsList(string type, string typeId)
        {
            List<DocumentDomainModel> documents = _repository.DocumentsList(type, typeId);
            var data = documents.ToList();
            return new JsonResult { Data = new { data = data } };
        }
        public ActionResult AttachDocumentsForPost(DocumentViewModel doc)
        {
            DocumentDomainModel documentResponse = new DocumentDomainModel();
            if (User != null)
            {
                documentResponse = _repository.AttachDocumentsForPost(doc, User.UserId);
            }
            else
            {
                documentResponse.Status = false;
                documentResponse.DocumentId = 0;
            }
            return new JsonResult { Data = new { status = documentResponse.Status, documentId = documentResponse.DocumentId, documentType = documentResponse.DocumentType, typeId = doc.TypeId, extension = doc.DocumentExtension } };
        }
        public JsonResult UploadDocument(string id)
        {
            bool status = false;
            try
            {
                int documentId = Convert.ToInt32(id.Split('_')[0]);
                string documentType = id.Split('_')[1].Replace("|","_");
                string typeId = id.Split('_')[2];
                string extension = id.Split('_')[3];

                if (extension != null && extension != "")
                {
                    HttpPostedFileBase document = Request.Files[0] as HttpPostedFileBase;
                    status = _repository.UploadDocument(documentId,documentType,typeId,extension, document,User.UserId);
                }
                else
                {
                    status = true;
                }
            }
            catch
            {
                status = false;
            }

            return Json(new
            {
                status = status,
            });
        }
        public ActionResult DeleteDocument(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.DeleteDocument(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
    }
}