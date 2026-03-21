<%@ Control AutoEventWireup="true" %>
<!-- Shared runtime script stack for default portal skins. -->
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/jquery/jquery-2.1.0.min.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/bootstrap/bootstrap.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/fcp-common.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/fcp-charts.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/fcp-elements.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/plugin/plugins.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/jquery-ui/jquery-ui-1.10.4.custom.min.js") %>'></script>
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/jquery/jquery.matchHeight.js") %>'></script>

<style type="text/css">
  #fcp-global-busy-banner {
    position: fixed;
    top: 12px;
    right: 12px;
    z-index: 50000;
    display: none;
    max-width: 420px;
    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.2);
  }
</style>

<div id="fcp-global-busy-banner" class="alert alert-info" role="status" aria-live="polite">
  Processing your request. Please wait...
</div>

<script type="text/javascript">
  (function () {
    if (window.fcpGlobalBusy) {
      return;
    }

    var busy = false;
    var disabledControls = [];

    function getBanner() {
      return document.getElementById('fcp-global-busy-banner');
    }

    function setBanner(message) {
      var banner = getBanner();
      if (!banner) return;
      banner.innerHTML = message || 'Processing your request. Please wait...';
      banner.style.display = 'block';
    }

    function hideBanner() {
      var banner = getBanner();
      if (!banner) return;
      banner.style.display = 'none';
    }

    function shouldSkipControl(control) {
      if (!control) return true;
      if (control.getAttribute('data-busy-ignore') === 'true') return true;
      if ((control.className || '').indexOf('no-busy-lock') >= 0) return true;

      var text = ((control.value || '') + ' ' + (control.innerText || '')).toLowerCase();
      if (text.indexOf('cancel') >= 0 || text.indexOf('close') >= 0 || text.indexOf('back') >= 0) {
        return true;
      }

      return false;
    }

    function disableControl(control) {
      if (!control || shouldSkipControl(control) || control.disabled) return;
      disabledControls.push(control);
      control.disabled = true;
      control.setAttribute('aria-disabled', 'true');
      if ((control.className || '').indexOf('disabled') < 0) {
        control.className = (control.className || '') + ' disabled';
      }
    }

    function disableActionControls() {
      disabledControls = [];

      var actionControls = document.querySelectorAll('input[type="submit"], input[type="button"], button, a.btn, a[role="button"]');
      for (var i = 0; i < actionControls.length; i++) {
        disableControl(actionControls[i]);
      }
    }

    function restoreControls() {
      for (var i = 0; i < disabledControls.length; i++) {
        var control = disabledControls[i];
        if (!control) continue;
        control.disabled = false;
        control.removeAttribute('aria-disabled');
      }
      disabledControls = [];
    }

    function beginBusy(message) {
      if (busy) return false;
      busy = true;
      disableActionControls();
      setBanner(message);
      return true;
    }

    function endBusy() {
      busy = false;
      hideBanner();
      restoreControls();
    }

    window.fcpGlobalBusy = {
      start: beginBusy,
      stop: endBusy
    };

    document.addEventListener('click', function (evt) {
      if (busy) {
        evt.preventDefault();
        evt.stopPropagation();
      }
    }, true);

    document.addEventListener('submit', function (evt) {
      if (busy) {
        evt.preventDefault();
        return;
      }

      var message = 'Processing your request. Please wait...';
      if (evt.submitter && evt.submitter.getAttribute) {
        var custom = evt.submitter.getAttribute('data-busy-message');
        if (custom) message = custom;
      }

      beginBusy(message);
    }, true);

    if (window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager) {
      var prm = window.Sys.WebForms.PageRequestManager.getInstance();
      prm.add_initializeRequest(function () {
        beginBusy('Loading updated content...');
      });
      prm.add_endRequest(function () {
        endBusy();
      });
    }
  })();
</script>
