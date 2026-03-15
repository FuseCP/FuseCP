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

namespace FuseCP.WebDav.Core.Config.Entities
{
    public class TwilioParameters: AbstractConfigCollection
    {
        public string AccountSid { get; private set; }
        public string AuthorizationToken { get; private set; }
        public string PhoneFrom { get; private set; }

        public TwilioParameters()
        {
            AccountSid = ConfigSection.Twilio?.AccountSid ?? string.Empty;
            AuthorizationToken = ConfigSection.Twilio?.AuthorizationToken ?? string.Empty;
            PhoneFrom = ConfigSection.Twilio?.PhoneFrom ?? string.Empty;
        }
    }
}
