using System;
using System.Collections.Generic;
using System.Web;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IDocumentRepository : IDisposable
    {
        List<DocumentDomainModel> DocumentsList(string type, string typeId);
        DocumentDomainModel AttachDocumentsForPost(DocumentViewModel doc, int loggedInUserId);
        bool UploadDocument(int documentId,string documentType, string typeId,string extension, HttpPostedFileBase file,int loggedInUser);
        StatusMessageViewModel DeleteDocument(int id,int loggedInUser);
    }
}
