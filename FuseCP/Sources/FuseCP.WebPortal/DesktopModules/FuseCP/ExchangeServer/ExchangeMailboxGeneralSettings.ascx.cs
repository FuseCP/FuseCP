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
using System.Diagnostics;
using System.Text;
using System.Web.UI.WebControls;
using FuseCP.Providers.HostedSolution;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal.ExchangeServer
{
    public partial class ExchangeMailboxGeneralSettings : FuseCPModuleBase
    {
        private const string bookingRequestCustom = "Custom";
        private const string bookingRequestAuto = "Auto";
        private const string bookingRequestDelegates = "Delegates";

        private PackageContext cntx = null;
        private PackageContext Cntx
        {
            get 
            {
                if (cntx == null)
                    cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);

                return cntx;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Utils.CheckQouta(Quotas.EXCHANGE2007_DISCLAIMERSALLOWED, Cntx))
                {
                    ddDisclaimer.Items.Add(new System.Web.UI.WebControls.ListItem("None", "-1"));
                    ExchangeDisclaimer[] disclaimers = ES.Services.ExchangeServer.GetExchangeDisclaimers(PanelRequest.ItemID);
                    foreach (ExchangeDisclaimer disclaimer in disclaimers)
                        ddDisclaimer.Items.Add(new System.Web.UI.WebControls.ListItem(disclaimer.DisclaimerName, disclaimer.ExchangeDisclaimerId.ToString()));
                }
                else
                {
                    locDisclaimer.Visible = false;
                    ddDisclaimer.Visible = false;
                }

                BindSettings();

                UserInfo user = UsersHelper.GetUser(PanelSecurity.EffectiveUserId);

                if (user != null)
                {
                    
                    if ((user.Role == UserRole.User) & (Utils.CheckQouta(Quotas.EXCHANGE2007_ISCONSUMER, Cntx)))
                    {
                        chkHideAddressBook.Visible = false;
                        chkDisable.Visible = false;
                    }

                }

                secRetentionPolicy.Visible = Utils.CheckQouta(Quotas.EXCHANGE2013_ALLOWRETENTIONPOLICY, Cntx);

                if (GetLocalizedString("buttonPanel.OnSaveClientClick") != null)
                    buttonPanel.OnSaveClientClick = GetLocalizedString("buttonPanel.OnSaveClientClick");
            }

            int planId = -1;
            int.TryParse(mailboxPlanSelector.MailboxPlanId, out planId);
            ExchangeMailboxPlan plan = ES.Services.ExchangeServer.GetExchangeMailboxPlan(PanelRequest.ItemID, planId);

            if (plan != null)
            {
                secArchiving.Visible = plan.EnableArchiving;
                secLitigationHoldSettings.Visible = plan.AllowLitigationHold && Utils.CheckQouta(Quotas.EXCHANGE2007_ALLOWLITIGATIONHOLD, Cntx);
            }
            else
            {
                secArchiving.Visible = false;
                secLitigationHoldSettings.Visible = false;
            }
        }

        private void BindSettings()
        {
            string currentStep = "mailbox general settings";

            try
            {
                // get settings
                currentStep = "mailbox general settings";
                ExchangeMailbox mailbox = ES.Services.ExchangeServer.GetMailboxGeneralSettings(PanelRequest.ItemID,
                    PanelRequest.AccountID);

                //get statistics
                currentStep = "mailbox statistics";
                ExchangeMailboxStatistics stats = ES.Services.ExchangeServer.GetMailboxStatistics(PanelRequest.ItemID,
                    PanelRequest.AccountID);

                // mailbox plan

                // title
                litDisplayName.Text = mailbox.DisplayName;

                // bind form
                chkHideAddressBook.Checked = mailbox.HideFromAddressBook;
                chkDisable.Checked = mailbox.Disabled;

                lblExchangeGuid.Text = string.IsNullOrEmpty(mailbox.ExchangeGuid) ? "<>" : mailbox.ExchangeGuid ;

                // get account meta
                currentStep = "mailbox account metadata";
                ExchangeAccount account = ES.Services.ExchangeServer.GetAccount(PanelRequest.ItemID, PanelRequest.AccountID);

                // get mailbox plan
                currentStep = "mailbox plan";
                ExchangeMailboxPlan plan = ES.Services.ExchangeServer.GetExchangeMailboxPlan(PanelRequest.ItemID, account.MailboxPlanId);

                chkPmmAllowed.Checked = (account.MailboxManagerActions & MailboxManagerActions.GeneralSettings) > 0;

                if (account.MailboxPlanId == 0)
                {
                    mailboxPlanSelector.AddNone = true;
                    mailboxPlanSelector.MailboxPlanId = "-1";
                }
                else
                {
                    mailboxPlanSelector.MailboxPlanId = account.MailboxPlanId.ToString();
                }

                if (account.ArchivingMailboxPlanId<1)
                {
                    mailboxRetentionPolicySelector.MailboxPlanId = "-1";
                }
                else
                {
                    mailboxRetentionPolicySelector.MailboxPlanId = account.ArchivingMailboxPlanId.ToString();
                }

                mailboxSize.QuotaUsedValue = Convert.ToInt32(stats.TotalSize / 1024 / 1024);
                mailboxSize.QuotaValue = (stats.MaxSize == -1) ? -1 : (int)Math.Round((double)(stats.MaxSize / 1024 / 1024));

                bool resourceMailbox = ((account.AccountType == ExchangeAccountType.Equipment) || (account.AccountType == ExchangeAccountType.Room));
                secBookingDelegates.Visible = resourceMailbox;
                secBookingOptions.Visible = resourceMailbox;
                trCapacity.Visible = resourceMailbox;
                if (resourceMailbox)
                {
                    currentStep = "resource mailbox settings";
                    ExchangeResourceMailboxSettings resourceSettings = ES.Services.ExchangeServer.GetResourceMailboxSettings(PanelRequest.ItemID, PanelRequest.AccountID);

                    txtCapacity.Text = resourceSettings.ResourceCapacity >= 0 ? resourceSettings.ResourceCapacity.ToString() : "";

                    if (resourceSettings.AutomateProcessing != CalendarProcessingFlags.AutoAccept || resourceSettings.AllBookInPolicy == resourceSettings.AllRequestInPolicy)
                    {
                        rblBookingRequests.SelectedValue = bookingRequestCustom;
                    }
                    else
                    {
                        rblBookingRequests.Items.FindByValue(bookingRequestCustom).Attributes.Add("hidden", "hidden");
                        if (resourceSettings.AllBookInPolicy)
                        {
                            rblBookingRequests.SelectedValue = bookingRequestAuto;
                        }
                        else
                        {
                            rblBookingRequests.SelectedValue = bookingRequestDelegates;
                        }
                    }

                    msDelegates.Visible = bookingRequestDelegates.Equals(rblBookingRequests.SelectedValue);
                    locDelegates.Visible = msDelegates.Visible;

                    if (resourceSettings.ResourceDelegates != null && resourceSettings.ResourceDelegates.Length > 0)
                    {
                        msDelegates.SetAccount(resourceSettings.ResourceDelegates[0]);
                    }

                    chkAllowRecurringMeetings.Checked = resourceSettings.AllowRecurringMeetings;
                    chkScheduleOnlyDuringWorkHours.Checked = resourceSettings.ScheduleOnlyDuringWorkHours;
                    chkEnforceSchedulingHorizon.Checked = resourceSettings.EnforceSchedulingHorizon;

                    txtBookingWindowInDays.Text = resourceSettings.BookingWindowInDays.ToString();
                    txtMaximumDuration.Text = Math.Round((resourceSettings.MaximumDurationInMinutes / 60f), 1).ToString();
                    txtAdditionalResponse.Text = resourceSettings.AdditionalResponse;
                }

                secLitigationHoldSettings.Visible = mailbox.EnableLitigationHold;

                litigationHoldSpace.QuotaUsedValue = Convert.ToInt32(stats.LitigationHoldTotalSize / 1024 / 1024);
                litigationHoldSpace.QuotaValue = (stats.LitigationHoldMaxSize == -1) ? -1 : (int)Math.Round((double)(stats.LitigationHoldMaxSize / 1024 / 1024));

                if (Utils.CheckQouta(Quotas.EXCHANGE2007_DISCLAIMERSALLOWED, Cntx))
                {
                    currentStep = "mailbox disclaimer";
                    int disclaimerId = ES.Services.ExchangeServer.GetExchangeAccountDisclaimerId(PanelRequest.ItemID, PanelRequest.AccountID);
                    ddDisclaimer.SelectedValue = disclaimerId.ToString();
                }

                int ArchivingMaxSize = -1;
                if (plan != null) ArchivingMaxSize = plan.ArchiveSizeMB;
                chkEnableArchiving.Checked = account.EnableArchiving;
                archivingQuotaViewer.QuotaUsedValue = Convert.ToInt32(stats.ArchivingTotalSize / 1024 / 1024);
                archivingQuotaViewer.QuotaValue = ArchivingMaxSize;
                rowArchiving.Visible = chkEnableArchiving.Checked;

                if (account.LevelId > 0 && Cntx.Groups.ContainsKey(ResourceGroups.ServiceLevels))
                {
                    currentStep = "support service level";
                    FuseCP.EnterpriseServer.Base.HostedSolution.ServiceLevel serviceLevel = ES.Services.Organizations.GetSupportServiceLevel(account.LevelId);

                    litServiceLevel.Visible = true;
                    litServiceLevel.Text = serviceLevel.LevelName;
                    litServiceLevel.ToolTip = serviceLevel.LevelDescription;

                }
                imgVipUser.Visible = account.IsVIP && Cntx.Groups.ContainsKey(ResourceGroups.ServiceLevels);

                litMailboxType.Text = account.AccountType.ToString();

                if (account.AccountType == ExchangeAccountType.SharedMailbox)
                    litDisplayName.Text += GetSharedLocalizedString("SharedMailbox.Text");

                if (account.AccountType == ExchangeAccountType.Room)
                    litDisplayName.Text += GetSharedLocalizedString("RoomMailbox.Text");

                if (account.AccountType == ExchangeAccountType.Equipment)
                    litDisplayName.Text += GetSharedLocalizedString("EquipmentMailbox.Text");

            }
            catch (Exception ex)
            {
                Exception detailedException = BuildBindSettingsException(currentStep, ex);
                System.Diagnostics.Trace.TraceError(detailedException.ToString());
                messageBox.ShowErrorMessage("EXCHANGE_GET_MAILBOX_SETTINGS", detailedException);
            }
        }

        private Exception BuildBindSettingsException(string currentStep, Exception ex)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat("Failed while loading {0} for mailbox item {1}, account {2}.", currentStep, PanelRequest.ItemID, PanelRequest.AccountID);

            Exception current = ex;
            int level = 0;
            while (current != null)
            {
                message.Append(' ');
                message.AppendFormat("[{0}] {1}", level, current.Message);
                current = current.InnerException;
                level++;
            }

            return new ApplicationException(message.ToString(), ex);
        }

        private bool SaveSettings()
        {
            if (!Page.IsValid)
                return false;

            try
            {
                if (mailboxPlanSelector.MailboxPlanId == "-1")
                {
                    messageBox.ShowErrorMessage("EXCHANGE_SPECIFY_PLAN");
                    return false;
                }

                int result = ES.Services.ExchangeServer.SetMailboxGeneralSettings(
                    PanelRequest.ItemID, PanelRequest.AccountID,
                    chkHideAddressBook.Checked,
                    chkDisable.Checked);

                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return false;
                }
                else
                {
                    int planId = Convert.ToInt32(mailboxPlanSelector.MailboxPlanId);
                    int policyId = -1;
                    int.TryParse(mailboxRetentionPolicySelector.MailboxPlanId, out policyId);

                    bool EnableArchiving = chkEnableArchiving.Checked;

                    result = ES.Services.ExchangeServer.SetExchangeMailboxPlan(PanelRequest.ItemID, PanelRequest.AccountID, planId,
                        policyId, EnableArchiving);

                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return false;
                    }
                }

                if (Utils.CheckQouta(Quotas.EXCHANGE2007_DISCLAIMERSALLOWED, Cntx))
                {
                    int disclaimerId;
                    if (int.TryParse(ddDisclaimer.SelectedValue, out disclaimerId))
                        ES.Services.ExchangeServer.SetExchangeAccountDisclaimerId(PanelRequest.ItemID, PanelRequest.AccountID, disclaimerId);
                }

                String accountType = litMailboxType.Text;
                bool resourceMailbox = ((ExchangeAccountType.Equipment.ToString().Equals(accountType)) || (ExchangeAccountType.Room.ToString().Equals(accountType)));
                if (resourceMailbox)
                {
                    ExchangeResourceMailboxSettings resourceSettings = new ExchangeResourceMailboxSettings();

                    int capacity = -1;
                    if (!String.IsNullOrEmpty(txtCapacity.Text)) int.TryParse(txtCapacity.Text, out capacity);
                    resourceSettings.ResourceCapacity = capacity;
                    if (bookingRequestAuto.Equals(rblBookingRequests.SelectedValue))
                    {
                        resourceSettings.AutomateProcessing = CalendarProcessingFlags.AutoAccept;
                        resourceSettings.AllBookInPolicy = true;
                        resourceSettings.AllRequestInPolicy = false;
                    }
                    else if (bookingRequestDelegates.Equals(rblBookingRequests.SelectedValue))
                    {
                        resourceSettings.AutomateProcessing = CalendarProcessingFlags.AutoAccept;
                        resourceSettings.AllBookInPolicy = false;
                        resourceSettings.AllRequestInPolicy = true;
                    }
                    else
                    {
                        resourceSettings.AutomateProcessing = CalendarProcessingFlags.AutoUpdate;
                    }

                    if (!String.IsNullOrEmpty(msDelegates.GetAccount()))
                    {
                        ExchangeAccount account = new ExchangeAccount();
                        account.AccountName = msDelegates.GetAccount();
                        account.AccountId = msDelegates.GetAccountId();
                        resourceSettings.ResourceDelegates = new ExchangeAccount[] { account };
                    }

                    resourceSettings.AllowRecurringMeetings = chkAllowRecurringMeetings.Checked;
                    resourceSettings.ScheduleOnlyDuringWorkHours = chkScheduleOnlyDuringWorkHours.Checked;
                    resourceSettings.EnforceSchedulingHorizon = chkEnforceSchedulingHorizon.Checked;

                    int bookingWindowInDays = 0;
                    int.TryParse(txtBookingWindowInDays.Text, out bookingWindowInDays);
                    resourceSettings.BookingWindowInDays = bookingWindowInDays;

                    if (!String.IsNullOrEmpty(txtMaximumDuration.Text))
                    {
                        float maximumDuration = 0;
                        float.TryParse(txtMaximumDuration.Text, out maximumDuration);
                        resourceSettings.MaximumDurationInMinutes = (int)Math.Round(maximumDuration * 60);
                    }

                    if (!String.IsNullOrEmpty(txtAdditionalResponse.Text))
                    {
                        resourceSettings.AddAdditionalResponse = true;
                        resourceSettings.AdditionalResponse = txtAdditionalResponse.Text;
                    }

                    result = ES.Services.ExchangeServer.SetResourceMailboxSettings(PanelRequest.ItemID, PanelRequest.AccountID, resourceSettings);
                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return false;
                    }
                }

                messageBox.ShowSuccessMessage("EXCHANGE_UPDATE_MAILBOX_SETTINGS");
                BindSettings();
                return true;
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("EXCHANGE_UPDATE_MAILBOX_SETTINGS", ex);
                return false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        protected void btnSaveExit_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
            {
                Response.Redirect(PortalUtils.EditUrl("ItemID", PanelRequest.ItemID.ToString(),
                "mailboxes",
                "SpaceID=" + PanelSecurity.PackageId));
            }
        }


        protected void chkPmmAllowed_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int result = ES.Services.ExchangeServer.SetMailboxManagerSettings(PanelRequest.ItemID, PanelRequest.AccountID,
                chkPmmAllowed.Checked, MailboxManagerActions.GeneralSettings);

                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }

                messageBox.ShowSuccessMessage("EXCHANGE_UPDATE_MAILMANAGER");
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("EXCHANGE_UPDATE_MAILMANAGER", ex);
            }
        }

        private int ConvertMbxSizeToIntMB(string inputValue)
        {
            int result = 0;

            if ((inputValue == null) || (inputValue == ""))
                return 0;

            if (inputValue.Contains("TB"))
            {
                result = Convert.ToInt32(inputValue.Replace(" TB", ""));
                result = result * 1024 * 1024;
            }
            else
                if (inputValue.Contains("GB"))
                {
                    result = Convert.ToInt32(inputValue.Replace(" GB", ""));
                    result = result * 1024;
                }
                else
                    if (inputValue.Contains("MB"))
                    {
                        result = Convert.ToInt32(inputValue.Replace(" MB", ""));
                    }

            return result;
        }

        public void mailboxPlanSelector_Changed(object sender, EventArgs e)
        {
        }

        protected void valDelegates_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (msDelegates.GetAccount() != null || !"Delegates".Equals(rblBookingRequests.SelectedValue));
            if (!bookingRequestCustom.Equals(rblBookingRequests.SelectedValue))
            {
                rblBookingRequests.Items.FindByValue(bookingRequestCustom).Attributes.Add("hidden", "hidden");
            }
        }

        protected void rblBookingRequests_SelectedIndexChanged(object sender, EventArgs e)
        {
            msDelegates.Visible = bookingRequestDelegates.Equals(rblBookingRequests.SelectedValue);
            locDelegates.Visible = msDelegates.Visible;
            if (!bookingRequestCustom.Equals(rblBookingRequests.SelectedValue))
            {
                rblBookingRequests.Items.FindByValue(bookingRequestCustom).Attributes.Add("hidden", "hidden");
            }
        }
    }
}
