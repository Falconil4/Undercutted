using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undercutted.Core
{
    public class MarketBoardData
    {
        public MarketBoardData(List<ItemCurrentData> items)
        {
            Items = items;
        }

        public List<ItemCurrentData> Items { get; init; }
    }

    public class ItemCurrentData
    {
        public ItemCurrentData(uint itemID, long lastUploadTime, List<Listing> listings)
        {
            ItemID = itemID;
            LastUploadTime = lastUploadTime;
            Listings = listings;
        }

        public uint ItemID { get; init; }
        public long LastUploadTime { get; init; }
        public DateTime LastUpload => DateTimeOffset.FromUnixTimeMilliseconds(LastUploadTime).LocalDateTime;
        public List<Listing> Listings { get; init; }
        public string? ItemName { get; set; }
    }

    public class Listing
    {
        public Listing(uint pricePerUnit, uint quantity, uint total, string retainerName, bool hq)
        {
            PricePerUnit = pricePerUnit;
            Quantity = quantity;
            Total = total;
            RetainerName = retainerName;
            HQ = hq;
        }

        public uint PricePerUnit { get; init; }
        public uint Quantity { get; init; }
        public uint Total { get; init; }
        public string RetainerName { get; init; }
        public bool HQ { get; init; }
    }
}
