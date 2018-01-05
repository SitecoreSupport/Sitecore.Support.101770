namespace Sitecore.Support.Shell.Applications.WebEdit.Commands
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.WebEdit;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.UI;

    [Serializable]
    public class FieldEditor : Sitecore.Shell.Applications.WebEdit.Commands.FieldEditor
    {
        protected override PageEditFieldEditorOptions GetOptions(ClientPipelineArgs args, NameValueCollection form)
        {
            PageEditFieldEditorOptions options = base.GetOptions(args, form);
            string commandItemId = args.Parameters["command"];
            Assert.IsNotNullOrEmpty(commandItemId, "Field Editor command expects 'command' parameter");
            Item commandItem = Client.CoreDatabase.GetItem(commandItemId);
            Assert.IsNotNull(commandItem, "command item");
            return new PageEditFieldEditorOptions(form, options.Fields)
            {
                Title = commandItem["Title"],
                Icon = commandItem["Icon"],
                DialogTitle = commandItem["Header"]

            };
        }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length >= 1)
            {
                ClientPipelineArgs args = new ClientPipelineArgs(context.Parameters);
                args.Parameters.Add("uri", context.Items[0].Uri.ToString());
                Context.ClientPage.Start(this, "StartFieldEditor", args);
            }
        }

        protected new void StartFieldEditor(ClientPipelineArgs args)
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                Page handler = current.Handler as Page;
                if (handler != null)
                {
                    NameValueCollection form = handler.Request.Form;
                    if (form != null)
                    {
                        if (!args.IsPostBack)
                        {
                            PageEditFieldEditorOptions options = this.GetOptions(args, form);
                            ModalDialogOptions modelDialogOptions = new ModalDialogOptions(options.ToUrlString().ToString())
                            {
                                Width = "720",
                                Height = "520",
                                Message = string.Empty,
                                Response = true,
                                MinWidth = null,
                                MinHeight = null,
                                Resizable = true,
                                Header = options.DialogTitle
                            };
                            SheerResponse.ShowModalDialog(modelDialogOptions);
                            args.WaitForPostBack();
                        }
                        else if (args.HasResult)
                        {
                            PageEditFieldEditorOptions.Parse(args.Result).SetPageEditorFieldValues();
                        }
                    }
                }
            }
        }
    }
}


