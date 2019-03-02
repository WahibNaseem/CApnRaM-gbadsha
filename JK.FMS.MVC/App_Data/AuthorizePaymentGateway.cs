using System;
using System.Collections.Generic;
using AuthorizeNet;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using System.Net;


/// <summary>
/// Summary description for ChargeCreditCard
/// </summary>
public class AuthorizePaymentGateway
{
    public AuthorizePaymentGateway()
    {

    }

    /// <summary>
    /// Charge Credit Card
    /// </summary>
    /// <param name="apiLoginID"></param>
    /// <param name="apiTransactionKey"></param>
    /// <param name="amount"></param>
    /// <param name="creditCard"></param>
    /// <param name="billingAddress"></param>
    /// <param name="lineItems"></param>
    /// <param name="environment"></param>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    public static dynamic ChargeCreditCard(String apiLoginID, String apiTransactionKey, decimal amount, creditCardType creditCard, customerAddressType billingAddress, lineItemType[] lineItems, string environment, string orderNo)
    {
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = environment == "LiveMode" ? AuthorizeNet.Environment.PRODUCTION : AuthorizeNet.Environment.SANDBOX;

        // define the merchant information (authentication / transaction id)
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = apiLoginID,
            ItemElementName = ItemChoiceType.transactionKey,
            Item = apiTransactionKey,
        };

        //standard api call to retrieve response
        var paymentType = new paymentType { Item = creditCard };

        var customer = new customerDataType()
        {
            //Passing order no to customer no. we have user id which is stored with order no
            type = customerTypeEnum.individual,
            id = orderNo

        };
        var transactionRequest = new transactionRequestType
        {
            transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

            amount = amount,
            payment = paymentType,
            billTo = billingAddress,
            lineItems = lineItems,
            shipTo = billingAddress,
            customer = customer,
            profile = new customerProfilePaymentType()
            {
                createProfile = true
            }
        };

        var request = new createTransactionRequest { transactionRequest = transactionRequest };

        ////Authorize.Net has changed thier TLS. 
        //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        // instantiate the controller that will call the service
        var controller = new createTransactionController(request);
        controller.Execute();

        // get the response from the service (errors contained if any)
        var response = controller.GetResultCode();

        if (response == messageTypeEnum.Ok)
        {
            return controller.GetApiResponse();
        }
        else if (response == messageTypeEnum.Error)
        {
            var responseError = controller.GetApiResponse();
            if (responseError == null)
            {
                return controller.GetErrorResponse();
            }
            return responseError;
        }
        else {
            return controller.GetApiResponse();
        }
    }

    
    /// <summary>
    /// CreateCustomerProfileFromTransaction
    /// </summary>
    /// <param name="apiLoginID"></param>
    /// <param name="apiTransactionKey"></param>
    /// <param name="transactionId"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static dynamic CreateCustomerProfileFromTransaction(string apiLoginID, string apiTransactionKey, string transactionId, string environment)
    {

        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = environment == "LiveMode" ? AuthorizeNet.Environment.PRODUCTION : AuthorizeNet.Environment.SANDBOX;
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = apiLoginID,
            ItemElementName = ItemChoiceType.transactionKey,
            Item = apiTransactionKey,
        };

        var request = new createCustomerProfileFromTransactionRequest { transId = transactionId };

        //Authorize.Net has changed thier TLS. 
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        var controller = new createCustomerProfileFromTransactionController(request);
        controller.Execute();

        createCustomerProfileResponse response = controller.GetApiResponse();



        return response;
    }


    /// <summary>
    /// CreateRecurringSubscriptionfromCustomerProfile
    /// </summary>
    /// <param name="apiLoginID"></param>
    /// <param name="apiTransactionKey"></param>
    /// <param name="intervalLength"></param>
    /// <param name="customerProfileId"></param>
    /// <param name="customerPaymentProfileId"></param>
    /// <param name="customerAddressId"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static dynamic CreateRecurringSubscriptionfromCustomerProfile(String apiLoginID, String apiTransactionKey,
            string customerProfileId, string customerPaymentProfileId, string customerAddressId, string environment, 
        //creditCardType creditCard,
        decimal amount, short intervalLength, DateTime startDate, short totalOccurrences, short trialOccurrences, decimal trialAmount, int unit)
    {

        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = environment == "LiveMode" ? AuthorizeNet.Environment.PRODUCTION : AuthorizeNet.Environment.SANDBOX;

        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = apiLoginID,
            ItemElementName = ItemChoiceType.transactionKey,
            Item = apiTransactionKey
        };

        paymentScheduleTypeInterval interval = new paymentScheduleTypeInterval();
        interval.length = intervalLength;
        if (unit == 0)
        {
            interval.unit = ARBSubscriptionUnitEnum.days;
        }
        else if (unit == 1)
        {
            interval.unit = ARBSubscriptionUnitEnum.months;
        }

        paymentScheduleType schedule = new paymentScheduleType
        {
            interval = interval,
            startDate = startDate,      // start date should be tomorrow
            totalOccurrences = totalOccurrences, // 999 indicates no end date
            trialOccurrences = trialOccurrences
        };

        #region Payment Information
        //paymentType cc = new paymentType { Item = creditCard };
        #endregion

        customerProfileIdType customerProfile = new customerProfileIdType()
        {
            customerProfileId = customerProfileId,
            customerPaymentProfileId = customerPaymentProfileId,
            customerAddressId = customerAddressId
        };

        ARBSubscriptionType subscriptionType = new ARBSubscriptionType()
        {
            amount = amount,
            trialAmount = trialAmount,
            paymentSchedule = schedule,
            profile = customerProfile
        };

        var request = new ARBCreateSubscriptionRequest { subscription = subscriptionType };

        //Authorize.Net has changed thier TLS. 
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        var controller = new ARBCreateSubscriptionController(request);          // instantiate the controller that will call the service
        controller.Execute();

        ARBCreateSubscriptionResponse response = controller.GetApiResponse();   // get the response from the service (errors contained if any)

        return response;
    }


    /// <summary>
    /// ChargeCustomerProfile
    /// </summary>
    /// <param name="apiLoginID"></param>
    /// <param name="apiTransactionKey"></param>
    /// <param name="customerProfileId"></param>
    /// <param name="customerPaymentProfileId"></param>
    /// <param name="environment"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static dynamic ChargeCustomerProfile(String apiLoginID, String apiTransactionKey, string customerProfileId,
            string customerPaymentProfileId, string environment, decimal amount)
    {

        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = environment == "LiveMode" ? AuthorizeNet.Environment.PRODUCTION : AuthorizeNet.Environment.SANDBOX;

        // define the merchant information (authentication / transaction id)
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = apiLoginID,
            ItemElementName = ItemChoiceType.transactionKey,
            Item = apiTransactionKey
        };

        //create a customer payment profile
        customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
        profileToCharge.customerProfileId = customerProfileId;
        profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = customerPaymentProfileId };

        var transactionRequest = new transactionRequestType
        {
            transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // refund type
            amount = amount,
            profile = profileToCharge
        };

        var request = new createTransactionRequest { transactionRequest = transactionRequest };

        // instantiate the collector that will call the service
        var controller = new createTransactionController(request);

        //Authorize.Net has changed thier TLS. 
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        controller.Execute();

        // get the response from the service (errors contained if any)
        var response = controller.GetApiResponse();

        return response;
    }


    /// <summary>
    /// Update Customer Payment Profile
    /// </summary>
    /// <param name="ApiLoginID"></param>
    /// <param name="ApiTransactionKey"></param>
    /// <param name="customerProfileId"></param>
    /// <param name="customerPaymentProfileId"></param>
    /// <returns></returns>
    public static ANetApiResponse UpdateCustomerPaymentProfile(String apiLoginID, String apiTransactionKey, creditCardType creditCard, customerAddressType billingAddress, string environment,string customerProfileId, string customerPaymentProfileId)
    {
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = environment == "LiveMode" ? AuthorizeNet.Environment.PRODUCTION : AuthorizeNet.Environment.SANDBOX;

        // define the merchant information (authentication / transaction id)
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = apiLoginID,
            ItemElementName = ItemChoiceType.transactionKey,
            Item = apiTransactionKey,
        };

        var paymentType = new paymentType { Item = creditCard };

        var paymentProfile = new customerPaymentProfileExType
        {
            billTo = new customerAddressType
            {
                // change information as required for billing
                firstName = billingAddress.firstName,
                lastName = billingAddress.lastName,
                address = billingAddress.address,
                zip = billingAddress.zip
            },
            payment = paymentType,
            customerPaymentProfileId = customerPaymentProfileId
        };

        var request = new updateCustomerPaymentProfileRequest();
        request.customerProfileId = customerProfileId;
        request.paymentProfile = paymentProfile;
        request.validationMode = validationModeEnum.liveMode;


        // instantiate the controller that will call the service
        var controller = new updateCustomerPaymentProfileController(request);

        //Authorize.Net has changed thier TLS. 
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        controller.Execute();

        // get the response from the service (errors contained if any)
        var response = controller.GetResultCode();

        if (response == messageTypeEnum.Ok)
        {
            return controller.GetApiResponse();
        }
        else if (response == messageTypeEnum.Error)
        {
            var responseError = controller.GetApiResponse();
            if (responseError == null)
            {
                return controller.GetErrorResponse();
            }
            return responseError;
        }
        else
        {
            return controller.GetApiResponse();
        }
    }


    public static ANetApiResponse ValidateCustomerPaymentProfile(String ApiLoginID, String ApiTransactionKey, string customerProfileId, string customerPaymentProfileId)
    {
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
        // define the merchant information (authentication / transaction id)
        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        {
            name = ApiLoginID,
            ItemElementName = ItemChoiceType.transactionKey,
            Item = ApiTransactionKey,
        };

        var request = new validateCustomerPaymentProfileRequest();
        request.customerProfileId = customerProfileId;
        request.customerPaymentProfileId = customerPaymentProfileId;
        request.validationMode = validationModeEnum.liveMode;


        // instantiate the controller that will call the service
        var controller = new validateCustomerPaymentProfileController(request);

        //Authorize.Net has changed thier TLS. 
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        controller.Execute();
//        string Status="";
        // get the response from the service (errors contained if any)
        var response = controller.GetApiResponse();

        if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
        {
            Console.WriteLine(response.messages.message[0].text);
        }
        else if (response != null)
        {
            Console.WriteLine("Error: " + response.messages.message[0].code + "  " +
                              response.messages.message[0].text);
        }

        return response;
    }

}
