namespace ProductMicroService.Data.Services
{
    public interface IInventoryService
    {
        Task IncreaseProductQuantityAsync(int productId, int quantity);

        Task DecreaseProductQuantityAsync(int productId, int quantity);

        Task UpdateProductQuantityAsync(int productId, int quantity);

        Task<int> GetProductQuantityAsync(int productId);
    }
}
