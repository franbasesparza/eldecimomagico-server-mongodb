namespace eldecimomagico.dal
{
    public class GooglePurchaseResponse
    {
        public string kind { get; set; }
        public string purchaseTimeMillis { get; set; }
        public int purchaseState { get; set; }
        public int consumptionState { get; set; }
        public string developerPayload { get; set; }
        public string orderId { get; set; }
        public int purchaseType { get; set; }
        public int acknowledgementState { get; set; }
        public string purchaseToken { get; set; }
        public string productId { get; set; }
        public int quantity { get; set; }
        public string obfuscatedExternalAccountId { get; set; }
        public string obfuscatedExternalProfileId { get; set; }
        public string regionCode { get; set; }
    }
}
