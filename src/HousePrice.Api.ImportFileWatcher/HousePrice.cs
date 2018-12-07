using System;

namespace HousePrice.Api.ImportFileWatcher
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class HousePrice
	{
		public string TransactionId { get; set; }
		public double Price { get; set; }
		public DateTime TransferDate { get; set; }
		public string Postcode { get; set; }
		public string PropertyType { get; set; }
		public string IsNew { get; set; }
		public string Duration { get; set; }
		public string PAON { get; set; }
		public string SAON { get; set; }
		public string Street { get; set; }
		public string Locality { get; set; }
		public string City { get; set; }
		public string District { get; set; }
		public string County { get; set; }
		public string CategoryType { get; set; }
		public string Status { get; set; }
	}
}