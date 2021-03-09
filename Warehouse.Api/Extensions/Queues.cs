namespace Warehouse.Api.Extensions
{
    public static class Queues
    {
        public const string CreateManufacturerQueue = "CreateManufacturerQueue";
        public const string UpdateManufacturerQueue = "UpdateManufacturerQueue";
        public const string DeleteManufacturerQueue = "DeleteManufacturerQueue";
        
        public const string CreateCustomerQueue = "CreateCustomerQueue";
        public const string DeleteCustomerQueue = "DeleteCustomerQueue";
        
        public const string CreateUserQueue = "CreateUserQueue";
        public const string UpdateUserQueue = "UpdateUserQueue";
    }
}