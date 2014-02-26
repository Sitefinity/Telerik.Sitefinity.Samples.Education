<asp:HiddenField ID="imagesField" runat="server" />
<script type="text/javascript">
    function pluginLoaded(sender, args) {   // HTML version 

        var imagesField = $get('<%= imagesField.ClientID %>');
        var imagesArr = Sys.Serialization.JavaScriptSerializer.deserialize(imagesField.value);
        slCtl = sender.getHost();

        for (idx in imagesArr) {
            slCtl.Content.mainPage.SetItem(imagesArr[idx]['Title'], imagesArr[idx]['Url']);
        }
    }

    function onSilverlightError(sender, args) {
        var appSource = "";
        if (sender != null && sender != 0) {
            appSource = sender.getHost().Source;
        }

        var errorType = args.ErrorType;
        var iErrorCode = args.ErrorCode;

        if (errorType == "ImageError" || errorType == "MediaError") {
            return;
        }

        var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

        errMsg += "Code: " + iErrorCode + "    \n";
        errMsg += "Category: " + errorType + "       \n";
        errMsg += "Message: " + args.ErrorMessage + "     \n";

        if (errorType == "ParserError") {
            errMsg += "File: " + args.xamlFile + "     \n";
            errMsg += "Line: " + args.lineNumber + "     \n";
            errMsg += "Position: " + args.charPosition + "     \n";
        }
        else if (errorType == "RuntimeError") {
            if (args.lineNumber != 0) {
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            errMsg += "MethodName: " + args.methodName + "     \n";
        }

        throw new Error(errMsg);
    }
</script>


 <div id="silverlightControlHost" style="height: 800px; width: 100%">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		  <param name="source" value='<%= ResolveUrl("~/ClientBin/CoverFlowSilverlight.xap") %>'/>
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="transparent" />
          <param name="onLoad" value="pluginLoaded" />
          <param name="windowless" value="true" />
		  <param name="minRuntimeVersion" value="4.0.50401.0" />
		  <param name="autoUpgrade" value="true" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
          </object>
</div>