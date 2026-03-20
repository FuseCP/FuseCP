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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using OrderBoxCoreLib;
using OrderBoxDomainsLib;

using FuseCP.EnterpriseServer;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace FuseCP.Ecommerce.EnterpriseServer
{
	public class DirectiRegistrar : SystemPluginBase, IDomainRegistrar
	{
		static DirectiRegistrar()
		{
			// Resolve OrderBoxCoreLib library
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_OrderBoxCoreLib_AssemblyResolve);
			// Resolve OrderBoxDomains library
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_OrderBoxDomainsLib_AssemblyResolve);
		}

		static System.Reflection.Assembly CurrentDomain_OrderBoxCoreLib_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			//
			if (!args.Name.Contains("OrderBoxCoreLib"))
				return null;
			//
			string assemblyFile = String.Empty;
			//
			if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)")))
			{
				assemblyFile = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "OrderBoxCoreLib\\OrderBoxCoreLib.dll");
			}
			else
			{
				assemblyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "OrderBoxCoreLib\\OrderBoxCoreLib.dll");
			}
			//
			if (!File.Exists(assemblyFile))
			{
				Trace.TraceInformation("OrderBoxCoreLib assembly could not be found at {0}", assemblyFile);
				return null;
			}
			//
			return Assembly.LoadFrom(assemblyFile);
		}

		static System.Reflection.Assembly CurrentDomain_OrderBoxDomainsLib_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			//
			if (!args.Name.Contains("OrderBoxDomainsLib"))
				return null;
			//
			string assemblyFile = String.Empty;
			//
			if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)")))
			{
				assemblyFile = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "OrderBoxDomainsLib\\OrderBoxDomainsLib.dll");
			}
			else
			{
				assemblyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "OrderBoxDomainsLib\\OrderBoxDomainsLib.dll");
			}
			//
			if (!File.Exists(assemblyFile))
			{
				Trace.TraceInformation("OrderBoxDomainsLib assembly could not be found at {0}", assemblyFile);
				return null;
			}
			//
			return Assembly.LoadFrom(assemblyFile);
		}

		#region Registrar's constants

		public const string DEMO_SERVICE_URL = "http://api.onlyfordemo.net/anacreon/servlet/APIv3-XML";
		public const string LIVE_SERVICE_URL = "http://www.myorderbox.com/anacreon/servlet/APIv3-XML";

		public const string DEMO_SECURE_SERVICE_URL = "https://api.onlyfordemo.net/anacreon/servlet/APIv3-XML";
		public const string LIVE_SECURE_SERVICE_URL = "https://www.myorderbox.com/anacreon/servlet/APIv3-XML";

		public const string SERVICE_LANGUAGE = "en";

		public const string RESELLER_ROLE = "reseller";

		public const string CONTACT_ID = "contactid";
		public const string NO_INVOICE = "NoInvoice";
		public const string STATUS = "status";
		public const string ENTITY_ID = "entityid";
		public const string NO_OF_YEARS = "noofyears";
		public const string EXPIRY_DATE = "expirydate";
		public const string ENDTIME = "endtime";
		public const string EAQID = "eaqid";
		public const string ACTION_STATUS_DESC = "actionstatusdesc";
		public const string ERROR = "error";
		public const string RECORDS_ON_PAGE = "recsonpage";
		public const string RECORDS_IN_DB = "recsindb";

		#endregion

		#region Properties

		public override string[] SecureSettings
		{
			get
			{
				return new string[] { DirectiSettings.PASSWORD };
			}
		}

		/// <summary>
		/// Gets whether plug-in use SSL secure channel
		/// </summary>
		public bool SecureChannel
		{
			get
			{
				return Convert.ToBoolean(PluginSettings[DirectiSettings.SECURE_CHANNEL]);
			}
		}
		/// <summary>
		/// Gets whether plug-in running in live mode
		/// </summary>
		public bool LiveMode
		{
			get
			{
				return Convert.ToBoolean(PluginSettings[DirectiSettings.LIVE_MODE]);
			}
		}
		/// <summary>
		/// Gets service username
		/// </summary>
		public string Username
		{
			get { return PluginSettings[DirectiSettings.USERNAME]; }
		}
		/// <summary>
		/// Gets service password
		/// </summary>
		public string Password
		{
			get { return PluginSettings[DirectiSettings.PASSWORD]; }
		}
		/// <summary>
		/// Gets service account parent id
		/// </summary>
		public int ParentId
		{
			get
			{
				return Convert.ToInt32(PluginSettings[DirectiSettings.PARENT_ID]);
			}
		}
		/// <summary>
		/// Gets service url
		/// </summary>
		public string ServiceUrl
		{
			get
			{
				if (LiveMode)
				{
					if (SecureChannel)
						return LIVE_SECURE_SERVICE_URL;
					// return unsecured service url
					return LIVE_SERVICE_URL;
				}
				if (SecureChannel)
					return DEMO_SECURE_SERVICE_URL;
				// return unsecured channel
				return DEMO_SERVICE_URL;
			}
		}

		public bool SubAccountRequired
		{
			get { return true; }
		}

		#endregion

		#region Helper routines

		private static string[] GetDomainNames(params string[] domains)
		{
			List<string> names = new List<string>();

			foreach (string domain in domains)
			{
				string[] parts = domain.Split('.');
				names.Add(parts[0]);
			}

			return names.ToArray();
		}

		private static string[] GetTopLevelDomains(params string[] domains)
		{
			int length = domains.Length;
			List<string> tlds = new List<string>();

			foreach (string domain in domains)
			{
				string[] parts = domain.Split('.');
				tlds.Add(parts[1]);
			}

			return tlds.ToArray();
		}

		private string GetDialingAreaCode(string phone)
		{
			if (String.IsNullOrEmpty(phone))
				return "000";

			char[] symbols = phone.ToCharArray();

			StringBuilder builder = new StringBuilder();

			int count = 0, index = 0;
			while (count < 3 && index < symbols.Length)
			{
				if (Char.IsDigit(symbols[index]))
				{
					builder.Append(symbols[index]);
					count++;
				}

				index++;
			}

			return builder.ToString();
		}

		private string GetDialingNumber(string number)
		{
			if (String.IsNullOrEmpty(number))
				return "00000000000";

			StringBuilder builder = new StringBuilder();
			char[] symbols = number.ToCharArray();

			int count = 0, index = 0;

			while (index < symbols.Length)
			{
				if (count == 3 && Char.IsDigit(symbols[index]))
					builder.Append(symbols[index]);
				else if (Char.IsDigit(symbols[index]))
					count++;

				index++;
			}

			return builder.ToString();
		}

		private string GetCompanyName(string companyName)
		{
			if (String.IsNullOrEmpty(companyName))
				return "For Personal Usage Only";

			return companyName;
		}

		private string GetAddress(string address)
		{
			if (String.IsNullOrEmpty(address))
				return "-";

			return address;
		}

		#endregion

		#region IDomainRegistrar Members

		private string registrarName;

		public string RegistrarName
		{
			get { return registrarName; }
			set { registrarName = value; }
		}

		public void AssemlyResolverTest()
		{
			// OrderBoxCoreLib test
			Customer customer = new Customer();
			// OrderBoxDomainsLib test
			DomOrder domOrder = new DomOrder();
		}

		public bool CheckSubAccountExists(string emailAddress)
		{
			if (String.IsNullOrEmpty(emailAddress))
				throw new ArgumentNullException("emailAddress");

			// check sub account exists
			OrderBoxCoreLib.APIKit.Properties.Url = ServiceUrl;
			// create an instance
			OrderBoxCoreLib.Customer customer = new OrderBoxCoreLib.Customer();
			// search a user by email address
			Hashtable srchResults = customer.list(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
				ParentId, null, null, emailAddress, null, null, null, null, null, null, null, null, null, 10, 1, null);
			// check search result
			if (Convert.ToInt32(srchResults[RECORDS_ON_PAGE]) > 0 &&
				Convert.ToInt32(srchResults[RECORDS_IN_DB]) > 0)
				return true;
			// customer account not found
			return false;
		}

		public int GetCustomerAccountId(string emailAddress)
		{
			// check email address
			if (String.IsNullOrEmpty(emailAddress))
				throw new ArgumentNullException("emailAddress");
			// create a customer
			OrderBoxCoreLib.APIKit.Properties.Url = ServiceUrl;
			// init customer api
			OrderBoxCoreLib.Customer customer = new OrderBoxCoreLib.Customer();
			// check does the user is already Directi customer
			int customerId = customer.getCustomerId(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
				ParentId, emailAddress);
			// throws an exception
			if (customerId <= 0)
				throw new Exception("Couldn't find customer with the specified email: " + emailAddress);
			//
			return customerId;
		}

		public int GetDefaultContactId(int customerId)
		{
			// Default Contact Section
			OrderBoxDomainsLib.APIKit.Properties.Url = ServiceUrl;
			// init contact api
			OrderBoxDomainsLib.DomContact contact = new OrderBoxDomainsLib.DomContact();
			// check whether a contact is already exists
			Hashtable ctHash = contact.listNames(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
				ParentId, customerId);
			// throws an exception
			if (!ctHash.ContainsKey("1"))
				throw new Exception("Couldn't find default contact for the specified account");
			// gets contact info
			Hashtable infoHash = (Hashtable)ctHash["1"];
			// return result
			return Convert.ToInt32(infoHash[CONTACT_ID]);
		}

		public int CreateCustomerAccount(ContractAccount accountInfo)
		{
			// setup url
			OrderBoxCoreLib.APIKit.Properties.Url = ServiceUrl;
			// init customer api
			OrderBoxCoreLib.Customer customer = new OrderBoxCoreLib.Customer();
			// create customer account if it doesn't exist
			int customerId = customer.addCustomer(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
                ParentId, accountInfo[ContractAccount.EMAIL], accountInfo[ContractAccount.PASSWORD],
                String.Concat(accountInfo[ContractAccount.FIRST_NAME], " ", accountInfo[ContractAccount.LAST_NAME]),
                GetCompanyName(accountInfo[ContractAccount.COMPANY_NAME]), GetAddress(accountInfo[ContractAccount.ADDRESS]),
                GetAddress(null), GetAddress(null), accountInfo[ContractAccount.CITY], accountInfo[ContractAccount.STATE],
                accountInfo[ContractAccount.COUNTRY], accountInfo[ContractAccount.ZIP],
                GetDialingAreaCode(accountInfo[ContractAccount.PHONE_NUMBER]),
                GetDialingNumber(accountInfo[ContractAccount.PHONE_NUMBER]), String.Empty, String.Empty,
                GetDialingAreaCode(accountInfo[ContractAccount.FAX_NUMBER]),
                GetDialingNumber(accountInfo[ContractAccount.FAX_NUMBER]), "en");
			// setup url
			OrderBoxDomainsLib.APIKit.Properties.Url = ServiceUrl;
			// init contact api
			OrderBoxDomainsLib.DomContact contact = new OrderBoxDomainsLib.DomContact();
			// create default contact
			int defaultContactId = contact.addDefaultContact(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
				ParentId, customerId);
			// return result
			return customerId;
		}

		public int AddCustomerContact(int customerId, string contactType, ContractAccount accountInfo, Hashtable extraInfo)
		{
			// setup url
			OrderBoxCoreLib.APIKit.Properties.Url = ServiceUrl;
			// init customer api
			OrderBoxDomainsLib.DomContact contact = new DomContact();
			// create customer account if it doesn't exist
			int contactId = contact.addContact(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
                ParentId, String.Concat(accountInfo[ContractAccount.FIRST_NAME], " ", accountInfo[ContractAccount.LAST_NAME]),
                accountInfo[ContractAccount.COMPANY_NAME], accountInfo[ContractAccount.EMAIL],
                GetAddress(accountInfo[ContractAccount.ADDRESS]), GetAddress(null), GetAddress(null),
                accountInfo[ContractAccount.CITY], accountInfo[ContractAccount.STATE], accountInfo[ContractAccount.COUNTRY],
                accountInfo[ContractAccount.ZIP], GetDialingAreaCode(accountInfo[ContractAccount.PHONE_NUMBER]),
                GetDialingNumber(accountInfo[ContractAccount.PHONE_NUMBER]), 
                GetDialingAreaCode(accountInfo[ContractAccount.FAX_NUMBER]),
                GetDialingNumber(accountInfo[ContractAccount.FAX_NUMBER]), customerId, contactType, extraInfo);
			//
			return contactId;
		}

		public bool IsContactValidForProduct(int contactId, string productKey)
		{
			// setup service url
			OrderBoxDomainsLib.APIKit.Properties.Url = ServiceUrl;
			//
			OrderBoxDomainsLib.DomContactExt contactExt = new DomContactExt();
			// 
			Hashtable result = contactExt.isValidRegistrantContact(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
				ParentId, new int[] {contactId}, new string[] { productKey });
			//
			Hashtable product = (Hashtable)result[productKey];
			//
			if (Convert.ToString(product[contactId.ToString()]) == "true")
				return true;
			//
			return false;
		}

		public DomainStatus CheckDomain(string domain)
		{
			// check domain not empty
			if (String.IsNullOrEmpty(domain))
				throw new ArgumentNullException("domain");

			// format values
			string[] domains = GetDomainNames(domain);
			string[] tlds = GetTopLevelDomains(domain);
			// setup service url 
			OrderBoxDomainsLib.APIKit.Properties.Url = ServiceUrl;
			// init domain order api
			OrderBoxDomainsLib.DomOrder domOrder = new OrderBoxDomainsLib.DomOrder();
			// check domain availability
			Hashtable directiResult = domOrder.checkAvailabilityMultiple(
				Username,
				Password,
				RESELLER_ROLE,
				SERVICE_LANGUAGE,
				ParentId,
				domains,
				tlds,
				false
			);
			// get result by domain
			Hashtable bunch = (Hashtable)directiResult[domain];
			// check result status
			if (String.Compare((String)bunch[STATUS], "available", true) == 0)
				return DomainStatus.NotFound;
			// return result
			return DomainStatus.Registered;
		}

		public void RegisterDomain(DomainNameSvc domainSvc, ContractAccount accountInfo, string[] nameServers)
		{
			int customerId = 0;
			// 1. check customer exists
			if (CheckSubAccountExists(accountInfo[ContractAccount.EMAIL]))
                customerId = GetCustomerAccountId(accountInfo[ContractAccount.EMAIL]);
			else
				customerId = CreateCustomerAccount(accountInfo);

			// obtain default contact id
			int contactId = GetDefaultContactId(customerId);

			// check for demo mode if so then set demo-nameservers.
			if (!LiveMode)
				nameServers = new string[] { "ns1.onlyfordemo.net", "ns2.onlyfordemo.net" };

			// fill parameters hashtable
			Hashtable domainHash = new Hashtable();
			// copy domain name
			domainHash[domainSvc.Fqdn] = domainSvc.PeriodLength.ToString();
			// setup service url
			OrderBoxDomainsLib.APIKit.Properties.Url = ServiceUrl;
			// init domain order api
			OrderBoxDomainsLib.DomOrder domOrder = new OrderBoxDomainsLib.DomOrder();

			// 
			int validateAttempts = 0;

			VALIDATE_REGISTRATION:
				// validate params
				Hashtable valResult = domOrder.validateDomainRegistrationParams(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
					ParentId, domainHash, new ArrayList(nameServers), contactId, contactId, contactId,
					contactId, customerId, NO_INVOICE);

			// get domain name hashtable
			valResult = (Hashtable)valResult[domainSvc.Fqdn];
			// check validation status
			if ((String)valResult[STATUS] == "error")
			{
				// try to update extended contact fields and re-validate params
				if (validateAttempts == 0 && domainSvc.Fqdn.EndsWith(".us"))
				{
					validateAttempts++;
					//
					OrderBoxDomainsLib.DomContactExt contactExt = new DomContactExt();
					// fill extension hash
					Hashtable exthash = new Hashtable();
					Hashtable domus = new Hashtable();
					domus["nexusCategory"] = domainSvc["NexusCategory"];
					domus["applicationPurpose"] = domainSvc["ApplicationPurpose"];
					exthash["domus"] = domus;
					// set default contact extensions
					bool succeed = contactExt.setContactDetails(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
						ParentId, contactId, exthash, "domus");
					// check result
					if (succeed)
						goto VALIDATE_REGISTRATION;
				}
				//
				throw new Exception((String)valResult[ERROR]);
			}

			// register domain
			Hashtable orderResult = domOrder.addWithoutValidation(Username, Password, RESELLER_ROLE, SERVICE_LANGUAGE,
				ParentId, domainHash, new ArrayList(nameServers), contactId, contactId, contactId,
				contactId, customerId, NO_INVOICE);

			// switch to the nested data bunch
			orderResult = (Hashtable)orderResult[domainSvc.Fqdn];

			// check returned status
			switch ((String)orderResult[STATUS])
			{
				case "error": // error
					throw new Exception(Convert.ToString(orderResult[ERROR]));
				case "Failed": // error
					throw new Exception(Convert.ToString(orderResult[ACTION_STATUS_DESC]));
				case "Success": // success
				case "InvoicePaid": // success
					// we are success so copy order number
					domainSvc[EAQID] = Convert.ToString(orderResult[EAQID]);
					domainSvc[ENTITY_ID] = Convert.ToString(orderResult[ENTITY_ID]);
					break;
			}
		}

		public void RenewDomain(DomainNameSvc domainSvc, ContractAccount accountInfo, string[] nameServers)
		{
			int customerId = GetCustomerAccountId(accountInfo[ContractAccount.EMAIL]);
			// setup service url
			OrderBoxDomainsLib.APIKit.Properties.Url = ServiceUrl;
			// init domain order api
			OrderBoxDomainsLib.DomOrder domOrder = new OrderBoxDomainsLib.DomOrder();
			
			// Get all domain name registration details
			Hashtable domainDetails = domOrder.getDetailsByDomain(Username, Password, RESELLER_ROLE,
				SERVICE_LANGUAGE, ParentId, domainSvc.Fqdn, new ArrayList { "All" });
			
			// fill parameters hashtable
			Hashtable domainHash = new Hashtable
			{
				{
					domainSvc.Fqdn,
					new Hashtable
					{
						{ENTITY_ID, domainDetails[ENTITY_ID]},
						{NO_OF_YEARS, domainSvc.PeriodLength.ToString()},
						{EXPIRY_DATE, domainDetails[ENDTIME]}
					}
				}
			};

			// Send renewal request to the registrar
			Hashtable orderResult = domOrder.renewDomain(Username, Password, RESELLER_ROLE,
				SERVICE_LANGUAGE, ParentId, domainHash, NO_INVOICE);
			
			// switch to the nested data bunch of the result received
			orderResult = (Hashtable)orderResult[domainSvc.Fqdn];

			// check returned status
			switch ((String)orderResult[STATUS])
			{
				case "error": // error
					throw new Exception(Convert.ToString(orderResult[ERROR]));
				case "Failed": // error
					throw new Exception(Convert.ToString(orderResult[ACTION_STATUS_DESC]));
				case "Success": // success
				case "InvoicePaid": // success
					// we are success so copy order number
					domainSvc[EAQID] = Convert.ToString(orderResult[EAQID]);
					domainSvc[ENTITY_ID] = Convert.ToString(orderResult[ENTITY_ID]);
					break;
			}
		}

		public TransferDomainResult TransferDomain(CommandParams args, DomainContacts contacts)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
