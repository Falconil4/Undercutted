using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Undercutted.Core
{
    public static class ApiConsumer
    {
        const string BASE_ADDRESS = "https://universalis.app/api/v2";

        public static async Task<MarketBoardData?> GetData(string world, List<string> itemNames)
        {
            if (Undercutted.GameItemsList == null) return null;

            HttpClient client = new();
            client.BaseAddress = new Uri(BASE_ADDRESS);

            List<uint> itemIds = itemNames.Select(name =>Undercutted.GameItemsList.First(item => item.Name == name).RowId).ToList();
            string ids = String.Join(',', itemIds);
            HttpResponseMessage response = await client.GetAsync($"{world}/{ids}?noGst=1");
            string responseData = await response.Content.ReadAsStringAsync();
            
            MarketBoardData? marketBoardData = JsonConvert.DeserializeObject<MarketBoardData>(responseData);
            if (marketBoardData?.Items == null)
            {
                ItemCurrentData? singleItemData = JsonConvert.DeserializeObject<ItemCurrentData>(responseData);
                if (singleItemData == null) return null;

                marketBoardData = new MarketBoardData(new() { singleItemData });
            }
            
            marketBoardData.Items.ForEach(itemData =>
            {
                itemData.ItemName = Undercutted.GameItemsList.First(item => item.RowId == itemData.ItemID).Name;
            });

            return marketBoardData;
        }
    }
}
