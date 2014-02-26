<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InitializeSample.aspx.cs" Inherits="SitefinityWebApp.InitializeSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>This sample has not been started before. Please wait while the data for the sample is being created...</h1>
        <asp:ScriptManager ID="sm" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    </div>
    </form>
</body>
</html>

<script language="javascript">
    Sys.Application.add_load(loadHandler);

    function loadHandler() {
        setTimeout("PageMethods.SetupSample(onSetupSampleComplete);", 2000);
    }

    function onSetupSampleComplete(result) {
        alert(result);
    }
</script>