
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace models.BaseClass
{

    public class Result
    {
        public int resultCode { get; set; }
        public string resultDescription { get; set; }

        public Body body { get; set; }


    }
    public class IntraDayTradeHistory
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Conract { get; set; }

        public double Price { get; set; }

        public double Quantity { get; set; }
    }
    public class Body
    {
        public List<IntraDayTradeHistory> intraDayTradeHistoryList { get; set; }
    }
    public class ResultTableObject
    {
        public List<TableObject> Table { get; set; }
    }
    public class TableObject
    {
        public DateTime DateTime { get; set; }
        public double TotalProcessCount { get; set; }
        public double TotalProcessPrice { get; set; }
        public double AvaragePrice { get; set; }

    }
}
