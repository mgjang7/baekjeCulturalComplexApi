namespace BaekjeCulturalComplexApi.Models.ViewModels
{
    public class IncidentalSalesDetailViewModel
    {
        public string DivisionName { get; set; }
        public string DivisionCode { get; set; }
        public string ItemName { get; set; }
        public int SupplyPrice { get; set; }
        public int Surtax { get; set; }
        public int SumPrice { get; set; }
        public string Note { get; set; }
    }
}
