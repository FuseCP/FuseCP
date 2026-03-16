Name: fusecp
Version:2.0.0.0
Release: 1%{?dist}
Summary: This is the FuseCP Server component
License: Creative Commons Share-alike    
URL: https://www.fusecp.com
Requires: /bin/sh, sed, dotnet-runtime-8.0, aspnetcore-runtime-8.0
AutoReqProv: no
BuildArch: noarch
Source0: %{name}-%{version}.tar.gz

%description
FuseCP is a complete management portal for Cloud Computing Companies
and IT Providers to automate the provisioning of a full suite of Multi-Tenant
services on servers. The powerful, flexible and fully open source FuseCP platform
gives users simple point-and-click control over Server applications including IIS 10,
Microsoft SQL Server 2022, MySQL, MariaDB, Active Directory, Microsoft Exchange 2019,
Microsoft Sharepoint 2019, Microsoft RemoteApp/RDS, Hyper-v and Proxmox Deployments.

%prep
%autosetup

%install
rm -rf $RPM_BUILD_ROOT
mkdir -p $RPM_BUILD_ROOT%{_bindir}
mkdir -p $RPM_BUILD_ROOT/usr/share
cp -rp usr/bin/* $RPM_BUILD_ROOT%{_bindir}
cp -rp usr/share/* $RPM_BUILD_ROOT/usr/share

%post
if [ $1 -ge 1 ];then
    sed -i 's|/usr/bin/fusecp-installer|%{_bindir}/fusecp-installer|g' /usr/share/applications/fusecp-installer.desktop
#  %{_bindir}/fusecp-installer
    rm /usr/share/fusecp/Installer/Setup2.*
    echo "======================================================"
    echo "Please run 'sudo fusecp-installer' to install FuseCP"
    echo "======================================================"
fi

%clean
rm -rf $RPM_BUILD_ROOT

%files
/usr/bin/fusecp-installer
/usr/share/fusecp/Installer/FuseCP.Installer.dll
/usr/share/fusecp/Installer/FuseCP.Installer.deps.json
/usr/share/fusecp/Installer/FuseCP.Installer.runtimeconfig.json
/usr/share/pixmaps/FuseCP.png
/usr/share/applications/fusecp-installer.desktop
/usr/share/doc/fusecp/copyright
/usr/share/doc/fusecp/ChangeLog
/usr/share/doc/fusecp/README
/usr/share/man/man1/fusecp-installer.1.gz

%changelog
* Fri Mar 22 2024 Simon Egli <simon.jakob.egli@gmail.com> - 2.0.0
- First version being packaged
