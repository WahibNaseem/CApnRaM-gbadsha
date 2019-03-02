using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MxMarchantQuickType
{


    public partial class Mxmerchant
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("authCode")]
        public string AuthCode { get; set; }

        [JsonProperty("authMessage")]
        public string AuthMessage { get; set; }

        [JsonProperty("authOnly")]
        public bool AuthOnly { get; set; }

        [JsonProperty("availableAuthAmount")]
        public string AvailableAuthAmount { get; set; }

        [JsonProperty("batch")]
        public string Batch { get; set; }

        [JsonProperty("batchId")]
        public long BatchId { get; set; }

        [JsonProperty("cardAccount")]
        public CardAccount CardAccount { get; set; }

        [JsonProperty("cardPresent")]
        public bool CardPresent { get; set; }

        [JsonProperty("clientReference")]
        public string ClientReference { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("creatorName")]
        public string CreatorName { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("customerCode")]
        public string CustomerCode { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("invoice")]
        public string Invoice { get; set; }

        [JsonProperty("merchantId")]
        public long MerchantId { get; set; }

        [JsonProperty("meta")]
        public string Meta { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }

        [JsonProperty("posData")]
        public PosData PosData { get; set; }

        [JsonProperty("purchases")]
        public Purchase[] Purchases { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("requireSignature")]
        public bool RequireSignature { get; set; }

        [JsonProperty("reviewIndicator")]
        public long ReviewIndicator { get; set; }

        [JsonProperty("risk")]
        public Risk Risk { get; set; }

        [JsonProperty("settledAmount")]
        public string SettledAmount { get; set; }

        [JsonProperty("settledCurrency")]
        public string SettledCurrency { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }

        [JsonProperty("tenderType")]
        public string TenderType { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Risk
    {
        [JsonProperty("avsAddressMatch")]
        public bool AvsAddressMatch { get; set; }

        [JsonProperty("avsResponseCode")]
        public string AvsResponseCode { get; set; }

        [JsonProperty("avsZipMatch")]
        public bool AvsZipMatch { get; set; }

        [JsonProperty("cvvMatch")]
        public bool CvvMatch { get; set; }

        [JsonProperty("cvvResponse")]
        public string CvvResponse { get; set; }

        [JsonProperty("cvvResponseCode")]
        public string CvvResponseCode { get; set; }
    }

    public partial class Purchase
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("discountAmount")]
        public string DiscountAmount { get; set; }

        [JsonProperty("discountRate")]
        public string DiscountRate { get; set; }

        [JsonProperty("extendedAmount")]
        public string ExtendedAmount { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("taxAmount")]
        public string TaxAmount { get; set; }

        [JsonProperty("taxRate")]
        public string TaxRate { get; set; }

        [JsonProperty("unitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [JsonProperty("unitPrice")]
        public string UnitPrice { get; set; }
    }

    public partial class PosData
    {
    }

    public partial class CardAccount
    {
        [JsonProperty("cardId")]
        public string CardId { get; set; }

        [JsonProperty("cardPresent")]
        public bool CardPresent { get; set; }

        [JsonProperty("cardType")]
        public string CardType { get; set; }

        [JsonProperty("entryMode")]
        public string EntryMode { get; set; }

        [JsonProperty("expiryMonth")]
        public string ExpiryMonth { get; set; }

        [JsonProperty("expiryYear")]
        public string ExpiryYear { get; set; }

        [JsonProperty("hasContract")]
        public bool HasContract { get; set; }

        [JsonProperty("last4")]
        public string Last4 { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public partial class Mxmerchant
    {
        public static Mxmerchant FromJson(string json) => JsonConvert.DeserializeObject<Mxmerchant>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Mxmerchant self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}