using System;
using System.Collections.Generic;

public enum NotificationReason
{
    RECEIPT_VALIDATED,
    RECEIPT_REFRESHED,
    WEBHOOK_REPEATED,
    ACKNOWLEDGED,
    PURCHASED,
    REVOKED,
    EXPIRED,
    RENEWED,
    PRICE_CHANGE_CONFIRMED,
    WILL_LAPSE,
    WILL_AUTO_RENEW,
    EXTENDED,
    REFUNDED,
    PAUSED,
    ENTERED_GRACE_PERIOD,
    TEST,
    OTHER
}

public class PurchasesUpdatedWebhook
{
    public string password { get; set; }
    public string type { get; set; }
    public PurchaseNotification notification { get; set; }
    public string applicationUsername { get; set; }
    public Dictionary<string, Purchase> purchases { get; set; }
}

public class PurchaseNotification
{
    public string reason { get; set; }
    public string id { get; set; }
    public string date { get; set; }
    public string productId { get; set; }
    public string purchaseId { get; set; }
    public string transactionId { get; set; }
}

public class Purchase
{
    // Define properties for Purchase object
    // For example:
    // public string productName { get; set; }
    // public decimal price { get; set; }
    // ...

    // You can add more properties based on the actual data structure
}