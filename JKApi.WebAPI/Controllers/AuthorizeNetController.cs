using AuthorizeNet.Api.Contracts.V1;
using JKApi.Data.DAL;
using JKApi.Service.Service.Administration.General;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Services;
using JKApi.WebAPI.Models;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [System.Web.Http.RoutePrefix("v{version:apiVersion}/FMS")]
    public class AuthorizeNetController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ChargeCreditCard")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Authorize PayWithCC(string CardNumber, string CardHolderName, string CardExpiry, string CardCVC, decimal Amount, string ClassID, bool IsProfile)
        {
            GeneralService generalService = new GeneralService();
            Authorize hd = new Authorize();
            Guid PaymentPRofileID = Guid.NewGuid();
            PaymentGatewayDetail PGD = generalService.GetPaymentGatewayList().Where(r => r.IsActive == true).FirstOrDefault();
            var data = generalService.AddressList(ClassID);
            PaymentProfileDetail PaymentProfileDetail = new PaymentProfileDetail();
            PaymentProfileDetail.CreatedDate = DateTime.Now;
            PaymentProfileDetail.Id = PaymentPRofileID;
            PaymentProfileDetail.FKClassId = Convert.ToInt32(ClassID);
            PaymentProfileDetail.FKPaymentGatewayID = PGD.Id;
            PaymentProfileDetail.FKTypeListId = 1;
            generalService.InsertPaymentProfileDetail(PaymentProfileDetail);
            var objCreditCardType = new creditCardType
            {
                cardNumber = CardNumber,
                expirationDate = CardExpiry,
                cardCode = CardCVC
            };
            var objcustomerAddressType = new customerAddressType
            {
                //firstName = "Sudher",
                //lastName = "Yadav",
                address = data.Address1,
                zip = data.PostalCode,
            };
            Guid OrderTransactionID = OrderTransaction(PaymentProfileDetail, Amount);
            dynamic response = CreditCardPayment(objCreditCardType, objcustomerAddressType, PGD.LoginID, PGD.TransactionKey, Amount, "TestMode", "1");

            bool isCreditCardPaymentSuccess = AuthorizenetCommon.CheckAuthorizeNetApiResponse(response);

            if (isCreditCardPaymentSuccess)
            {
                hd.StatusCode = 200;
                hd.SuccessMessage = "Payment Approved.";
                if (IsProfile)
                {
                    PaymentProfileDetail.AccountNumber = response.transactionResponse.accountNumber;
                    PaymentProfileDetail.AccountType = response.transactionResponse.accountType;
                    hd.CardNumber = PaymentProfileDetail.AccountNumber;
                    hd.CardType = PaymentProfileDetail.AccountType;
                    dynamic CustomerProfileResponse = CreateCustomerProfileFromTransaction(PGD, response);
                    bool isCustomerProfileCreatedSuccessfully = AuthorizenetCommon.CheckAuthorizeNetApiResponse(CustomerProfileResponse);
                    if (isCustomerProfileCreatedSuccessfully)
                    {
                        PaymentProfileDetail.CustomerProfileID = Convert.ToInt64(CustomerProfileResponse.customerProfileId);
                        PaymentProfileDetail.CustomerPaymentProfileID = Convert.ToInt64(CustomerProfileResponse.customerPaymentProfileIdList[0]);
                        generalService.UpdatePaymentProfile(PaymentProfileDetail);
                    }
                }
                CreateOrderTransactionsResponse(response, OrderTransactionID, PaymentPRofileID);


            }
            else
            {
                hd.StatusCode = 201;
                hd.ErrorMessage = "Payment Failed.";
            }

            return hd;
        }


        private dynamic CreditCardPayment(creditCardType objCreditCardType, customerAddressType objcustomerAddressType, string ApiLoginID, string ApiTransactionKey, decimal TotalOrderPrice, string PaymentGatewayMode, string OrderId)
        {

            #region Order Items
            var i = 1;

            int j = 0;

            var lineItems = new lineItemType[i];
            lineItems[j] = new lineItemType
            {
                itemId = Convert.ToString(1),
                name = Convert.ToString("Invoices Payment"),
                quantity = 1,
                unitPrice = Convert.ToDecimal(TotalOrderPrice)
            };
            #endregion

            dynamic response = AuthorizePaymentGateway.ChargeCreditCard(ApiLoginID, ApiTransactionKey, Convert.ToDecimal(TotalOrderPrice), objCreditCardType, objcustomerAddressType, lineItems, PaymentGatewayMode, Convert.ToString(OrderId));

            return response;
        }

        private dynamic CreateCustomerProfileFromTransaction(PaymentGatewayDetail PGD, dynamic CCresponse)
        {
            dynamic response = AuthorizePaymentGateway.CreateCustomerProfileFromTransaction(PGD.LoginID, PGD.TransactionKey, CCresponse.transactionResponse.transId, "TestMode");
            return response;
        }

        public Guid OrderTransaction(PaymentProfileDetail PPD, decimal Amount)
        {
            Guid OrderID = Guid.NewGuid();

            OrderTransaction order = new JKApi.Data.DAL.OrderTransaction();

            order.Id = OrderID;
            order.FKPaymentProfileID = PPD.Id;
            order.Price = Amount;
            order.PriceWithTax = Amount;
            order.Tax = 0;
            order.CreatedDate = DateTime.Now;
            GeneralService generalService = new GeneralService();
            generalService.OrderTransaction(order);

            return OrderID;
        }

        private void CreateOrderTransactionsResponse(dynamic response, Guid orderId, Guid PaymentProfileID)
        {

            OrderTransactionsResponse objOrderTransactionsResponse = new OrderTransactionsResponse();
            GeneralService generalService = new GeneralService();
            objOrderTransactionsResponse.FKOrderID = orderId;
            objOrderTransactionsResponse.CreatedDate = DateTime.Now;
            objOrderTransactionsResponse.FKPaymentProfileID = PaymentProfileID;
            objOrderTransactionsResponse.OrderTxReponseID = Guid.NewGuid();
            objOrderTransactionsResponse.MessagesResultCode = Convert.ToString(response.messages.resultCode);
            objOrderTransactionsResponse.MessagesCode = Convert.ToString(response.messages.message[0].code);
            objOrderTransactionsResponse.MessagesText = Convert.ToString(response.messages.message[0].text);

            if (AuthorizenetCommon.IstransactionResponsePropertyExist(response))
            {
                objOrderTransactionsResponse.ResponseCode = response.transactionResponse.responseCode;
                objOrderTransactionsResponse.AuthCode = Convert.ToString(response.transactionResponse.authCode);
                objOrderTransactionsResponse.AvsResultCode = Convert.ToString(response.transactionResponse.avsResultCode);

                objOrderTransactionsResponse.CvvResultCode = Convert.ToString(response.transactionResponse.cvvResultCode);
                objOrderTransactionsResponse.CavvResultCode = Convert.ToString(response.transactionResponse.cavvResultCode);

                objOrderTransactionsResponse.TransId = Convert.ToString(response.transactionResponse.transId);
                objOrderTransactionsResponse.RefTransId = Convert.ToString(response.transactionResponse.refTransID);
                objOrderTransactionsResponse.TransHash = Convert.ToString(response.transactionResponse.transHash);

                objOrderTransactionsResponse.AccountNumber = Convert.ToString(response.transactionResponse.accountNumber);
                objOrderTransactionsResponse.EntryMode = null;
                objOrderTransactionsResponse.AccountType = Convert.ToString(response.transactionResponse.accountType);


                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    objOrderTransactionsResponse.TMessagesCode = Convert.ToString(response.transactionResponse.messages[0].code);
                    objOrderTransactionsResponse.TMessagesDescription = Convert.ToString(response.transactionResponse.messages[0].description);
                }
                else
                {
                    try
                    {
                        objOrderTransactionsResponse.TMessagesCode = Convert.ToString(response.transactionResponse.errors[0].errorCode);
                        objOrderTransactionsResponse.TMessagesDescription = Convert.ToString(response.transactionResponse.errors[0].errorText);
                    }
                    catch
                    {
                        objOrderTransactionsResponse.TMessagesCode = null;
                        objOrderTransactionsResponse.TMessagesDescription = null;
                    }

                }
            }

            generalService.OrderTransactionsResponse(objOrderTransactionsResponse);


        }

    }
   
}
