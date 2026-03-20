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
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

using FuseCP.Providers.HostedSolution;
using FuseCP.EnterpriseServer;


namespace FuseCP.Import.CsvBulk
{
	public enum AccountTypes
	{
		Mailbox,
		Contact,
		User,
        Room,
        Equipment,
        SharedMailbox
	}

	/// <summary>
	/// Imports mailboxes from csv file
	/// </summary>
	public class ExchangeImport
	{
		public const int ERROR_USER_WRONG_PASSWORD = -110;
		public const int ERROR_USER_WRONG_USERNAME = -109;
		public const int ERROR_USER_ACCOUNT_CANCELLED = -105;
		public const int ERROR_USER_ACCOUNT_DEMO = -106;
		public const int ERROR_USER_ACCOUNT_PENDING = -103;
		public const int ERROR_USER_ACCOUNT_SHOULD_BE_ADMINISTRATOR = -107;
		public const int ERROR_USER_ACCOUNT_SHOULD_BE_RESELLER = -108;
		public const int ERROR_USER_ACCOUNT_SUSPENDED = -104;

		private ServerContext serverContext;
		private int totalMailboxes = 0;
		private int totalContacts = 0;
		private int totalUsers = 0;
		private int index = 0;

		private int DisplayNameIndex = -1;
		private int EmailAddressIndex = -1;
		private int PasswordIndex = -1;
		private int FirstNameIndex = -1;
		private int MiddleNameIndex = -1;
		private int LastNameIndex = -1;
		private int TypeIndex = -1;
		private int AddressIndex = -1;
		private int CityIndex = -1;
		private int StateIndex = -1;
		private int ZipIndex = -1;
		private int CountryIndex = -1;
		private int JobTitleIndex = -1;
		private int CompanyIndex = -1;
		private int DepartmentIndex = -1;
		private int OfficeIndex = -1;
		private int BusinessPhoneIndex = -1;
		private int FaxIndex = -1;
		private int HomePhoneIndex = -1;
		private int MobilePhoneIndex = -1;
		private int PagerIndex = -1;
		private int WebPageIndex = -1;
		private int NotesIndex = -1;
        private int PlanIndex = -1;

        private int defaultPlanId = -1;
        private Dictionary<string, int> planName2Id = new Dictionary<string,int>();

		public ExchangeImport()
		{

		}

		/// <summary>
		/// Starts import process
		/// </summary>
		public void Start()
		{
			try
			{

				//Authenticates user
				if (!Connect(
					ConfigurationManager.AppSettings["ES.WebService"],
					ConfigurationManager.AppSettings["ES.Username"],
					ConfigurationManager.AppSettings["ES.Password"]))
					return;

				// query organization Id from DB
				int itemId = GetOrganizationId();

				//Parse csv file
				if (itemId != 0)
				{
					ProcessFile(ConfigurationManager.AppSettings["InputFile"], itemId);
				}

			}
			catch (Exception ex)
			{
				Log.WriteError("Unexpected error occured", ex);
			}
		}

		/// <summary>
		/// Returns Organization Id from database
		/// </summary>
		/// <returns></returns>
		private int GetOrganizationId()
		{
			SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
			SqlCommand cmd = new SqlCommand();

			int id = 0;
			string orgId = ConfigurationManager.AppSettings["TargetOrganization"];

			cmd.CommandText = string.Format("SELECT ItemID FROM ExchangeOrganizations WHERE OrganizationID = '{0}'", orgId);
			cmd.CommandType = CommandType.Text;
			cmd.Connection = sqlConnection;
			try
			{
				sqlConnection.Open();
				object obj = cmd.ExecuteScalar();
				if (obj == null)
				{
					Log.WriteError(string.Format("Organization '{0}' not found", orgId));
				}
				else
				{
					id = (int)obj;
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("SQL error occured", ex);
			}
			finally
			{
				if (sqlConnection != null)
					sqlConnection.Close();
			}
			return id;
		}

		private void ProcessFile(string inputFile, int orgId)
		{
			// regexp to parse csv string
			Regex regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

			try
			{
				if (!File.Exists(inputFile))
				{
					Log.WriteError(string.Format("File '{0}' not found", inputFile));
					return;
				}
				index = 0;

                GetMailboxPlans(orgId);

				using (StreamReader sr = new StreamReader(inputFile))
				{
					string line;

					// Read lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null)
					{
						//process line
						if (!ProcessLine(index, orgId, line, regex))
						{
							ShowSummary();
							return;
						}
						index++;
					}
				}
				ShowSummary();
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				Log.WriteError("Unexpected error occured", e);
			}
		}

        private void GetMailboxPlans(int orgId)
        {
            ExchangeMailboxPlan[] plans = ES.Services.ExchangeServer.GetExchangeMailboxPlans(orgId, false);
            foreach (ExchangeMailboxPlan plan in plans)
            {
                if (!planName2Id.ContainsKey(plan.MailboxPlan))
                {
                    planName2Id.Add(plan.MailboxPlan, plan.MailboxPlanId);
                }
                if (plan.IsDefault)
                {
                    defaultPlanId = plan.MailboxPlanId;
                }
            }
        }

		private void ShowSummary()
		{
			Log.WriteLine(string.Format("{0} line(s) processed", index));
			Log.WriteLine(string.Format("{0} mailbox(s) imported", totalMailboxes));
			Log.WriteLine(string.Format("{0} contact(s) imported", totalContacts));
			Log.WriteLine(string.Format("{0} user(s) imported", totalUsers));
		}

		private bool ProcessLine(int index, int orgId, string line, Regex regex)
		{
			//ignore null string
			if (string.IsNullOrEmpty(line))
			{
				return true;
			}

			string[] cells = regex.Split(line);

			for (int i = 0; i < cells.Length; i++)
			{
				cells[i] = cells[i].Trim(new char[] { '"' });
			}

			// process first row  - document heading
			// calculate indexes of required columns
			if (index == 0)
			{
				for (int i = 0; i < cells.Length; i++)
				{
					if (StringEquals(cells[i], "Display Name"))
						DisplayNameIndex = i;
					else if (StringEquals(cells[i], "E-mail Address"))
						EmailAddressIndex = i;
					else if (StringEquals(cells[i], "Password"))
						PasswordIndex = i;
					else if (StringEquals(cells[i], "First Name"))
						FirstNameIndex = i;
					else if (StringEquals(cells[i], "Middle Name"))
						MiddleNameIndex = i;
					else if (StringEquals(cells[i], "Last Name"))
						LastNameIndex = i;
					else if (StringEquals(cells[i], "Type"))
						TypeIndex = i;

					else if (StringEquals(cells[i], "Address"))
						AddressIndex = i;
					else if (StringEquals(cells[i], "City"))
						CityIndex = i;
					else if (StringEquals(cells[i], "State"))
						StateIndex = i;
					else if (StringEquals(cells[i], "Zip"))
						ZipIndex = i;
					else if (StringEquals(cells[i], "Country"))
						CountryIndex = i;
					else if (StringEquals(cells[i], "Job Title"))
						JobTitleIndex = i;
					else if (StringEquals(cells[i], "Company"))
						CompanyIndex = i;
					else if (StringEquals(cells[i], "Department"))
						DepartmentIndex = i;
					else if (StringEquals(cells[i], "Office"))
						OfficeIndex = i;
					else if (StringEquals(cells[i], "Business Phone"))
						BusinessPhoneIndex = i;
					else if (StringEquals(cells[i], "Fax"))
						FaxIndex = i;
					else if (StringEquals(cells[i], "Home Phone"))
						HomePhoneIndex = i;
					else if (StringEquals(cells[i], "Mobile Phone"))
						MobilePhoneIndex = i;
					else if (StringEquals(cells[i], "Pager"))
						PagerIndex = i;
					else if (StringEquals(cells[i], "Web Page"))
						WebPageIndex = i;
					else if (StringEquals(cells[i], "Notes"))
						NotesIndex = i;
                    else if (StringEquals(cells[i], "Mailbox Plan"))
                        PlanIndex = i;
                }
				return true;
			}
			//check csv structure
			if (TypeIndex == -1)
			{
				Log.WriteError("Column 'Type' not found");
				return false;
			}
			if (DisplayNameIndex == -1)
			{
				Log.WriteError("Column 'Display Name' not found");
				return false;
			}
			if (EmailAddressIndex == -1)
			{
				Log.WriteError("Column 'E-mail Address' not found");
				return false;
			}
			if (PasswordIndex == -1)
			{
				Log.WriteError("Column 'Password' not found");
				return false;
			}
			if (FirstNameIndex == -1)
			{
				Log.WriteError("Column 'First Name' not found");
				return false;
			}
			if (MiddleNameIndex == -1)
			{
				Log.WriteError("Column 'Middle Name' not found");
				return false;
			}
			if (LastNameIndex == -1)
			{
				Log.WriteError("Column 'Last Name' not found");
				return false;
			}
			if (AddressIndex == -1)
			{
				Log.WriteError("Column 'Address' not found");
				return false;
			}
			if (CityIndex == -1)
			{
				Log.WriteError("Column 'City' not found");
				return false;
			}
			if (StateIndex == -1)
			{
				Log.WriteError("Column 'State' not found");
				return false;
			}
			if (ZipIndex == -1)
			{
				Log.WriteError("Column 'Zip' not found");
				return false;
			}
			if (CountryIndex == -1)
			{
				Log.WriteError("Column 'Country' not found");
				return false;
			}
			if (JobTitleIndex == -1)
			{
				Log.WriteError("Column 'Job Title' not found");
				return false;
			}
			if (CompanyIndex == -1)
			{
				Log.WriteError("Column 'Company' not found");
				return false;
			}
			if (DepartmentIndex == -1)
			{
				Log.WriteError("Column 'Department' not found");
				return false;
			}
			if (OfficeIndex == -1)
			{
				Log.WriteError("Column 'Office' not found");
				return false;
			}
			if (BusinessPhoneIndex == -1)
			{
				Log.WriteError("Column 'Last Name' not found");
				return false;
			}
			if (FaxIndex == -1)
			{
				Log.WriteError("Column 'Fax' not found");
				return false;
			}
			if (HomePhoneIndex == -1)
			{
				Log.WriteError("Column 'Home Phone' not found");
				return false;
			}
			if (MobilePhoneIndex == -1)
			{
				Log.WriteError("Column 'Mobile Phone' not found");
				return false;
			}
			if (PagerIndex == -1)
			{
				Log.WriteError("Column 'Pager' not found");
				return false;
			}
			if (WebPageIndex == -1)
			{
				Log.WriteError("Column 'WebPage' not found");
				return false;
			}
			if (NotesIndex == -1)
			{
				Log.WriteError("Column 'Notes' not found");
				return false;
			}

			string typeName = cells[TypeIndex];
			string displayName = cells[DisplayNameIndex];
			string emailAddress = cells[EmailAddressIndex];
			string password = cells[PasswordIndex];
			string firstName = cells[FirstNameIndex];
			string middleName = cells[MiddleNameIndex];
			string lastName = cells[LastNameIndex];
			string address = cells[AddressIndex];
			string city = cells[CityIndex];
			string state = cells[StateIndex];
			string zip = cells[ZipIndex];
			string country = cells[CountryIndex];
			string jobTitle = cells[JobTitleIndex];
			string company = cells[CompanyIndex];
			string department = cells[DepartmentIndex];
			string office = cells[OfficeIndex];
			string businessPhone = cells[BusinessPhoneIndex];
			string fax = cells[FaxIndex];
			string homePhone = cells[HomePhoneIndex];
			string mobilePhone = cells[MobilePhoneIndex];
			string pager = cells[PagerIndex];
			string webPage = cells[WebPageIndex];
			string notes = cells[NotesIndex];

            int planId;
            // do we have plan-column?
            if (PlanIndex > -1)
            {
                string planName = cells[PlanIndex];
                if (!planName2Id.TryGetValue(planName, out planId))
                {
                    Log.WriteInfo(String.Format("Warning at line {0}: Plan named {1} does not exist!", index + 1, planName));
                    // fall back to default plan
                    planId = defaultPlanId;
                }
            }
                // or not?
            else
            {
                // fall back to default plan
                planId = defaultPlanId;
            }
            if (planId < 0)
            {
                Log.WriteError(string.Format("Error at line {0}: No valid plan name and/or no valid default plan", index + 1));
                return false;
            }



			if (string.IsNullOrEmpty(typeName))
			{
				Log.WriteError(string.Format("Error at line {0}: field 'Type' is empty", index + 1));
				return false;
			}

			if (!StringEquals(typeName, "Mailbox") &&
				!StringEquals(typeName, "Contact") &&
				!StringEquals(typeName, "User")&&
                !StringEquals(typeName, "Room")&&
                !StringEquals(typeName, "Equipment")&&
                !StringEquals(typeName, "SharedMailbox"))
			{
                Log.WriteError(string.Format("Error at line {0}: field 'Type' is invalid. Should be 'Mailbox' or 'Contact' or 'User' or 'Room' or 'Equipment' or 'SharedMailbox'", index + 1));
				return false;
			}

			AccountTypes type = (AccountTypes)Enum.Parse(typeof(AccountTypes), typeName, true);

			if (string.IsNullOrEmpty(displayName))
			{
				Log.WriteError(string.Format("Error at line {0}: field 'Display Name' is empty", index + 1));
				return false;
			}
			if (string.IsNullOrEmpty(emailAddress))
			{
				Log.WriteError(string.Format("Error at line {0}: field 'E-mail Address' is empty", index + 1));
				return false;
			}
			if (emailAddress.IndexOf("@") == -1)
			{
				Log.WriteError(string.Format("Error at line {0}: field 'E-mail Address' is invalid", index + 1));
				return false;
			}
			if (type == AccountTypes.Mailbox && string.IsNullOrEmpty(password))
			{
				Log.WriteError(string.Format("Error at line {0}: field 'Password' is empty", index + 1));
				return false;
			}
			if (!string.IsNullOrEmpty(middleName) && middleName.Length > 6)
			{
				middleName = middleName.Substring(0, 6);
				Log.WriteInfo(string.Format("Warning at line {0}: field 'Middle Name' was truncated to 6 symbols", index + 1));
			}

			if (type == AccountTypes.Mailbox)
			{
				//create mailbox using web service
				if (!CreateMailbox(ExchangeAccountType.Mailbox, index, orgId, displayName, emailAddress, password, firstName, middleName, lastName,
					address, city, state, zip, country, jobTitle, company, department, office,
					businessPhone, fax, homePhone, mobilePhone, pager, webPage, notes, planId))
				{
					return false;
				}
				totalMailboxes++;
			}
            if (type == AccountTypes.Room)
            {
                //create mailbox using web service
                if (!CreateMailbox(ExchangeAccountType.Room, index, orgId, displayName, emailAddress, password, firstName, middleName, lastName,
                    address, city, state, zip, country, jobTitle, company, department, office,
                    businessPhone, fax, homePhone, mobilePhone, pager, webPage, notes, planId))
                {
                    return false;
                }
                totalMailboxes++;
            }
            if (type == AccountTypes.Equipment)
            {
                //create mailbox using web service
                if (!CreateMailbox(ExchangeAccountType.Equipment, index, orgId, displayName, emailAddress, password, firstName, middleName, lastName,
                    address, city, state, zip, country, jobTitle, company, department, office,
                    businessPhone, fax, homePhone, mobilePhone, pager, webPage, notes, planId))
                {
                    return false;
                }
                totalMailboxes++;
            }
            if (type == AccountTypes.SharedMailbox)
            {
                //create mailbox using web service
                if (!CreateMailbox(ExchangeAccountType.SharedMailbox, index, orgId, displayName, emailAddress, password, firstName, middleName, lastName,
                    address, city, state, zip, country, jobTitle, company, department, office,
                    businessPhone, fax, homePhone, mobilePhone, pager, webPage, notes, planId))
                {
                    return false;
                }
                totalMailboxes++;
            }



			else if (type == AccountTypes.Contact)
			{
				//create contact using web service
				if (!CreateContact(index, orgId, displayName, emailAddress, firstName, middleName, lastName,
					address, city, state, zip, country, jobTitle, company, department, office,
					businessPhone, fax, homePhone, mobilePhone, pager, webPage, notes))
				{
					return false;
				}
				totalContacts++;
			}
			else if (type == AccountTypes.User)
			{
				//create user using web service
				if (!CreateUser(index, orgId, displayName, emailAddress, password, firstName, middleName, lastName,
					address, city, state, zip, country, jobTitle, company, department, office,
					businessPhone, fax, homePhone, mobilePhone, pager, webPage, notes))
				{
					return false;
				}
				totalUsers++;
			}

			return true;
		}

		/// <summary>
		/// Creates mailbox
		/// </summary>
        private bool CreateMailbox(ExchangeAccountType exchangeAccountType, int index, int orgId, string displayName, string emailAddress, string password, string firstName, string middleName, string lastName,
								string address, string city, string state, string zip, string country, string jobTitle, string company, string department, string office,
								string businessPhone, string fax, string homePhone, string mobilePhone, string pager, string webPage, string notes, int planId)
		{
			bool ret = false;
			try
			{
				string name = emailAddress.Substring(0, emailAddress.IndexOf("@"));
				string domain = emailAddress.Substring(emailAddress.IndexOf("@") + 1);

				//create mailbox
				//ES.Services.ExchangeServer.
				string accountName = string.Empty;
                int accountId = ES.Services.ExchangeServer.CreateMailbox(orgId, 0, exchangeAccountType, accountName, displayName, name, domain, password, false, string.Empty, planId, -1, string.Empty, false);
				if (accountId < 0)
				{
					string errorMessage = GetErrorMessage(accountId);
					Log.WriteError(string.Format("Error at line {0}: {1}", index + 1, errorMessage));
					return false;
				}
                //ExchangeMailbox mailbox = ES.Services.ExchangeServer.GetMailboxGeneralSettings(orgId, accountId);
                OrganizationUser mailbox = ES.Services.Organizations.GetUserGeneralSettings(orgId, accountId);

				mailbox.FirstName = firstName;
				mailbox.Initials = middleName;
				mailbox.LastName = lastName;
				mailbox.Address = address;
				mailbox.City = city;
				mailbox.State = state;
				mailbox.Zip = zip;
				mailbox.Country = country;
				mailbox.JobTitle = jobTitle;
				mailbox.Company = company;
				mailbox.Department = department;
				mailbox.Office = office;
				mailbox.BusinessPhone = businessPhone;
				mailbox.Fax = fax;
				mailbox.HomePhone = homePhone;
				mailbox.MobilePhone = mobilePhone;
				mailbox.Pager = pager;
				mailbox.WebPage = webPage;
				mailbox.Notes = notes;

				//update mailbox
                /*
				ES.Services.ExchangeServer.SetMailboxGeneralSettings(orgId, accountId, mailbox.DisplayName,
					null, mailbox.HideFromAddressBook, mailbox.Disabled, mailbox.FirstName, mailbox.Initials,
					mailbox.LastName, mailbox.Address, mailbox.City, mailbox.State, mailbox.Zip, mailbox.Country,
					mailbox.JobTitle, mailbox.Company, mailbox.Department, mailbox.Office, null, mailbox.BusinessPhone,
					mailbox.Fax, mailbox.HomePhone, mailbox.MobilePhone, mailbox.Pager, mailbox.WebPage, mailbox.Notes);
                */
                ES.Services.Organizations.SetUserGeneralSettings(orgId, accountId, mailbox.DisplayName,
                    null, /*mailbox.HideFromAddressBook*/ false, mailbox.Disabled, mailbox.Locked, mailbox.FirstName, mailbox.Initials,
                    mailbox.LastName, mailbox.Address, mailbox.City, mailbox.State, mailbox.Zip, mailbox.Country,
                    mailbox.JobTitle, mailbox.Company, mailbox.Department, mailbox.Office, null, mailbox.BusinessPhone,
                    mailbox.Fax, mailbox.HomePhone, mailbox.MobilePhone, mailbox.Pager, mailbox.WebPage, mailbox.Notes,
                    // these are new and not in csv ...
                    mailbox.ExternalEmail, mailbox.SubscriberNumber,mailbox.LevelId, mailbox.IsVIP, false);
                ret = true;
			}
			catch (Exception ex)
			{
				Log.WriteError(string.Format("Error at line {0}: Unable to create mailbox", index + 1), ex);
			}
			return ret;
		}

		private string GetErrorMessage(int errorCode)
		{
			string errorMessage = "Unspecified error";
			switch (errorCode)
			{
				case BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS:
					errorMessage = "Email already exists";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_DELETE_SOME_PROBLEMS:
					errorMessage = "Unspecified delete error";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_MAILBOXES_QUOTA_LIMIT:
					errorMessage = "Mailbox quota reached";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_CONTACTS_QUOTA_LIMIT:
					errorMessage = "Contact quota reached";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_DLISTS_QUOTA_LIMIT:
					errorMessage = "Distribution list quota reached";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_PFOLDERS_QUOTA_LIMIT:
					errorMessage = "Public folder quota reached";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_DOMAINS_QUOTA_LIMIT:
					errorMessage = "Domain quota reached";
					break;
				case BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES:
					errorMessage = "Storage quota reached";
					break;
			}
			return errorMessage;
		}

		private bool CreateContact(int index, int orgId, string displayName, string emailAddress, string firstName, string middleName, string lastName,
								string address, string city, string state, string zip, string country, string jobTitle, string company, string department, string office,
								string businessPhone, string fax, string homePhone, string mobilePhone, string pager, string webPage, string notes)
		{
			bool ret = false;
			try
			{
				string name = emailAddress.Substring(0, emailAddress.IndexOf("@"));
				string domain = emailAddress.Substring(emailAddress.IndexOf("@") + 1);

				//create contact
				int accountId = ES.Services.ExchangeServer.CreateContact(orgId, displayName, emailAddress);
				if (accountId < 0)
				{
					string errorMessage = GetErrorMessage(accountId);
					Log.WriteError(string.Format("Error at line {0}: {1}", index + 1, errorMessage));
					return false;
				}
				ExchangeContact contact = ES.Services.ExchangeServer.GetContactGeneralSettings(orgId, accountId);

				contact.FirstName = firstName;
				contact.Initials = middleName;
				contact.LastName = lastName;
				contact.Address = address;
				contact.City = city;
				contact.State = state;
				contact.Zip = zip;
				contact.Country = country;
				contact.JobTitle = jobTitle;
				contact.Company = company;
				contact.Department = department;
				contact.Office = office;
				contact.BusinessPhone = businessPhone;
				contact.Fax = fax;
				contact.HomePhone = homePhone;
				contact.MobilePhone = mobilePhone;
				contact.Pager = pager;
				contact.WebPage = webPage;
				contact.Notes = notes;

				//update mailbox
				ES.Services.ExchangeServer.SetContactGeneralSettings(orgId, accountId, contact.DisplayName, contact.EmailAddress,
					contact.HideFromAddressBook, contact.FirstName, contact.Initials,
					contact.LastName, contact.Address, contact.City, contact.State, contact.Zip, contact.Country,
					contact.JobTitle, contact.Company, contact.Department, contact.Office, null, contact.BusinessPhone,
					contact.Fax, contact.HomePhone, contact.MobilePhone, contact.Pager, contact.WebPage, contact.Notes, contact.UseMapiRichTextFormat);

				ret = true;
			}
			catch (Exception ex)
			{
				Log.WriteError(string.Format("Error at line {0}: Unable to create contact", index + 1), ex);
			}
			return ret;
		}

		private bool CreateUser(int index, int orgId, string displayName, string emailAddress, string password, string firstName, string middleName, string lastName,
								string address, string city, string state, string zip, string country, string jobTitle, string company, string department, string office,
								string businessPhone, string fax, string homePhone, string mobilePhone, string pager, string webPage, string notes)
		{
			bool ret = false;
			try
			{
				string name = emailAddress.Substring(0, emailAddress.IndexOf("@"));
				string domain = emailAddress.Substring(emailAddress.IndexOf("@") + 1);
				string accountName = string.Empty;
				int accountId = ES.Services.Organizations.CreateUser(orgId, displayName, name, domain, password, string.Empty,false, string.Empty);

				if (accountId < 0)
				{
					string errorMessage = GetErrorMessage(accountId);
					Log.WriteError(string.Format("Error at line {0}: {1}", index + 1, errorMessage));
					return false;
				}
				OrganizationUser user = ES.Services.Organizations.GetUserGeneralSettings(orgId, accountId);

				user.FirstName = firstName;
				user.Initials = middleName;
				user.LastName = lastName;
				user.Address = address;
				user.City = city;
				user.State = state;
				user.Zip = zip;
				user.Country = country;
				user.JobTitle = jobTitle;
				user.Company = company;
				user.Department = department;
				user.Office = office;
				user.BusinessPhone = businessPhone;
				user.Fax = fax;
				user.HomePhone = homePhone;
				user.MobilePhone = mobilePhone;
				user.Pager = pager;
				user.WebPage = webPage;
				user.Notes = notes;

				//update 
				ES.Services.Organizations.SetUserGeneralSettings(orgId, accountId, user.DisplayName,
					null, false, user.Disabled, user.Locked, user.FirstName, user.Initials,
					user.LastName, user.Address, user.City, user.State, user.Zip, user.Country,
					user.JobTitle, user.Company, user.Department, user.Office, null, user.BusinessPhone,
					user.Fax, user.HomePhone, user.MobilePhone, user.Pager, user.WebPage, user.Notes, user.ExternalEmail, user.SubscriberNumber, user.LevelId, user.IsVIP, false);
				ret = true;
			}
			catch (Exception ex)
			{
				Log.WriteError(string.Format("Error at line {0}: Unable to create user", index + 1), ex);
			}
			return ret;
		}
		private bool Connect(string server, string username, string password)
		{
			bool ret = true;
			serverContext = new ServerContext();
			serverContext.Server = server;
			serverContext.Username = username;
			serverContext.Password = password;

			ES.InitializeServices(serverContext);
			int status = -1;
			try
			{
				status = ES.Services.Authentication.AuthenticateUser(serverContext.Username, serverContext.Password, null);
			}
			catch (Exception ex)
			{
				Log.WriteError("Authentication error", ex);
				return false;
			}

			string errorMessage = "Check your internet connection or server URL.";
			if (status != 0)
			{
				switch (status)
				{
					case ERROR_USER_WRONG_USERNAME:
						errorMessage = "Wrong username.";
						break;
					case ERROR_USER_WRONG_PASSWORD:
						errorMessage = "Wrong password.";
						break;
					case ERROR_USER_ACCOUNT_CANCELLED:
						errorMessage = "Account cancelled.";
						break;
					case ERROR_USER_ACCOUNT_PENDING:
						errorMessage = "Account pending.";
						break;
				}
				Log.WriteError(
					string.Format("Cannot connect to the remote server. {0}", errorMessage));
				ret = false;
			}
			return ret;
		}

		private bool StringEquals(string str1, string str2)
		{
			return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
