using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;
using Microsoft.SqlServer.Server;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductRepository : IDisposable
    {
        List<ProductViewModel> GetProductsListAndCount(string whereCondition, string start, string length, string sorting);
        ProductViewModel GetProductById(int id);
        StatusMessageViewModel InsertUpdateProduct(ProductViewModel model,string AttributeDetailData, string Tagsdata, int loggedInUserId);
        StatusMessageViewModel DeleteProduct(int id, int loggedInUserId);
        List<ProductViewModel> GetFeaturedProducts();
        List<ProductViewModel> GetNewArrivalProducts();
        ProductViewModel GetProductSalePrice(int productId);
        ProductViewModel GetProductByBarcode(string barcode);
        ProductViewModel GetProductWithAttributes(int productId);
        List<ProductViewModel> GetSimilrProducts(int productId);
        List<ProductViewModel> GetProductsForShop();
        List<ProductFilterViewModel> GetFiltersList();
        List<ProductViewModel> GetFilteredProducts(string searchString);



    }
}
