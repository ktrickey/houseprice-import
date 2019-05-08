using CsvHelper.Configuration;

namespace HousePrice.Api.ImportFileWatcher
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class HousePriceMap : ClassMap<ImportFileWatcher.HousePrice>
    {
        public HousePriceMap()
        {
            Map( m => m.TransactionId ).Index(0);
            Map( m => m.Price ).Index(1);
            Map( m => m.TransferDate ).Index(2).TypeConverterOption.Format("yyyy-MM-dd hh:mm");
            Map( m => m.Postcode ).Index(3);
            Map( m => m.PropertyType ).Index(4);
            Map( m => m.IsNew ).Index(5);
            Map( m => m.Duration ).Index(6);
            Map( m => m.PAON ).Index(7);
            Map( m => m.SAON ).Index(8);
            Map( m => m.Street ).Index(9);
            Map( m => m.Locality ).Index(10);
            Map( m => m.City ).Index(11);
            Map( m => m.District ).Index(12);
            Map( m => m.County ).Index(13);
            Map( m => m.CategoryType ).Index(14);
            Map( m => m.Status ).Index(15);
        }
    }
}