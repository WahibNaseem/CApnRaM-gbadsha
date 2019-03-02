using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Business.Enumeration
{
    /// <summary>
    /// This enumeration contains error codes specific to PTS
    /// Following is the summary of code and related errors
    /// 10xx - General errors
    /// 20xx - Network errors
    /// 30xx - Database errors
    /// 40xx - Service error
    /// 90xx - Business error
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// No error.
        /// </summary>
        [Description("No error.")]
        None = 0,

        #region General Errors

        /// <summary>
        /// Error code indicating that the post data request does not follow standard format.
        /// </summary>
        [Description("The request data is invalid.")]
        InvalidRequest = 1001,

        /// <summary>
        /// Error code indicating that the post data request does not contain a valid ID.
        /// </summary>
        [Description("The request id is invalid.")]
        InvalidRequestID = 1002,

        #endregion

        #region Network Errors



        #endregion

        #region Database Errors

        /// <summary>
        /// Error code indicating that the post data request does not follow standard format.
        /// </summary>
        [Description("The request identity (unique) is invalid.")]
        InvalidID = 3001,

        #endregion

        #region Service Errors



        #endregion

        #region Business Errors

        /// <summary>
        /// Error code indicating that the post data request does not follow standard format.
        /// </summary>
        [Description("Plan name already exists.")]
        DuplicatePlanName = 9001,

        /// <summary>
        /// Error code indicating duplicate security name
        /// </summary>
        [Description("Security Name already exists.")]
        DuplicateSecurity = 9002,

        /// <summary>
        /// Error code indicating duplicate document name
        /// </summary>
        [Description("Document Name already exists.")]
        DuplicateDocument = 9005,

        /// <summary>
        /// Error code indicating duplicate state code for withholding
        /// </summary>
        [Description("State Withholding already exists.")]
        DuplicateStateWithholding = 9006,

        /// <summary>
        /// Error code indicating that the post data request does not follow standard format.
        /// </summary>
        [Description("Year is not valid per business scope.")]
        InvalidYear = 9003,

        /// <summary>
        /// Error code indicating that the post data request does not follow standard format.
        /// </summary>
        [Description("Amount exceeds contribution limit.")]
        InvalidAmount = 9004,

        #endregion

        #region Authentication Errors
        #endregion

        #region Authorization Errors
        #endregion

        /// <summary>
        /// Error code indicating that the username/password is invalid.
        /// </summary>
        [Description("Username or password is invalid.")]
        InvalidAuthentication = 2002,

        /// <summary>
        /// Error code indicating that username has already existed.
        /// </summary>
        [Description("Someone already registered with that username. Try a different one.")]
        DuplicatedUsername = 2003,

        /// <summary>
        /// Error code indicating that server can not register the user.
        /// </summary>
        [Description("We can not register at the moment. Please try again.")]
        InvalidRegistration = 2004,

        /// <summary>
        /// Error code indicating that the auth token is not valid.
        /// </summary>
        [Description("The auth token is not valid. Please try again.")]
        InvalidAuthToken = 2007,

        /// <summary>
        /// Error code indicating that the auth token is expired.
        /// </summary>
        [Description("The auth token was expired. Please login again.")]
        ExpiredAuthToken = 2008,

        /// <summary>
        /// Error code indicating that email is not valid.
        /// </summary>
        [Description("The email was used by another user. Please try another email.")]
        InvalidEmailAddress = 2009,

        /// <summary>
        /// Error code indicating file format was not supported.
        /// </summary>
        [Description("The file format was not supported. Please try again.")]
        UnsupportedFile = 2010,

        /// <summary>
        /// Error code indicating PIN length is invalid.
        /// </summary>
        [Description("PIN length should be in the range of 4 to 6 digits.")]
        InvalidPinLength = 2011,

        /// <summary>
        /// Error code indicating Username or PIN or Facility is invalid.
        /// </summary>
        [Description("Invalid facility")]
        InvalidFacility = 2012,

        /// <summary>
        /// Error code indicating no default Facility is there for the user.
        /// </summary>
        [Description("No default facility exists")]
        NoDefaultFacility = 2013,

        /// <summary>
        /// Error code indicating that the user does not have right to perform this operation.
        /// </summary>
        [Description("You are not permitted to perform this operation")]
        UnauthorizedAction = 2014,

        /// <summary>
        /// Error code indicating that the user can not change his role.
        /// </summary>
        [Description("You are not permitted to change your role")]
        InvalidSelfRoleChange = 2015
    }
}
