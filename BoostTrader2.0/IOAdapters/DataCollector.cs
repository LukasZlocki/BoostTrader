using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// Dataloader to handle with 
/// </summary>


namespace BoostTrader2._0.IOAdapters
{
    public class DataCollector 
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public double StartPrice { get; set; }
        public double EndPrice { get; set; }
        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
        public int Volume { get; set; }


/*
        public DataCollector (string Name, string Data, double StartPrice, double EndPrice, double MaxPrice, double MinPrice, double Volume)
        {
            this.Name = Name;
            this.Date = Data;
            this.StartPrice = StartPrice;
            this.EndPrice = EndPrice;
            this.MaxPrice = MaxPrice;
            this.MinPrice = MinPrice;
            this.Volume = Volume;
        }

 */ 
        public void LoadPieceOfData(string date, double startPrice, double endPrice, double minPrice, double maxPrice)
        {
            this.Date = date;
            this.StartPrice = startPrice;
            this.EndPrice = endPrice;
            this.MaxPrice = maxPrice;
            this.MinPrice = minPrice;
        }

  

        #region Getters

        public string GetData()
        {
            return this.Date;
        }

        public double GetStartPrice()
        {
            return this.StartPrice;
        }

        public double GetEndPrice()
        {
            return this.EndPrice;
        }

        public double GetMaxPrice()
        {
            return this.MaxPrice;
        }

        public double GetMinPrice()
        {
            return this.MinPrice;
        }

        #endregion
    }
}
