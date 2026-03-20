// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;

namespace FuseCP.Ecommerce.EnterpriseServer
{
	/// <summary>
	/// Payment provider for the Authorize.NET Gateway 
	/// </summary>
	public class AuthorizeNetProvider : SystemPluginBase, IPaymentGatewayProvider
	{
		public const string DEMO_SERVICE_URL = "https://test.authorize.net/gateway/transact.dll";
		public const string LIVE_SERVICE_URL = "https://secure.authorize.net/gateway/transact.dll";
		public const char DELIMITER_CHAR = '|';
		public const string API_VERSION = "3.1";
		public const string TRANSACTION_TYPE = "AUTH_CAPTURE";
		public const string PAYMENT_METHOD = "CC";

		public const string MD5_INVALID_MSG = "MD5 hash value received is either incorrect or modified.";

		public override string[] SecureSettings
		{
			get
			{
				return new string[] { AuthNetSettings.MD5_HASH, AuthNetSettings.TRANSACTION_KEY };
			}
		}

		/// <summary>
		/// Gets Authorize.Net account is demo
		/// </summary>
		public bool DemoAccount
		{
			get
			{
				return Convert.ToBoolean(PluginSettings[AuthNetSettings.DEMO_ACCOUNT]);
			}
		}
		/// <summary>
		/// Gets whether provider is running in live mode
		/// </summary>
		public bool LiveMode
		{
			get
			{
				return Convert.ToBoolean(PluginSettings[AuthNetSettings.LIVE_MODE]);
			}
		}
		/// <summary>
		/// Gets whether email confirmations enabled
		/// </summary>
		public bool SendConfirmation
		{
			get
			{
				return Convert.ToBoolean(PluginSettings[AuthNetSettings.SEND_CONFIRMATION]);
			}
		}
		/// <summary>
		/// Get MD5 hash value for the account
		/// </summary>
		public string MD5_Hash
		{
			get { return PluginSettings[AuthNetSettings.MD5_HASH]; }
		}
		/// <summary>
		/// Gets transaction key for the account
		/// </summary>
		public string TransactionKey
		{
			get { return PluginSettings[AuthNetSettings.TRANSACTION_KEY]; }
		}
		/// <summary>
		/// Gets Authorize.Net account username
		/// </summary>
		public string Username
		{
			get { return PluginSettings[AuthNetSettings.USERNAME]; }
		}
		/// <summary>
		/// Gets merchant email to send confirmations by email
		/// </summary>
		public string MerchantEmail
		{
			get { return PluginSettings[AuthNetSettings.MERCHANT_EMAIL]; }
		}
		/// <summary>
		/// Gets Authorize.Net service url
		/// </summary>
		public string ServiceUrl
		{
			get
			{
				if (DemoAccount)
					return DEMO_SERVICE_URL;
				return LIVE_SERVICE_URL;
			}
		}

		#region IPaymentGatewayProvider Members

		public TransactionResult SubmitPaymentTransaction(CheckoutDetails details)
		{
			//init result structure
			TransactionResult ret = new TransactionResult();
			//create request content
			string data = GetRequestData(details);
			// read service response
			AIMResponse aimResponse = null;
			using (HttpClient httpClient = new HttpClient())
			using (StringContent content = new StringContent(data, Encoding.ASCII, "application/x-www-form-urlencoded"))
			using (HttpResponseMessage response = httpClient.PostAsync(ServiceUrl, content).GetAwaiter().GetResult())
			using (Stream responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
			{
				response.EnsureSuccessStatusCode();
				aimResponse = new AIMResponse(responseStream, DELIMITER_CHAR);
			}
			// copy raw service response
			ret.RawResponse = aimResponse.RawResponse;
			// read service response status
			switch (aimResponse[AIMField.ResponseCode])
			{
				case "1": //This transaction has been approved.
				case "4": //This transaction is being held for review.
					// check MD5 signature
					if (!CheckResponseSignature(Username, MD5_Hash, aimResponse[AIMField.TransactionId], details[CheckoutKeys.Amount],
						aimResponse[AIMField.ResponseSignature]))
					{
						throw new Exception(MD5_INVALID_MSG);
					}
					//
					ret.Succeed = true;
					//
					ret.TransactionId = aimResponse[AIMField.TransactionId];
					// mark transaction as a completed
					ret.TransactionStatus = TransactionStatus.Approved;
					//
					break;
				case "2": // This transaction has been declined.
				case "3": // There has been an error processing this transaction.
					//
					ret.StatusCode = String.Concat(AuthorizeNetKeys.ErrorPrefix, aimResponse[AIMField.ResponseCode], 
						aimResponse[AIMField.ResponseReasonCode]);
					//
					ret.Succeed = false;
					//
					ret.TransactionStatus = TransactionStatus.Declined;
					//
					break;
			}
			// return result
			return ret;
		}
		#endregion

		private bool CheckResponseSignature(string login, string md5hash, 
			string transactId, string amount, string signature)
		{
			//input string = "MD5 Hash Value"+"API Login ID"+"Trans ID"+"Amount"
			byte[] data = Encoding.ASCII.GetBytes(md5hash + login + transactId + amount);
			byte[] result;
			using (MD5 md5 = MD5.Create())
			{
				result = md5.ComputeHash(data);
			}
			StringBuilder sb = new StringBuilder();
			foreach (byte b in result)
			{
				sb.Append(b.ToString("X2"));
			}
			// validate response signature
			return (sb.ToString() == signature);
		}
		
		private string GetRequestData(CheckoutDetails details)
		{
			StringBuilder sb = new StringBuilder();

			AddRequestData(CheckoutKeys.CardNumber, AuthorizeNetKeys.CardNumber, sb, details);
			AddRequestData(CheckoutKeys.VerificationCode, AuthorizeNetKeys.VerificationCode, sb, details);

			// expire date
			string expireDate = String.Concat(details[CheckoutKeys.ExpireMonth], details[CheckoutKeys.ExpireYear]);
			AddRequestData(AuthorizeNetKeys.ExpirationDate, expireDate, sb);

			AddRequestData(CheckoutKeys.Amount, AuthorizeNetKeys.Amount, sb, details);
			
			//TODO: Add currency support
			AddRequestData(CheckoutKeys.Currency, AuthorizeNetKeys.CurrencyCode, sb, details);

			AddRequestData(CheckoutKeys.InvoiceNumber, AuthorizeNetKeys.InvoiceNumber, sb, details);
            AddRequestData(CheckoutKeys.ContractNumber, AuthorizeNetKeys.TransDescription, sb, details);

			AddRequestData(CheckoutKeys.FirstName, AuthorizeNetKeys.FirstName, sb, details);
			AddRequestData(CheckoutKeys.LastName, AuthorizeNetKeys.LastName, sb, details);
			// email is shipping
			AddRequestData(CheckoutKeys.CustomerEmail, AuthorizeNetKeys.CustomerEmail, sb, details);
			AddRequestData(CheckoutKeys.Address, AuthorizeNetKeys.Address, sb, details);
			AddRequestData(CheckoutKeys.Zip, AuthorizeNetKeys.Zip, sb, details);
			AddRequestData(CheckoutKeys.City, AuthorizeNetKeys.City, sb, details);
			AddRequestData(CheckoutKeys.State, AuthorizeNetKeys.State, sb, details);
			AddRequestData(CheckoutKeys.Country, AuthorizeNetKeys.Country, sb, details);
			AddRequestData(CheckoutKeys.Phone, AuthorizeNetKeys.Phone, sb, details);
			AddRequestData(CheckoutKeys.Fax, AuthorizeNetKeys.Fax, sb, details);
			AddRequestData(CheckoutKeys.CustomerId, AuthorizeNetKeys.CustomerId, sb, details);
			AddRequestData(CheckoutKeys.IPAddress, AuthorizeNetKeys.IPAddress, sb, details);

			// shipping information
			AddRequestData(CheckoutKeys.ShipToFirstName, AuthorizeNetKeys.ShipToFirstName, sb, details);
			AddRequestData(CheckoutKeys.ShipToLastName, AuthorizeNetKeys.ShipToLastName, sb, details);
			AddRequestData(CheckoutKeys.ShipToCompany, AuthorizeNetKeys.ShipToCompany, sb, details);
			AddRequestData(CheckoutKeys.ShipToZip, AuthorizeNetKeys.ShipToZip, sb, details);
			AddRequestData(CheckoutKeys.ShipToAddress, AuthorizeNetKeys.ShipToAddress, sb, details);
			AddRequestData(CheckoutKeys.ShipToCity, AuthorizeNetKeys.ShipToCity, sb, details);
			AddRequestData(CheckoutKeys.ShipToState, AuthorizeNetKeys.ShipToState, sb, details);
			AddRequestData(CheckoutKeys.ShipToCountry, AuthorizeNetKeys.ShipToCountry, sb, details);
			
			// service settings
			// copy account username
			AddRequestData(AuthorizeNetKeys.Account, Username, sb);
			// copy response delimiter char
			AddRequestData(AuthorizeNetKeys.DelimiterChar, DELIMITER_CHAR.ToString(), sb);
			// copy transaction key
			AddRequestData(AuthorizeNetKeys.TransactionKey, TransactionKey, sb);
			// copy send confirmation flag & merchant email
			if (SendConfirmation)
			{
				AddRequestData(AuthorizeNetKeys.MerchantEmail, MerchantEmail, sb);
				AddRequestData(AuthorizeNetKeys.SendConfirmation, "TRUE", sb);
			}
			else
			{
				AddRequestData(AuthorizeNetKeys.SendConfirmation, "FALSE", sb);
			}
			// copy API version
			AddRequestData(AuthorizeNetKeys.Version, API_VERSION, sb);
			// copy demo mode flag
			if (!LiveMode)
				AddRequestData(AuthorizeNetKeys.DemoMode, "TRUE", sb);
			// copy payment method
			AddRequestData(AuthorizeNetKeys.PaymentMethod, PAYMENT_METHOD, sb);
			// copy transaction type
			AddRequestData(AuthorizeNetKeys.TransactType, TRANSACTION_TYPE, sb);
			// copy delim data flag
			AddRequestData(AuthorizeNetKeys.DelimData, "TRUE", sb);
			// copy relay response flag
			AddRequestData(AuthorizeNetKeys.RelayResponse, "FALSE", sb);
			// return result
			return sb.ToString();
		}

		private void AddRequestData(string key1, string key2, StringBuilder sb, CheckoutDetails details)
		{
			AddRequestData(key2, details[key1], sb);
		}

		private void AddRequestData(string key, StringBuilder sb, CheckoutDetails details)
		{
			AddRequestData(key, details[key], sb);
		}

		private void AddRequestData(string key, string value, StringBuilder sb)
		{
			if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
			{
				if (sb.Length == 0)
					sb.AppendFormat("{0}={1}", key, value);
				else
					sb.AppendFormat("&{0}={1}", key, value);
			}
		}
	}
}
