// Copyright (C) 2026 FuseCP
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

namespace FuseCP.Portal
{
    public partial class SecurityBruteForceAttempts : FuseCPModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chkFailedOnly.Checked = true;
                BindAttempts();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindAttempts();
        }

        private void BindAttempts()
        {
            try
            {
                var attempts = ES.Services.Users.GetBruteForceAttempts(
                    txtIpFilter.Text?.Trim(),
                    ddlLayer.SelectedValue,
                    chkFailedOnly.Checked,
                    0,
                    200);

                gvAttempts.DataSource = attempts;
                gvAttempts.DataBind();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
                DisableControls = true;
            }
        }
    }
}
